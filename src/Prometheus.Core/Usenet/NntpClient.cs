using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prometheus.Core.Usenet;

namespace Prometheus.Core.Usenet
{
    public class NntpClient
    {
        private readonly INntpConnection connection;

        public NntpClient() : this((INntpConnection) new NntpConnection()) { }

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
            var userResponse = AuthExtensions.AuthInfoUser(connection, username);

            if (userResponse.Code == 281) return true;

            if (userResponse.Code != 381 || password == null) return false;

            var passResponse = AuthExtensions.AuthInfoPass(connection, password);

            return passResponse.Code == 281;
        }

        public bool SetReaderMode()
        {
            var response = SessionExtensions.ModeReader(connection);
            if (response.Code == 502)
            {
                connection.Close();
                return false;
            }

            return response.Code == 200 || response.Code == 201;
        }

        public Group SelectGroup(string group)
        {
            var response = SelectionExtensions.Group(this.connection, group);
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
            var response = RetrievalExtensions.Article(connection, messageId);
            if (response.Lines == null) return null;

            var articleLines = response.Lines.ToList();

            var headers = articleLines.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).Select(Header.Create);
            var body = articleLines.SkipWhile(x => !string.IsNullOrWhiteSpace(x)).Skip(1).ToList();

            return new Article(headers, body);
        }

        public IEnumerable<Header> RetrieveHeader(string messageId)
        {
            var response = RetrievalExtensions.Head(connection, messageId);
            if (response.Lines == null) return Enumerable.Empty<Header>();

            return response.Lines.Select(Header.Create);
        }
    }
}