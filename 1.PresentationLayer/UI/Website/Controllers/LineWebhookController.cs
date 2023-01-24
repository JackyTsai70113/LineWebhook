using System;
using BL.Service.Interface;
using BL.Service.Line.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using isRock.LineBot;
using Microsoft.Extensions.Configuration;

namespace Website.Controllers {

    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LineWebhookController : ControllerBase {
        private IConfiguration _config;
        private readonly ILogger<LineWebhookController> _logger;
        private readonly ILineBotService _lineNotifyBotService;
        private readonly ILineWebhookService _lineWebhookService;

        public LineWebhookController(
            IConfiguration config
            , ILogger<LineWebhookController> logger
            , ILineBotService LineNotifyBotService
            , ILineWebhookService LineWebhookService) {
            // Bot = new Bot(ConfigService.Line_ChannelAccessToken);
            _config = config;
            _logger = logger;
            _lineNotifyBotService = LineNotifyBotService;
            _lineWebhookService = LineWebhookService;
        }

        /// <summary>
        /// LineWebhook的入口，解讀line的訊息並回覆訊息。
        /// </summary>
        /// <param name="receivedMessage">從line接收到的訊息字串</param>
        /// <returns>API 結果</returns>
        [HttpPost]
        [Route("index")]
        public IActionResult Index(ReceivedMessage receivedMessage) {
            var req = JsonSerializer.Serialize(receivedMessage);
            try {
                //處理requestModel
                if (receivedMessage.events.Count == 0) return Ok();
                _logger.LogDebug($"request: {req}");

                var messages = _lineWebhookService.GetReplyMessages(receivedMessage.events[0]);

                // Add 紀錄發至LineServer的requestBody
                _logger.LogDebug($"response: {JsonSerializer.Serialize(messages)}");

                _lineNotifyBotService.ReplyMessage(receivedMessage.events[0].replyToken, messages);
                return Ok();
            } catch (Exception ex) {
                _logger.LogError($"[Index] request: {req}, ex: {ex}");
                return Content($"error, request: {req}, ex: {ex}");
            }
        }

        [HttpPost]
        [Route("TestForResponse")]
        public IActionResult TestForResponse(ReceivedMessage receivedMessage) {
            var req = JsonSerializer.Serialize(receivedMessage);
            try {
                var messages = _lineWebhookService.GetReplyMessages(receivedMessage.events[0]);
                return Ok($"request: {JsonSerializer.Serialize(receivedMessage)}\nresponse: {JsonSerializer.Serialize(messages)}");
            } catch (Exception ex) {
                _logger.LogError($"[TestForResponse] req: {req}, ex: {ex}");
                return Content($"error, req: {req}, ex: {ex}");
            }
        }

        [HttpPost]
        [Route("testForRequest")]
        public IActionResult TestForRequest(ReceivedMessage requestBody) {
            return Ok(requestBody);
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test() {
            _logger.LogInformation("Info!");
            _logger.LogWarning("Warning!");
            _logger.LogTrace("Trace!");
            _logger.LogDebug("Debug");
            _logger.LogCritical("Critical");
            _logger.LogError("Error");
            bool res = _lineNotifyBotService.Notify_Jacky("Test line notify");
            return Ok($"test result: {res}");
        }
    }
}