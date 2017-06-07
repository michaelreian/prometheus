using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace Prometheus.Core.Picaroon
{
    public class GetProxyQuery : IRequest<Uri>
    {
    }

    public class GetProxyQueryHandler : IAsyncRequestHandler<GetProxyQuery, Uri>
    {
        private readonly IMediator mediator;
        private readonly ILogger logger;

        public GetProxyQueryHandler(IMediator mediator, ILogger logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<Uri> Handle(GetProxyQuery message)
        {
            var proxies = await this.mediator.Send(new GetProxiesQuery());

            foreach (var proxy in proxies.OrderBy(p => p.Speed))
            {
                var builder = new UriBuilder(proxy.Secure ? "https" : "http", proxy.Domain);

                var uri = builder.Uri;

                if (await this.Check(uri))
                {
                    return uri;
                }
            }

            return null;
        }

        private async Task<bool> Check(Uri uri)
        {
            try
            {
                using (var restClient = new RestClient(uri))
                {
                    var response = await restClient.Get<string>("");

                    response.EnsureSuccessStatusCode();
                }

                return true;
            }
            catch (Exception exception)
            {
                this.logger.Warning(exception, "There was an error checking {Uri}", uri);
            }
            return false;
        }


    }
}