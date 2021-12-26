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
using Newtonsoft.Json;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

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
                Log.Information($"{JsonConvert.SerializeObject(receivedMessage, Formatting.Indented)}");
                Log.Information($"====================");

                List<MessageBase> messages = _lineWebhookService.GetReplyMessages(receivedMessage);

                // Add 紀錄發至LineServer的requestBody
                Log.Information($"========== TO LINE SERVER ==========");
                Log.Information($"messages:");
                Log.Information($"{JsonConvert.SerializeObject(messages, Formatting.Indented)}");
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
                        LineHttpPostException response =
                            JsonConvert.DeserializeObject<LineHttpPostException>(responseStr);
                        Log.Error(
                            $"LineWebhookService.ResponseToLineServer 錯誤, replyToken: {replyToken},\n" +
                            $"messages: {JsonConvert.SerializeObject(messages, Formatting.Indented)},\n" +
                            $"response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
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
        [Route("index2")]
        public IActionResult Index2(dynamic requestBody) {
//             try {
//                 //處理requestModel
//                 ReceivedMessage receivedMessage = Utility.Parsing(requestBody.ToString());
// 
//                 Log.Information($"========== From LINE SERVER ==========");
//                 Log.Information($"requestModel:");
//                 Log.Information($"{JsonConvert.SerializeObject(receivedMessage, Formatting.Indented)}");
//                 Log.Information($"====================");
// 
//                 List<MessageBase> messages = _lineWebhookService.GetReplyMessages(receivedMessage);
// 
//                 // Add 紀錄發至LineServer的requestBody
//                 Log.Information($"========== TO LINE SERVER ==========");
//                 Log.Information($"messages:");
//                 Log.Information($"{JsonConvert.SerializeObject(messages, Formatting.Indented)}");
//                 Log.Information($"====================");
// 
//                 string replyToken = receivedMessage.events.FirstOrDefault().replyToken;
//                 try {
//                     string result = lineBot.ReplyMessage(replyToken, messages);
//                     return Content(requestBody + "\n" + result);
//                 } catch (Exception ex) {
//                     if (ex.InnerException is WebException) {
//                         int responseStartIndex = ex.ToString().IndexOf("Response") + "Response:".Count();
//                         int responseEndIndex = ex.ToString().IndexOf("Endpoint");
//                         string responseStr = ex.ToString()[responseStartIndex..responseEndIndex].Trim();
//                         LineHttpPostException response =
//                             JsonConvert.DeserializeObject<LineHttpPostException>(responseStr);
//                         Log.Error(
//                             $"LineWebhookService.ResponseToLineServer 錯誤, replyToken: {replyToken},\n" +
//                             $"messages: {JsonConvert.SerializeObject(messages, Formatting.Indented)},\n" +
//                             $"response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
//                         // _lineNotifyBotService.PushMessage_Jacky($"message: {response.message}, " +
//                         //     $"details: {JsonConvert.SerializeObject(response.details)}");
//                         return Content($"[Index] JLineBot 無法發送，requestBody: {requestBody}, ex: {ex}");
//                     }
//                     throw;
//                 }
//             } catch (Exception ex) {
//                 Log.Error($"[Index] requestBody: {requestBody}, ex: {ex}");
//                 return Content($"Index 發生錯誤，requestBody: {requestBody}, ex: {ex}");
//             }
            return Content($"Index 發生錯誤，requestBody: {requestBody}");
        }

        [HttpPost]
        [Route("notify")]
        public IActionResult Notify(dynamic requestBody) {
            string requestBodyStr = requestBody.ToString();
            // _logger.LogInformation("================================");
            // _logger.LogInformation(requestBodyStr);
            return Ok();
        }

        [HttpGet]
        [Route("test")]
        public IActionResult Test() {
            return Ok("test");
        }
    }
}