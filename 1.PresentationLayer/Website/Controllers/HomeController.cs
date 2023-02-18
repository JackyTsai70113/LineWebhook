using System;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private string version = "";
        [HttpGet("")]
        public IActionResult Test()
        {
            if (version == "")
                version = DateTime.UtcNow.AddHours(8).ToString("yyMMdd");
            return Ok($"網站正常運作中({version})，時間: {DateTime.UtcNow.AddHours(8)}");
        }
    }
}