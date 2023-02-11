using System;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult Test()
        {
            return Ok($"網站正常運作中，時間: {DateTime.UtcNow.AddHours(8)}");
        }
    }
}