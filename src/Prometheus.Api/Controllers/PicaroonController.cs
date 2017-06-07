using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Core;
using Prometheus.Core.Picaroon;

namespace Prometheus.Api.Controllers
{
    public class PicaroonController : Controller
    {
        private readonly IMediator mediator;

        public PicaroonController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("api/v1/torrents/browse")]
        public async Task<IActionResult> Browse(string baseUrl = "https://unblockedbay.info", string categoryID = null)
        {
            var results = await this.mediator.Send(new GetTorrentsQuery { BaseUrl = baseUrl, CategoryID = categoryID });

            return Ok(results);
        }

        [HttpGet("api/v1/torrents/categories")]
        public async Task<IActionResult> GetCategories(string baseUrl = "https://unblockedbay.info")
        {
            var categories = await this.mediator.Send(new GetCategoriesQuery { BaseUrl = baseUrl });

            return Ok(categories);
        }

        [HttpGet("api/v1/torrents/proxies")]
        public async Task<IActionResult> GetProxies()
        {
            var proxies = await this.mediator.Send(new GetProxiesQuery());

            return Ok(proxies);
        }

        [HttpGet("api/v1/torrents/proxies/valid")]
        public async Task<IActionResult> GetValidProxy()
        {
            var uri = await this.mediator.Send(new GetProxyQuery());

            return Ok(uri);
        }
    }
}