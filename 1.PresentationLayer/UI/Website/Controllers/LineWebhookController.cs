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
using Serilog;
using Core.Domain.Utilities;
using System.Text;

namespace Website.Controllers {

    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LineWebhookController : ControllerBase {
        private readonly Bot lineBot;

        private readonly ILogger<LineWebhookController> _logger;
        private readonly ILineNotifyBotService _lineNotifyBotService;
        private readonly ILineWebhookService _lineWebhookService;

        public LineWebhookController(ILogger<LineWebhookController> logger
            , ILineNotifyBotService LineNotifyBotService
            , ILineWebhookService LineWebhookService
            ) {
            lineBot = new Bot(ConfigService.Line_ChannelAccessToken);
            _logger = logger;
            _lineNotifyBotService = LineNotifyBotService;
            _lineWebhookService = LineWebhookService;
        }

        /// <summary>
        /// LineWebhook的入口，解讀line的訊息並回覆訊息。
        /// </summary>
        /// <param name="requestBodyStr">從line接收到的訊息字串</param>
        /// <returns>API 結果</returns>
        [HttpPost]
        [Route("index")]
        public IActionResult Index([FromBody] dynamic requestBody) {
            try {
                //處理requestModel
                ReceivedMessage receivedMessage = Utility.Parsing(requestBody.ToString());

                Log.Information($"========== From LINE SERVER ==========");
                Log.Information($"requestModel:");
                Log.Information($"{JsonUtility.Serialize(receivedMessage, isIndented:true)}");
                Log.Information($"====================");

                List<MessageBase> messages = _lineWebhookService.GetReplyMessages(receivedMessage);

                // Add 紀錄發至LineServer的requestBody
                Log.Information($"========== TO LINE SERVER ==========");
                Log.Information($"messages:");
                Log.Information($"{JsonUtility.Serialize(messages, isIndented:true)}");
                Log.Information($"====================");

                string replyToken = receivedMessage.events.FirstOrDefault().replyToken;
                try {
                    string result = lineBot.ReplyMessage(replyToken, messages);
                    return Content(requestBody + "\n" + result);
                } catch (Exception ex) {
                    if (ex.InnerException is WebException) {
                        int responseStartIndex = ex.ToString().IndexOf("Response") + "Response:".Count();
                        int responseEndIndex = ex.ToString().IndexOf("Endpoint");
                        string responseStr = ex.ToString()[responseStartIndex..responseEndIndex].Trim();
                        LineHttpPostException response = JsonUtility.Deserialize<LineHttpPostException>(responseStr);
                        Log.Error(
                            $"LineWebhookService.ResponseToLineServer 錯誤, replyToken: {replyToken},\n" +
                            $"messages: {JsonUtility.Serialize(messages, isIndented:true)},\n" +
                            $"response: {JsonUtility.Serialize(response, isIndented:true)}");
                        // _lineNotifyBotService.PushMessage_Jacky($"message: {response.message}, " +
                        //     $"details: {JsonConvert.SerializeObject(response.details)}");
                        return Content($"[Index] JLineBot 無法發送，requestBody: {requestBody}, ex: {ex}");
                    }
                    throw;
                }
            } catch (Exception ex) {
                Log.Error($"[Index] requestBody: {requestBody}, ex: {ex}");
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
                sb.Append($"========== From LINE SERVER ==========");
                sb.Append($"requestModel:");
                sb.Append($"{JsonUtility.Serialize(receivedMessage, isIndented:true, isIgnorezNullValue:true)}");
                sb.Append($"====================");

                List<MessageBase> messages = _lineWebhookService.GetReplyMessages(receivedMessage);

                // Add 紀錄發至LineServer的requestBody
                sb.Append($"========== TO LINE SERVER ==========");
                sb.Append($"messages:");
                sb.Append($"{JsonUtility.Serialize(messages, isIndented:true, isIgnorezNullValue:true)}");
                sb.Append($"====================");
                return Ok(sb.ToString());
            } catch (Exception ex) {
                Log.Error($"[Index] requestBody: {requestBody}, ex: {ex}");
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
            _lineNotifyBotService.PushMessage_Jacky("test");
            return Ok("test");
        }
    }
}