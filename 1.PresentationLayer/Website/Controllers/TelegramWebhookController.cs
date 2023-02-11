using System;
using System.Threading;
using System.Threading.Tasks;
using BL.Service;
using BL.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly ITelegramWebhookService TelegramWebhookService;

        public TelegramWebhookController()
        {
            TelegramWebhookService = new TelegramWebhookService();
        }

        /// <summary>
        /// TelegramWebhook的入口，負責解讀line的訊息。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("index2")]
        public IActionResult Index2()
        {
            try
            {
                string result = TelegramWebhookService.Response();
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content($"Index 發生錯誤, requestBody: ex: {ex}");
            }
        }

        [HttpPost]
        public IActionResult Index([FromBody] dynamic requestBody)
        {
            try
            {
                Console.WriteLine($"========== From Telegram SERVER ==========");
                Console.WriteLine($"requestBody:");
                Console.WriteLine($"{requestBody}");
                Console.WriteLine($"====================");
                return Content("\n");
            }
            catch (Exception ex)
            {
                return Content($"Index 發生錯誤，requestBody: ex: {ex}");
            }
        }

        [HttpPost]
        [Route("notify")]
        public ActionResult Notify(NotifyModel notifyModel)
        {
            if (string.IsNullOrWhiteSpace(notifyModel.Message))
            {
                return BadRequest();
            }
            Task.Run(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    DateTime now = DateTime.Now;
                    TelegramWebhookService.NotifyByMessage("現在時間： " + now.ToString());
                    Thread.Sleep(20000);
                }
            });
            return Ok();
        }

        [HttpGet]
        [Route("test")]
        public ActionResult Test()
        {
            TelegramWebhookService.Test();
            return Ok();
        }
    }

    public class NotifyModel
    {
        public string Message { get; set; }
    }
}