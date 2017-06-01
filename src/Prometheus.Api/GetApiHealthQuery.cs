using System.Threading.Tasks;
using MediatR;
using Prometheus.Core;

namespace Prometheus.Api
{
    public class GetApiHealthQuery : IRequest<bool>
    {
        
    }

    public class GetApiHealthQueryHandler : IAsyncRequestHandler<GetApiHealthQuery, bool>
    {
        public async Task<bool> Handle(GetApiHealthQuery message)
        {
            return await Task.FromResult(true);
        }
    }
}