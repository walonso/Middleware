using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace webApiMiddleware.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class ConfigurationController : Controller
    {
        private readonly IClientConfiguration clientConfiguration;

        public ConfigurationController(IClientConfiguration clientConfiguration)
        {
            this.clientConfiguration = clientConfiguration;
        }

        [HttpGet]
        public ActionResult<ClientConfiguration> GetConfigurationController()
        {
            return new ClientConfiguration
            {
                ClientName = clientConfiguration.ClientName,
                InvokedDateTime = clientConfiguration.InvokedDateTime
            };
        }
    }
}