using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Prometheus.Core;

namespace Prometheus.Api.Controllers
{
    public class TestController : Controller
    {
        private readonly IBus bus;
        private readonly IMediator mediator;

        public TestController(IBus bus, IMediator mediator)
        {
            this.bus = bus;
            this.mediator = mediator;
        }

        [Route("hello"), HttpPost]
        public IActionResult Hello(string name)
        {
            this.bus.Send(new HelloWorldEvent
            {
                Name = name
            });

            return Ok();
        }

        [Route("ping"), HttpPost]
        public IActionResult Ping()
        {
            this.bus.Send(new PingEvent
            {
                Timestamp = DateTime.UtcNow
            });

            return Ok();
        }

        [Route("do"), HttpPost]
        public IActionResult Do(string something)
        {
            this.bus.Send(new DoSomethingCommand
            {
                Something = something
            }); 

            return Ok();
        }

        [Route("health/check"), HttpGet]
        public async Task<IActionResult> HealthCheck()
        {
            return Ok(await this.mediator.Send(new GetHealthQuery()));
        }

        [Route(""), HttpGet]
        [SwaggerIgnore]
        public async Task<IActionResult> Index()
        {
            return await this.HealthCheck();
        }
    }
}
