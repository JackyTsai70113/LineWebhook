using BL.Service.Telegram;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Website.Controllers
{
    [ApiController]
    [Route("api/webhook/telegram")]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly ILogger<TelegramWebhookController> Logger;
        private readonly ITelegramService _telegramWebhookService;

        public TelegramWebhookController(ILogger<TelegramWebhookController> logger, ITelegramService telegramWebhookService)
        {
            Logger = logger;
            _telegramWebhookService = telegramWebhookService;
        }

        [HttpPost]
        [Route("")]
        public IActionResult Index([FromBody] Update update)
        {
            // var s = update.ToString();
            // _logger.LogDebug($"s: {s}");
            // try
            // {
            // Update update = JsonSerializer.Deserialize<Update>(d.ToString());

            Logger.LogDebug("request: {req}", update);

            Message message = _telegramWebhookService.HandleUpdate(update);

            // Add 紀錄發至LineServer的requestBody
            Logger.LogDebug("response: {messages}", message);
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
