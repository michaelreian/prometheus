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
            this.bus.Publish(new HelloWorldEvent
            {
                Name = "Michael"
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
