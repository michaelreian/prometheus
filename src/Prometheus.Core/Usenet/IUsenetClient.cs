using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prometheus.Core.Usenet
{
    public interface IUsenetClient : IDisposable
    {
        Task<Group> GetGroup(string groupName);
        Task<List<Overview>> GetOverviews(string groupName, long first, long last);
        Task<List<string>> GetHeaders();
    }
}