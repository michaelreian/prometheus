using System;
using System.Threading.Tasks;

namespace Prometheus.Core.Usenet
{
    public interface INntpConnection : IDisposable
    {
        bool IsConnected { get; }

        Task<NntpResponse> Connect(string hostname, int port, bool useSsl);

        NntpResponse ExecuteCommand(string command);

        NntpMultilineResponse ExecuteMultilineCommand(string command, int validCode);

        void Close();
    }
}