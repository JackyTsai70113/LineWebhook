using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Website.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : ControllerBase {
        private readonly IConfiguration _config;

        public ConfigController(IConfiguration config) {
            _config = config;
        }

        public IActionResult Index() {
            //return Ok(_config.GetSection("Line").GetSection("NotifyBearerToken_Group").Value);
            return Ok(_config["Line:NotifyBearerToken_Group"]);
        }
    }
}