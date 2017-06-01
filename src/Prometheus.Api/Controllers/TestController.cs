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
        [Route("publish"), HttpPost]
        public IActionResult Publish(string message)
        {
            var publisher = new Publisher();

            publisher.Publish(message);

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
