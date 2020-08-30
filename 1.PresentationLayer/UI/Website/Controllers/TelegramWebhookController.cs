using BL.Services;
using BL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Website.Controllers {

    /// <summary>
    /// TelegramWebhook控制器，Line Server 的 I/O
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TelegramWebhookController : ControllerBase {
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
        [Route("index2")]
        public IActionResult Index2() {
            try {
                string result = telegramWebhookService.Response();
                return Content(result);
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

        [HttpPost]
        [Route("notify")]
        public ActionResult Notify(NotifyModel notifyModel) {
            if (string.IsNullOrWhiteSpace(notifyModel.message)) {
                return BadRequest();
            }
            Task.Run(() => {
                for (int i = 0; i < 5; i++) {
                    DateTime now = DateTime.Now;
                    telegramWebhookService.NotifyByMessage(now.ToString());
                    Thread.Sleep(20000);
                }
            });
            telegramWebhookService.NotifyByMessage("Over");
            return Ok();
        }

        [HttpGet]
        [Route("test")]
        public ActionResult Test() {
            telegramWebhookService.Test();
            return Ok();
        }
    }

    public class NotifyModel {
        public string message { get; set; }
    }
}