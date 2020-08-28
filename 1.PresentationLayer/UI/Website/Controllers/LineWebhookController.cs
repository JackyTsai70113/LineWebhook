using BL.Services;
using BL.Services.Interfaces;
using Core.Domain.DTO.ResponseDTO.Line.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Line;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Website.Controllers {

    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    [ApiController]
    [Route("LineWebhook")]
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
                //處理requestModel
                RequestModelFromLineServer lineRequestModel =
                    _LineWebhookService.GetLineRequestModel(requestBody);

                Console.WriteLine($"========== From LINE SERVER ==========");
                Console.WriteLine($"requestModel:");
                Console.WriteLine($"{JsonConvert.SerializeObject(lineRequestModel, Formatting.Indented)}");
                Console.WriteLine($"====================");

                string replyToken = lineRequestModel.Events[0].replyToken;
                List<Message> messages = _LineWebhookService.GetReplyMessages(lineRequestModel);
                string result = _LineWebhookService.ResponseToLineServer(replyToken, messages);

                return Content(requestBody.ToString() + "\n" + result);
            } catch (Exception ex) {
                return Content($"Index 發生錯誤，requestBody: {requestBody}, ex: {ex}");
            }
        }
    }
}