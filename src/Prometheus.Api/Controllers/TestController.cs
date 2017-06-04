using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Prometheus.Core;

namespace Prometheus.Api.Controllers
{
    public class TestController : Controller
    {
        private readonly IBus bus;

        public TestController(IBus bus)
        {
            this.bus = bus;
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
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                Online = true
            });
        }

        [Route(""), HttpGet]
        [SwaggerIgnore]
        public IActionResult Index()
        {
            return this.HealthCheck();
        }
    }
}
