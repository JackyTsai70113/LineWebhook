using BL.Services.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers {
    public class RedisController : Controller {
        private readonly ICacheService _redisCacheService;

        public RedisController(ICacheService redisCacheService) {
            _redisCacheService = redisCacheService;
        }
        [HttpGet]
        public IActionResult ViewDB() {
            var dd = string.Join("\n", _redisCacheService.GetKeys("*"));
            ViewData["db"] = dd;
            return View();
        }

        [HttpPost]
        public string GetKeys(string para) {
            var data = string.Join("\n", _redisCacheService.GetKeys(para));
            return data;
        }
    }
}