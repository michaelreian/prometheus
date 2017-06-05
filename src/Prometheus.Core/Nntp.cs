using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Prometheus.Core
{
    public class Article
    {
        public Article(IEnumerable<Header> headers, List<string> body)
        {
            Headers = headers;
            Body = body;
        }

        public IEnumerable<Header> Headers { get; private set; }

        public List<string> Body { get; private set; }
    }

    public static class AuthExtensions
    {
        public static NntpResponse AuthInfoUser(this INntpConnection connection, string username)
        {
            var command = string.Format("AUTHINFO USER {0}", username);

            return connection.ExecuteCommand(command);
        }

        public static NntpResponse AuthInfoPass(this INntpConnection connection, string password)
        {
            var command = string.Format("AUTHINFO PASS {0}", password);

            return connection.ExecuteCommand(command);
        }
    }

    internal class CommandBuilder
    {
        private readonly List<string> parameters = new List<string>();

        private readonly string command;

        private CommandBuilder(string command)
        {
            this.command = command;
        }

        public void AddParameter<T>(T parameter)
        {
            parameters.Add(parameter.ToString());
        }

        public void AddParameter(string format, params object[] args)
        {
            parameters.Add(string.Format(format, args));
        }

        public void AddDateAndTimeParameters(DateTime date)
        {
            AddParameter(date.ToString("yyyyMMdd"));
            AddParameter(date.ToString("HHmmss"));
        }

        public override string ToString()
        {
            return parameters.Any() ? string.Join(" ", new[] { command }.Union(parameters)) : command;
        }

        public static string Build(string command, Action<CommandBuilder> builder)
        {
            var commandBuilder = new CommandBuilder(command);

            builder(commandBuilder);

            return commandBuilder.ToString();
        }
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this ListKeyword listKeyword)
        {
            switch (listKeyword)
            {
                case ListKeyword.Active:
                    return "ACTIVE";
                case ListKeyword.ActiveTimes:
                    return "ACTIVE.TIMES";
                case ListKeyword.DistribPats:
                    return "DISTRIB.PATS";
                case ListKeyword.Headers:
                    return "HEADERS";
                case ListKeyword.NewsGroups:
                    return "NEWSGROUPS";
                case ListKeyword.OverviewFormat:
                    return "OVERVIEW.FMT";
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class Group
    {
        public long FirstArticleNo { get; set; }
        public long LastArticleNo { get; set; }
        public long ArticleCount { get; set; }
        public string GroupName { get; set; }
    }

    public class Header
    {
        private Header(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Key, Value);
        }

        public static Header Create(string line)
        {
            var parts = line.Split(':');
            return new Header(parts[0], parts[1].Trim(' '));
        }
    }

    internal static class HelperExtensions
    {

        public static string WithBrackets(this string value)
        {
            return string.Format("<{0}>", value.Trim('<', '>'));
        }
    }

    public static class InfoExtensions
    {
        public static NntpResponse Date(this INntpConnection connection)
        {
            return connection.ExecuteCommand("DATE");
        }

        public static NntpMultilineResponse Help(this INntpConnection connection)
        {
            return connection.ExecuteMultilineCommand("HELP", 100);
        }

        public static NntpMultilineResponse NewGroups(this INntpConnection connection, DateTime createdSince, bool isGmt = false)
        {
            var command = CommandBuilder.Build("NEWGROUPS", builder =>
            {
                builder.AddDateAndTimeParameters(createdSince);
                if (isGmt)
                {
                    builder.AddParameter("GMT");
                }
            });

            return connection.ExecuteMultilineCommand(command, 231);
        }

        public static NntpMultilineResponse NewNews(this INntpConnection connection, DateTime createdSince, string wildmat, bool isGmt = false)
        {
            var command = CommandBuilder.Build("NEWNEWS", builder =>
            {
                builder.AddParameter(wildmat);
                builder.AddDateAndTimeParameters(createdSince);
                if (isGmt)
                {
                    builder.AddParameter("GMT");
                }
            });

            return connection.ExecuteMultilineCommand(command, 230);
        }

        public static NntpMultilineResponse List(this INntpConnection connection, ListKeyword? keyword = null, string wildmatOrArgument = null)
        {
            var command = CommandBuilder.Build("LIST", builder =>
            {
                if (!keyword.HasValue) return;

                builder.AddParameter(keyword.Value.GetDescription());

                if (!string.IsNullOrWhiteSpace(wildmatOrArgument))
                {
                    builder.AddParameter(wildmatOrArgument);
                }
            });

            return connection.ExecuteMultilineCommand(command, 215);
        }

        public static NntpMultilineResponse Over(this INntpConnection connection, string messageId)
        {
            var command = string.Format("OVER {0}", messageId.WithBrackets());

            return connection.ExecuteMultilineCommand(command, 224);
        }

        public static NntpMultilineResponse Over(this INntpConnection connection, int? articleNo = null)
        {
            var command = CommandBuilder.Build("OVER", builder =>
            {
                if (articleNo.HasValue) builder.AddParameter(articleNo.Value);
            });

            return connection.ExecuteMultilineCommand(command, 224);
        }

        public static NntpMultilineResponse Over(this INntpConnection connection, Range range)
        {
            var command = string.Format("OVER {0}", range);

            return connection.ExecuteMultilineCommand(command, 224);
        }

        public static NntpMultilineResponse Hdr(this INntpConnection connection, string field, string messageId)
        {
            var command = string.Format("HDR {0} {1}", field, messageId.WithBrackets());

            return connection.ExecuteMultilineCommand(command, 225);
        }

        public static NntpMultilineResponse Hdr(this INntpConnection connection, string field, int? articleNo = null)
        {
            var command = CommandBuilder.Build(string.Format("HDR {0}", field), builder =>
            {
                if (articleNo.HasValue) builder.AddParameter(articleNo.Value);
            });

            return connection.ExecuteMultilineCommand(command, 225);
        }

        public static NntpMultilineResponse Hdr(this INntpConnection connection, string field, Range range)
        {
            var command = string.Format("HDR {0} {1}", field, range);

            return connection.ExecuteMultilineCommand(command, 225);
        }
    }

    public interface INntpConnection : IDisposable
    {
        bool IsConnected { get; }

        Task<NntpResponse> Connect(string hostname, int port, bool useSsl);

        NntpResponse ExecuteCommand(string command);

        NntpMultilineResponse ExecuteMultilineCommand(string command, int validCode);

        void Close();
    }

    public enum ListKeyword
    {
        Active,
        ActiveTimes,
        DistribPats,
        Headers,
        NewsGroups,
        OverviewFormat
    }

    public class NntpClient
    {
        private readonly INntpConnection connection;

        public NntpClient() : this(new NntpConnection()) { }

        public NntpClient(INntpConnection connection)
        {
            this.connection = connection;
        }

        public async Task<bool> Connect(string hostname, int port, bool useSsl)
        {
            var connectResponse = await connection.Connect(hostname, port, useSsl);
            return connectResponse.Code == 200 || connectResponse.Code == 201;
        }

        public bool Authenticate(string username, string password = null)
        {
            var userResponse = connection.AuthInfoUser(username);

            if (userResponse.Code == 281) return true;

            if (userResponse.Code != 381 || password == null) return false;

            var passResponse = connection.AuthInfoPass(password);

            return passResponse.Code == 281;
        }

        public bool SetReaderMode()
        {
            var response = connection.ModeReader();
            if (response.Code == 502)
            {
                connection.Close();
                return false;
            }

            return response.Code == 200 || response.Code == 201;
        }

        public Group SelectGroup(string group)
        {
            var response = this.connection.Group(group);
            if (response.Code != 211)
            {
                return null;
            }
            return MapGroup(response);
        }

        private Group MapGroup(NntpResponse response)
        {
            var items = response.Message.Split(' ');
            return new Group
            {
                ArticleCount = long.Parse(items[0]),
                FirstArticleNo = long.Parse(items[1]),
                LastArticleNo = long.Parse(items[2]),
                GroupName = items[3],
            };
        }

        public Article RetrieveArticle(string messageId)
        {
            var response = connection.Article(messageId);
            if (response.Lines == null) return null;

            var articleLines = response.Lines.ToList();

            var headers = articleLines.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).Select(Header.Create);
            var body = articleLines.SkipWhile(x => !string.IsNullOrWhiteSpace(x)).Skip(1).ToList();

            return new Article(headers, body);
        }

        public IEnumerable<Header> RetrieveHeader(string messageId)
        {
            var response = connection.Head(messageId);
            if (response.Lines == null) return Enumerable.Empty<Header>();

            return response.Lines.Select(Header.Create);
        }
    }

    public class NntpConnection : INntpConnection
    {
        private static readonly Encoding DEFAULT_ENCODING = Encoding.GetEncoding("iso-8859-1");

        private readonly TcpClient client = new TcpClient();

        private NntpStreamReader reader;

        private StreamWriter writer;

        public bool IsConnected => client.Connected;

        public async Task<NntpResponse> Connect(string hostname, int port, bool useSsl)
        {
            await client.ConnectAsync(hostname, port);

            var stream = await GetStream(hostname, useSsl);

            reader = new NntpStreamReader(stream, DEFAULT_ENCODING);
            writer = new StreamWriter(stream, DEFAULT_ENCODING) { AutoFlush = true };

            return ReadResponse((code, message) => new NntpResponse(code, message));
        }

        public NntpResponse ExecuteCommand(string command)
        {
            EnsureConnected();

            return ExecuteCommand(command, (code, message) => new NntpResponse(code, message));
        }

        public NntpMultilineResponse ExecuteMultilineCommand(string command, int validCode)
        {
            EnsureConnected();

            return ExecuteCommand(command, (code, message) => new NntpMultilineResponse(code, message, code == validCode ? reader.ReadAllLines() : null));
        }

        private void EnsureConnected()
        {
            if (!IsConnected) throw new NntpException("Must be connected to execute commands.");
        }

        public void Close()
        {
            client.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }

        private TResponse ExecuteCommand<TResponse>(string command, Func<int, string, TResponse> responseFunc)
        {
            writer.WriteLine(command);
            return ReadResponse(responseFunc);
        }

        private TResponse ReadResponse<TResponse>(Func<int, string, TResponse> responseFunc)
        {
            var responseText = reader.ReadLine();
            if (responseText == null)
            {
                throw new NntpException("Did not receive response from server.");
            }

            int code;
            if (!int.TryParse(responseText.Substring(0, 3), out code))
            {
                throw new NntpException("Received invalid response from server.");
            }

            return responseFunc(code, responseText.Substring(4));
        }

        private async Task<Stream> GetStream(string hostname, bool useSsl)
        {
            Stream stream = client.GetStream();
            if (!useSsl) return stream;

            var sslStream = new SslStream(stream);
            await sslStream.AuthenticateAsClientAsync(hostname);

            return sslStream;
        }
    }

    public class NntpException : Exception
    {
        public NntpException(string message) : base(message) { }
    }

    public class NntpMultilineResponse : NntpResponse
    {
        internal NntpMultilineResponse(int code, string message, IEnumerable<string> lines) : base(code, message)
        {
            Lines = lines;
        }

        public IEnumerable<string> Lines { get; private set; }
    }

    public class NntpResponse
    {
        internal NntpResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; private set; }

        public string Message { get; private set; }
    }

    internal class NntpStreamReader : StreamReader
    {
        public NntpStreamReader(Stream stream, System.Text.Encoding encoding) : base(stream, encoding, true) { }

        public IEnumerable<string> ReadAllLines()
        {
            var lines = new List<string>();

            string readLine;
            while ((readLine = ReadLine()) != null)
            {
                if (readLine == ".") break;

                if (readLine.StartsWith(".."))
                    readLine = readLine.Substring(1);

                lines.Add(readLine);
            }

            return lines;
        }
    }

    public class Range
    {
        private readonly long fromArticleNo;

        private long? toArticleNo;

        private Range(long fromArticleNo)
        {
            this.fromArticleNo = fromArticleNo;
        }

        public static Range From(long articleNo)
        {
            return new Range(articleNo);
        }

        public Range To(long articleNo)
        {
            toArticleNo = articleNo;
            return this;
        }

        public override string ToString()
        {
            var range = string.Format("{0}-", fromArticleNo);

            if (toArticleNo.HasValue)
            {
                range += toArticleNo.Value;
            }

            return range;
        }
    }

    public static class RetrievalExtensions
    {
        public static NntpMultilineResponse XOVER(this INntpConnection connection, Range range)
        {
            var command = CommandBuilder.Build("XOVER", builder =>
            {
                builder.AddParameter(range);
            });

            return connection.ExecuteMultilineCommand(command, 224);
        }

        public static NntpMultilineResponse XHDR(this INntpConnection connection, Range range)
        {
            var command = CommandBuilder.Build("XHDR", builder =>
            {
                builder.AddParameter(range);
            });

            return connection.ExecuteMultilineCommand(command, 221);
        }

        public static NntpMultilineResponse Article(this INntpConnection connection, string messageId)
        {
            return ExecuteArticleCommand(connection, "ARTICLE", messageId, 220);
        }

        public static NntpMultilineResponse Article(this INntpConnection connection, int? articleNo = null)
        {
            return ExecuteArticleCommand(connection, "ARTICLE", articleNo, 220);
        }

        public static NntpMultilineResponse Head(this INntpConnection connection, string messageId)
        {
            return ExecuteArticleCommand(connection, "HEAD", messageId, 221);
        }

        public static NntpMultilineResponse Head(this INntpConnection connection, int? articleNo = null)
        {
            return ExecuteArticleCommand(connection, "HEAD", articleNo, 221);
        }

        public static NntpMultilineResponse Body(this INntpConnection connection, string messageId)
        {
            return ExecuteArticleCommand(connection, "BODY", messageId, 222);
        }

        public static NntpMultilineResponse Body(this INntpConnection connection, int? articleNo = null)
        {
            return ExecuteArticleCommand(connection, "BODY", articleNo, 222);
        }

        public static NntpResponse Stat(this INntpConnection connection, string messageId)
        {
            return ExecuteArticleCommand(connection, "STAT", messageId, 223);
        }

        public static NntpResponse Stat(this INntpConnection connection, int? articleNo = null)
        {
            return ExecuteArticleCommand(connection, "STAT", articleNo, 223);
        }

        private static NntpMultilineResponse ExecuteArticleCommand(INntpConnection connection, string command, string messageId, int validCode)
        {
            return connection.ExecuteMultilineCommand(string.Join(" ", command, messageId.WithBrackets()), validCode);
        }

        private static NntpMultilineResponse ExecuteArticleCommand(INntpConnection connection, string command, int? articleNo, int validCode)
        {
            var articleCommand = CommandBuilder.Build(command, builder =>
            {
                if (articleNo.HasValue) builder.AddParameter(articleNo.Value);
            });

            return connection.ExecuteMultilineCommand(articleCommand, validCode);
        }
    }

    public static class SelectionExtensions
    {
        public static NntpResponse Group(this INntpConnection connection, string group)
        {
            var command = string.Format("GROUP {0}", group);

            return connection.ExecuteCommand(command);
        }

        public static NntpMultilineResponse ListGroup(this INntpConnection connection, string group, int articleNo)
        {
            var command = string.Format("LISTGROUP {0} {1}", group, articleNo);

            return connection.ExecuteMultilineCommand(command, 211);
        }

        public static NntpMultilineResponse ListGroup(this INntpConnection connection, string group = null, Range range = null)
        {
            var command = CommandBuilder.Build("LISTGROUP", builder =>
            {
                if (string.IsNullOrWhiteSpace(group)) return;

                builder.AddParameter(group);
                if (range != null)
                {
                    builder.AddParameter(range);
                }
            });

            return connection.ExecuteMultilineCommand(command, 211);
        }

        public static NntpResponse Last(this INntpConnection connection)
        {
            return connection.ExecuteCommand("LAST");
        }

        public static NntpResponse Next(this INntpConnection connection)
        {
            return connection.ExecuteCommand("NEXT");
        }
    }

    public static class SessionExtensions
    {
        public static NntpMultilineResponse Capabilities(this INntpConnection connection, string keyword = null)
        {
            var command = CommandBuilder.Build("CAPABILITIES", builder =>
            {
                if (!string.IsNullOrWhiteSpace(keyword)) builder.AddParameter(keyword);
            });

            return connection.ExecuteMultilineCommand(command, 101);
        }

        public static NntpResponse ModeReader(this INntpConnection connection)
        {
            return connection.ExecuteCommand("MODE READER");
        }

        public static NntpResponse Quit(this INntpConnection connection)
        {
            return connection.ExecuteCommand("QUIT");
        }
    }

}