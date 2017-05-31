using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Core;

namespace Prometheus.Api.Controllers
{
    public class TestController : Controller
    {
        [Route("publish"), HttpPost]
        public void Publish(string message)
        {
            var publisher = new Publisher();

            publisher.Publish(message);
        }
    }
}
