using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BL.Services;
using BL.Services.Interfaces;
using BL.Services.Line.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
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
        private IConfigurationRoot _configRoot;
        private readonly ILogger<LineWebhookController> _logger;
        private readonly ILineNotifyBotService _lineNotifyBotService;
        private readonly ILineWebhookService _lineWebhookService;

        public LineWebhookController(
            IConfiguration config
            , ILogger<LineWebhookController> logger
            , ILineNotifyBotService LineNotifyBotService
            , ILineWebhookService LineWebhookService) {
            // Bot = new Bot(ConfigService.Line_ChannelAccessToken);
            _configRoot = (IConfigurationRoot)config;
            _logger = logger;
            _lineNotifyBotService = LineNotifyBotService;
            _lineWebhookService = LineWebhookService;
        }

        /// <summary>
        /// LineWebhook的入口，解讀line的訊息並回覆訊息。
        /// </summary>
        /// <param name="requestBody">從line接收到的訊息字串</param>
        /// <returns>API 結果</returns>
        [HttpPost]
        [Route("index")]
        public IActionResult Index([FromBody] dynamic requestBody) {
            try {
                //處理requestModel
                ReceivedMessage receivedMessage = Utility.Parsing(requestBody.ToString());

                _logger.LogInformation($"========== From LINE SERVER ==========");
                _logger.LogInformation($"requestModel:");
                _logger.LogInformation($"{JsonSerializer.Serialize(receivedMessage)}");
                _logger.LogInformation($"====================");

                List<MessageBase> messages = _lineWebhookService.GetReplyMessages(receivedMessage.events[0]);

                // Add 紀錄發至LineServer的requestBody
                _logger.LogInformation($"========== TO LINE SERVER ==========");
                _logger.LogInformation($"messages:");
                _logger.LogInformation($"{JsonSerializer.Serialize(messages)}");
                _logger.LogInformation($"====================");

                string replyToken = receivedMessage.events[0].replyToken;
                try {
                    var Bot = new Bot(_configRoot.GetSection("Line").GetSection("NotifyBearerToken_Group").Value);
                    string result = Bot.ReplyMessage(replyToken, messages);
                    return Content(requestBody + "\n" + result);
                } catch (Exception ex) {
                    if (ex.InnerException is WebException) {
                        int responseStartIndex = ex.ToString().IndexOf("Response") + "Response:".Count();
                        int responseEndIndex = ex.ToString().IndexOf("Endpoint");
                        string responseStr = ex.ToString()[responseStartIndex..responseEndIndex].Trim();
                        LineHttpPostException response = JsonSerializer.Deserialize<LineHttpPostException>(responseStr);
                        _logger.LogError(
                            $"LineWebhookService.ResponseToLineServer 錯誤, replyToken: {replyToken},\n" +
                            $"messages: {JsonSerializer.Serialize(messages)},\n" +
                            $"response: {JsonSerializer.Serialize(response)}");
                        _lineNotifyBotService.PushMessage_Jacky($"message: {response.message},\n" +
                            $"details: {JsonSerializer.Serialize(response.details)}");
                        return Content($"[Index] JLineBot 無法發送，requestBody: {requestBody}, ex: {ex}");
                    }
                    throw;
                }
            } catch (Exception ex) {
                _logger.LogError($"[Index] requestBody: {requestBody}, ex: {ex}");
                return Content($"Index 發生錯誤，requestBody: {requestBody}, ex: {ex}");
            }
        }

        [HttpPost]
        [Route("TestForRequest")]
        public IActionResult TestForRequest(dynamic requestBody) {
            try {
                //處理requestModel
                ReceivedMessage receivedMessage = Utility.Parsing(requestBody.ToString());

                StringBuilder sb = new StringBuilder();
                sb.Append($"========== From LINE SERVER ==========\n");
                sb.Append($"requestModel:\n");
                sb.Append($"{JsonSerializer.Serialize(receivedMessage)}\n");
                sb.Append($"====================\n");

                List<MessageBase> messages = _lineWebhookService.GetReplyMessages(receivedMessage.events[0]);

                // Add 紀錄發至LineServer的requestBody
                sb.Append($"========== TO LINE SERVER ==========\n");
                sb.Append($"messages:\n");
                sb.Append($"{JsonSerializer.Serialize(messages)}\n");
                sb.Append($"====================");
                return Ok(sb.ToString());
            } catch (Exception ex) {
                _logger.LogError($"[Index] requestBody: {requestBody}, ex: {ex}");
                return Content($"Index 發生錯誤，requestBody: {requestBody}, ex: {ex}");
            }
        }

        [HttpPost]
        [Route("notify")]
        public IActionResult Notify(dynamic requestBody) {
            string requestBodyStr = requestBody.ToString();
            return Ok(requestBodyStr);
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test() {
            string str = "";
            foreach (var provider in _configRoot.Providers.ToList()) {
                str += provider.ToString() + "\n";
            }

            _logger.LogInformation("Hello, {Name}!", Environment.UserName);
            _logger.LogInformation("Info!");
            _logger.LogWarning("Warning!");
            _logger.LogTrace("Trace!");
            _logger.LogDebug("Debug");
            _logger.LogCritical("Critical");
            _logger.LogError("Error");
            _lineNotifyBotService.PushMessage_Jacky("Test line notify");
            return Ok("test: " + str);
        }
    }
}