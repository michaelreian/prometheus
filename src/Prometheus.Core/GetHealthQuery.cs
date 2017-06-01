using System.Threading.Tasks;
using MediatR;

namespace Prometheus.Core
{
    public class GetHealthQuery : IRequest<bool>
    {
        
    }

    public class GetHealthQueryHandler : IAsyncRequestHandler<GetHealthQuery, bool>
    {
        public async Task<bool> Handle(GetHealthQuery message)
        {
            return await Task.FromResult(true);
        }
    }
}