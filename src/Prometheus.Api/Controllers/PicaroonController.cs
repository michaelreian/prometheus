using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Prometheus.Core;
using Prometheus.Core.Picaroon;

namespace Prometheus.Api.Controllers
{
    [Route("api/v1/picaroon")]
    public class PicaroonController : Controller
    {
        private readonly IMediator mediator;
        private readonly IMemoryCache cache;

        public PicaroonController(IMediator mediator, IMemoryCache cache)
        {
            this.mediator = mediator;
            this.cache = cache;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(GetTorrentsQuery query)
        {
            var results = await this.mediator.Send(query);

            return Ok(results);
        }

        [HttpGet("detail")]
        public async Task<IActionResult> Detail(GetTorrentDetailQuery query)
        {
            var results = await this.mediator.Send(query);

            return Ok(results);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories(string proxyUrl)
        {
            var categories = await this.cache.GetOrCreateAsync($"categories-{proxyUrl}", async entry =>
            {
                return await this.mediator.Send(new GetCategoriesQuery { BaseUrl = proxyUrl });
            });
            
            return Ok(categories);
        }

        [HttpGet("proxies")]
        public async Task<IActionResult> GetProxies()
        {
            var proxies = await this.mediator.Send(new GetProxiesQuery());

            return Ok(proxies);
        }

        [HttpGet("proxy")]
        public async Task<IActionResult> GetProxy()
        {
            var uri = await this.cache.GetOrCreateAsync("proxy", async entry =>
            {
                return await this.mediator.Send(new GetProxyQuery());
            });

            return Ok(uri);
        }
    }
}