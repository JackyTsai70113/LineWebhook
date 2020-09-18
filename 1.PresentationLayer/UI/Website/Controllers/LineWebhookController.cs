using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BL.Services;
using BL.Services.Interfaces;
using BL.Services.Line;
using BL.Services.Line.Interfaces;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace Website.Controllers {

    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LineWebhookController : ControllerBase {
        private readonly ILogger<LineWebhookController> _logger;
        private ILineNotifyBotService _lineNotifyBotService { get; set; }
        private ILineWebhookService _lineWebhookService { get; set; }

        public LineWebhookController(ILogger<LineWebhookController> logger
            , ILineNotifyBotService LineNotifyBotService
            , ILineWebhookService LineWebhookService) {
            _logger = logger;
            _lineNotifyBotService = LineNotifyBotService;
            _lineWebhookService = LineWebhookService;
        }

        /// <summary>
        /// LineWebhook的入口，負責解讀line的訊息。
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index([FromBody] dynamic requestBody) {
            string replyToken = string.Empty;
            List<MessageBase> messages = null;
            try {
                //處理requestModel
                ReceivedMessage receivedMessage = Utility.Parsing(requestBody.ToString());

                Log.Information($"========== From LINE SERVER ==========");
                Log.Information($"requestModel:");
                Log.Information($"{JsonConvert.SerializeObject(receivedMessage, Formatting.Indented)}");
                Log.Information($"====================");

                messages = _lineWebhookService.GetReplyMessages(receivedMessage);

                // Add 紀錄發至LineServer的requestBody
                Log.Information($"========== TO LINE SERVER ==========");
                Log.Information($"messages:");
                Log.Information($"{JsonConvert.SerializeObject(messages, Formatting.Indented)}");
                Log.Information($"====================");

                replyToken = receivedMessage.events.FirstOrDefault().replyToken;
                string result = _lineWebhookService.ResponseToLineServer(replyToken, messages);
                return Content(requestBody.ToString() + "\n" + result);
            } catch (Exception ex) {
                if (ex.InnerException is WebException) {
                    int responseStartIndex = ex.ToString().IndexOf("Response") + "Response:".Count();
                    int responseEndIndex = ex.ToString().IndexOf("Endpoint");
                    string responseStr = ex.ToString()[responseStartIndex..responseEndIndex].Trim();
                    LineHttpPostExceptionResponse response =
                        JsonConvert.DeserializeObject<LineHttpPostExceptionResponse>(responseStr);
                    Log.Error(
                        $"LineWebhookService.ResponseToLineServer 錯誤, replyToken: {replyToken},\n" +
                        $"messages: {JsonConvert.SerializeObject(messages, Formatting.Indented)},\n" +
                        $"response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
                    _lineNotifyBotService.PushMessage_Jacky($"message: {response.message}, " +
                        $"details: {JsonConvert.SerializeObject(response.details)}");
                }
                return Content($"Index 發生錯誤，requestBody: {requestBody}, ex: {ex}");
            }
        }

        [HttpPost]
        [Route("notify")]
        public IActionResult Notify(dynamic requestBody) {
            string requestBodyStr = requestBody.ToString();
            _logger.LogInformation("================================");
            _logger.LogInformation(requestBodyStr);
            return Ok();
        }
    }
}