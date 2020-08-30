using BL.Services;
using BL.Services.Interfaces;
using BL.Services.Line;
using Core.Domain.DTO.ResponseDTO.Line.Messages;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Line;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            _LineWebhookService = new LineWebhookService();
        }

        /// <summary>
        /// LineWebhook的入口，負責解讀line的訊息。
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index([FromBody] dynamic requestBody) {
            try {
                Console.WriteLine(requestBody.ToString());
                //Channel Access Token
                var token = ConfigService.LineChannelAccessToken;
                //create bot instance
                Bot bot = new Bot(token);
                //Get  Post RawData
                string postData = requestBody.ToString();
                ReceivedMessage receivedMessage = JsonConvert.DeserializeObject<ReceivedMessage>(requestBody.ToString());

                string replyToken2 = receivedMessage.events.FirstOrDefault().replyToken;
                string message = "你剛才說了 " + receivedMessage.events.FirstOrDefault().message;
                //bot.
                ////取得LineBot接收到的訊息
                var ReceivedMessage = bot.ReplyMessage(replyToken2, message);
                return new OkResult();
                ////發送訊息
                //var ret = LineBotHelper.SendMessage(
                //    new List<string>() { ReceivedMessage.result[0].content.from },
                //        "你剛才說了 " + ReceivedMessage.result[0].content.text);

                //如果給200，LineBot訊息就不會重送
                //return Request.CreateResponse(HttpStatusCode.OK, ret);

                //處理requestModel
                RequestModelFromLineServer lineRequestModel =
                    _LineWebhookService.GetLineRequestModel(requestBody);

                Console.WriteLine($"========== From LINE SERVER ==========");
                Console.WriteLine($"requestModel:");
                Console.WriteLine($"{JsonConvert.SerializeObject(lineRequestModel, Formatting.Indented)}");
                Console.WriteLine($"====================");

                string replyToken = lineRequestModel.Events[0].replyToken;
                List<Core.Domain.DTO.ResponseDTO.Line.Messages.Message> messages = _LineWebhookService.GetReplyMessages(lineRequestModel);
                string result = _LineWebhookService.ResponseToLineServer(replyToken, messages);

                return Content(requestBody.ToString() + "\n" + result);
            } catch (Exception ex) {
                return Content($"Index 發生錯誤，requestBody: {requestBody}, ex: {ex}");
            }
        }
    }
}