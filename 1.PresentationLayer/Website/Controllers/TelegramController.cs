using System;
using System.Threading;
using System.Threading.Tasks;
using BL.Service.Telegram;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramService _telegramWebhookService;

        public TelegramController(ITelegramService telegramWebhookService)
        {
            _telegramWebhookService = telegramWebhookService;
        }

        /// <summary>
        /// get me
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get_me")]
        public IActionResult GetMe()
        {
            try
            {
                var user = _telegramWebhookService.GetMe();
                return Ok(user);
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
                return Content($"Index 發生錯誤, requestBody: ex: {ex}");
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
                    _telegramWebhookService.NotifyByMessage("現在時間： " + now.ToString());
                    Thread.Sleep(1000);
                }
            });
            return Ok();
        }

        [HttpGet]
        [Route("send_dice")]
        public ActionResult SendDice()
        {
            var msgs = _telegramWebhookService.SendDice();
            return Ok(msgs);
        }
    }

    public class NotifyModel
    {
        public string Message { get; set; }
    }
}