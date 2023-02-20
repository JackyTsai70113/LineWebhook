using System;
using System.Text.Json;
using BL.Service.Telegram;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Args;

namespace Website.Controllers
{
    [ApiController]
    [Route("api/webhook/telegram")]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly Logger<TelegramWebhookController> _logger;
        private readonly ITelegramService _telegramWebhookService;

        public TelegramWebhookController(Logger<TelegramWebhookController> logger, ITelegramService telegramWebhookService)
        {
            _logger = logger;
            _telegramWebhookService = telegramWebhookService;
        }

        [HttpPost]
        public IActionResult Index([FromBody] MessageEventArgs messageEventArgs)
        {
            _logger.LogDebug("messageEventArgs: {messageEventArgs}", messageEventArgs);
            try
            {
                _telegramWebhookService.UpdateWebhook(messageEventArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError("Index 發生錯誤, messageEventArgs: {messageEventArgs}, ex: {ex}", messageEventArgs, ex);
            }
            return Ok();
        }
    }
}