using System;
using System.Threading.Tasks;
using BL.Service.Telegram;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace Website.Controllers
{
    [ApiController]
    [Route("api/webhook/telegram")]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly ILogger<TelegramWebhookController> _logger;
        private readonly ITelegramService _telegramWebhookService;

        public TelegramWebhookController(ILogger<TelegramWebhookController> logger, ITelegramService telegramWebhookService)
        {
            _logger = logger;
            _telegramWebhookService = telegramWebhookService;
        }

        [HttpPost("")]
        public IActionResult Index([FromBody] dynamic update)
        {
            var s = update.ToString();
            _logger.LogDebug($"s: {s}");
            // try
            // {
            //     return Ok(await _telegramWebhookService.UpdateWebhook(update));
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError("Index 發生錯誤, update: {update}, ex: {ex}", update, ex);
            // }
            return Ok();
        }
    }
}