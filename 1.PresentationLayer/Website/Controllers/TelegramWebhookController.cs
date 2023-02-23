using System;
using BL.Service.Telegram;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        public IActionResult Index([FromBody] Update update)
        {
            // var s = update.ToString();
            // _logger.LogDebug($"s: {s}");
            // try
            // {
            // Update update = JsonSerializer.Deserialize<Update>(d.ToString());
            _telegramWebhookService.HandleUpdate(update);
            return Ok();
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError("Index 發生錯誤, update: {update}, ex: {ex}", update, ex);
            // }
            // return Ok();
        }
    }
}
