using System;
using Microsoft.AspNetCore.Mvc;
using BL.Interfaces;
using BL.Services;
using Microsoft.Extensions.Logging;

namespace Website.Controllers {

    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    public class LineWebhookController : Controller {
        private readonly ILogger<HomeController> _logger;
        private ILineWebhookService LineWebhookService { get; set; }

        public LineWebhookController(ILogger<HomeController> logger) {
            _logger = logger;
            LineWebhookService = new LineWebhookService();
        }

        /// <summary>
        /// LINE Webhook的入口，負責解讀line的訊息。
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index([FromBody] dynamic requestBody) {
            try {
                string result = LineWebhookService.Response(requestBody);
                return Content(requestBody.ToString() + "\n" + result);
            } catch (Exception ex) {
                return Content($"Index 發生錯誤，requestBody: {requestBody}, ex: {ex}");
            }
        }
    }
}