using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;

namespace Prometheus.Core.Picaroon
{
    public class GetProxiesQuery : IRequest<List<Proxy>>
    {
    }


    public class Proxy
    {
        public string Domain { get; set; }
        public decimal Speed { get; set; }
        public bool Secure { get; set; }
        public string Country { get; set; }
        public bool Probed { get; set; }
    }

    public class GetProxiesQueryHandler : IAsyncRequestHandler<GetProxiesQuery, List<Proxy>>
    {
        private static readonly string THE_PIRATE_BAY_PROXY_LIST_URL = "https://thepiratebay-proxylist.org/api/v1/proxies";

        private readonly IMediator mediator;

        public GetProxiesQueryHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<List<Proxy>> Handle(GetProxiesQuery message)
        {
            using (var restClient = new RestClient())
            {
                var response = await restClient.Get<Result>(THE_PIRATE_BAY_PROXY_LIST_URL);

                response.EnsureSuccessStatusCode();

                return await response.GetData(x => x.Proxies);
            }
        }

        private class Result
        {
            public List<Proxy> Proxies { get; set; }
        }
    }
}