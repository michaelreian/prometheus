using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus.Core.Usenet
{
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
}