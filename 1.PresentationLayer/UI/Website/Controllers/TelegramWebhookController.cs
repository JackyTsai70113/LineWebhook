using System;
using BL.Interfaces;
using BL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers {
    /// <summary>
    /// TelegramWebhook控制器，Line Server 的 I/O
    /// </summary>
    public class TelegramWebhookController : Controller {
        private readonly ITelegramWebhookService telegramWebhookService;
        public TelegramWebhookController() {
            this.telegramWebhookService = new TelegramWebhookService();
        }

        /// <summary>
        /// TelegramWebhook的入口，負責解讀line的訊息。
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index2() {
            try {
                string result = telegramWebhookService.Response();
                return Content("\n" + result);
            } catch (Exception ex) {
                return Content($"Index 發生錯誤，requestBody: ex: {ex}");
            }
        }

        [HttpPost]
        public IActionResult Index([FromBody] dynamic requestBody) {
            try {
                Console.WriteLine($"========== From Telegram SERVER ==========");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{requestBody}");
                Console.WriteLine($"====================");
                return Content("\n");
            } catch (Exception ex) {
                return Content($"Index 發生錯誤，requestBody: ex: {ex}");
            }
        }
    }
}