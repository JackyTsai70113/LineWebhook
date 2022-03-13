using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BL.Services;
using BL.Services.Interfaces;
using BL.Services.Line.Interfaces;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Domain.Utilities;
using System.Text;

namespace Website.Controllers {

    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LineWebhookController : ControllerBase {
        private readonly Bot Bot;
        private readonly ILogger<LineWebhookController> logger;
        private readonly ILineNotifyBotService _lineNotifyBotService;
        private readonly ILineWebhookService _lineWebhookService;

        public LineWebhookController(ILogger<LineWebhookController> logger
            , ILineNotifyBotService LineNotifyBotService
            , ILineWebhookService LineWebhookService
            ) {
            Bot = new Bot(ConfigService.Line_ChannelAccessToken);
            this.logger = logger;
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

                logger.LogInformation($"========== From LINE SERVER ==========");
                logger.LogInformation($"requestModel:");
                logger.LogInformation($"{JsonUtility.Serialize(receivedMessage, isIndented: true)}");
                logger.LogInformation($"====================");

                List<MessageBase> messages = _lineWebhookService.GetReplyMessages(receivedMessage.events[0]);

                // Add 紀錄發至LineServer的requestBody
                logger.LogInformation($"========== TO LINE SERVER ==========");
                logger.LogInformation($"messages:");
                logger.LogInformation($"{JsonUtility.Serialize(messages, isIndented: true)}");
                logger.LogInformation($"====================");

                string replyToken = receivedMessage.events[0].replyToken;
                try {
                    string result = Bot.ReplyMessage(replyToken, messages);
                    return Content(requestBody + "\n" + result);
                } catch (Exception ex) {
                    if (ex.InnerException is WebException) {
                        int responseStartIndex = ex.ToString().IndexOf("Response") + "Response:".Count();
                        int responseEndIndex = ex.ToString().IndexOf("Endpoint");
                        string responseStr = ex.ToString()[responseStartIndex..responseEndIndex].Trim();
                        LineHttpPostException response = JsonUtility.Deserialize<LineHttpPostException>(responseStr);
                        logger.LogError(
                            $"LineWebhookService.ResponseToLineServer 錯誤, replyToken: {replyToken},\n" +
                            $"messages: {JsonUtility.Serialize(messages, isIndented: true)},\n" +
                            $"response: {JsonUtility.Serialize(response, isIndented: true)}");
                        _lineNotifyBotService.PushMessage_Jacky($"message: {response.message},\n" +
                            $"details: {JsonUtility.Serialize(response.details, isIndented: true)}");
                        return Content($"[Index] JLineBot 無法發送，requestBody: {requestBody}, ex: {ex}");
                    }
                    throw;
                }
            } catch (Exception ex) {
                logger.LogError($"[Index] requestBody: {requestBody}, ex: {ex}");
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
                sb.Append($"{JsonUtility.Serialize(receivedMessage, isIndented: true, isIgnorezNullValue: true)}\n");
                sb.Append($"====================\n");

                List<MessageBase> messages = _lineWebhookService.GetReplyMessages(receivedMessage.events[0]);

                // Add 紀錄發至LineServer的requestBody
                sb.Append($"========== TO LINE SERVER ==========\n");
                sb.Append($"messages:\n");
                sb.Append($"{JsonUtility.Serialize(messages, isIndented: true, isIgnorezNullValue: true)}\n");
                sb.Append($"====================");
                return Ok(sb.ToString());
            } catch (Exception ex) {
                logger.LogError($"[Index] requestBody: {requestBody}, ex: {ex}");
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
            logger.LogInformation("Hello, {Name}!", Environment.UserName);
            logger.LogInformation("Info!");
            logger.LogWarning("Warning!");
            logger.LogTrace("Trace!");
            logger.LogDebug("Debug");
            logger.LogCritical("Critical");
            logger.LogError("Error");
            return Ok("test");
        }
    }
}