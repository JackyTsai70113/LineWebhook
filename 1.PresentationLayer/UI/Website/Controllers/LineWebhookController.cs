using BL.Services;
using BL.Services.Interfaces;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _LineWebhookService = new LineWebhookService(ConfigService.LineChannelAccessToken);
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

                Console.WriteLine($"========== From LINE SERVER ==========");
                Console.WriteLine($"requestModel:");
                Console.WriteLine($"{JsonConvert.SerializeObject(receivedMessage, Formatting.Indented)}");
                Console.WriteLine($"====================");

                string replyToken = receivedMessage.events.FirstOrDefault().replyToken;
                List<MessageBase> messages = _LineWebhookService.GetReplyMessages(receivedMessage);
                string result = _LineWebhookService.ResponseToLineServer(replyToken, messages);

                return Content(requestBody.ToString() + "\n" + result);
            } catch (Exception ex) {
                return Content($"Index 發生錯誤，requestBody: {requestBody}, ex: {ex}");
            }
        }
    }
}