using System;
using System.Collections.Generic;
using System.Linq;
using BL.Services;
using BL.Services.Interfaces;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Website.Services;

namespace Website.Controllers {

    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LineWebhookController : ControllerBase {
        private readonly ILogger<LineWebhookController> _logger;
        private ILineWebhookService _LineWebhookService { get; set; }

        public LineWebhookController(ILogger<LineWebhookController> logger) {
            _logger = logger;
            _LineWebhookService = new LineWebhookService(ConfigService.Line_ChannelAccessToken);
        }

        /// <summary>
        /// LineWebhook的入口，負責解讀line的訊息。
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index([FromBody] dynamic requestBody) {
            try {
                //Get  Post RawData
                string postData = requestBody.ToString();

                //處理requestModel
                ReceivedMessage receivedMessage = Utility.Parsing(postData);

                Log.Information($"========== From LINE SERVER ==========");
                Log.Information($"requestModel:");
                Log.Information($"{JsonConvert.SerializeObject(receivedMessage, Formatting.Indented)}");
                Log.Information($"====================");

                string replyToken = receivedMessage.events.FirstOrDefault().replyToken;
                List<MessageBase> messages = _LineWebhookService.GetReplyMessages(receivedMessage);
                string result = _LineWebhookService.ResponseToLineServer(replyToken, messages);

                // Add 紀錄發至LineServer的requestBody
                Log.Information($"========== TO LINE SERVER ==========");
                Log.Information($"messages:");
                Log.Information($"{JsonConvert.SerializeObject(messages, Formatting.Indented)}");
                Log.Information($"====================");
                return Content(requestBody.ToString() + "\n" + result);
            } catch (Exception ex) {
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