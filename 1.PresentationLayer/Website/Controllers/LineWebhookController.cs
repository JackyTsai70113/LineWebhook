using System;
using System.Text.Json;
using BL.Service.Interface;
using BL.Service.Line;
using isRock.LineBot;
using isRock.LineBot.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Website.Controllers
{
    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LineWebhookController : ControllerBase
    {
        private readonly ILogger<LineWebhookController> Logger;
        private readonly LineBotService LineBotService;
        private readonly ILineWebhookService LineWebhookService;

        public LineWebhookController(
            ILogger<LineWebhookController> logger,
            LineBotService lineBotService,
            ILineWebhookService lineWebhookService)
        {
            Logger = logger;
            LineBotService = lineBotService;
            LineWebhookService = lineWebhookService;
        }

        /// <summary>
        /// LineWebhook的入口，解讀line的訊息並回覆訊息。
        /// </summary>
        /// <param name="receivedMessage">從line接收到的訊息字串</param>
        /// <returns>API 結果</returns>
        [HttpPost]
        [Route("index")]
        public void Index(ReceivedMessage receivedMessage)
        {
            string req = "";
            try
            {
                req = JsonSerializer.Serialize(receivedMessage);
                if (receivedMessage.events.Count == 0) return;
                Logger.LogInformation("request: {req}", req);

                var messages = LineWebhookService.GetReplyMessages(receivedMessage.events[0]);

                // Add 紀錄發至LineServer的requestBody
                Logger.LogInformation("response: {messages}", messages.ToJson());

                var result = LineBotService.ReplyMessage(receivedMessage.events[0].replyToken, messages);
                if (!result)
                {
                    Logger.LogError("回覆訊息失敗, request: {req}, messages: {messages}", req, messages.ToJson());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("[Index] request: {req}, ex: {ex}", req, ex);
            }
        }

        [HttpPost]
        [Route("test_response")]
        public IActionResult TestForResponse(ReceivedMessage receivedMessage)
        {
            try
            {
                var messages = LineWebhookService.GetReplyMessages(receivedMessage.events[0]);
                return Ok(messages.ToJson());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Set new android token for the current driver
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/linewebhook/test_request?cmd=cd%20123
        ///
        /// </remarks>
        /// <param name="cmd">can't be null or empty</param>
        /// <returns></returns>
        [HttpGet]
        [Route("test_request")]
        public IActionResult TestForRequest([FromQuery] string cmd)
        {
            var @event = new Event
            {
                type = "message",
                message = new Message
                {
                    type = "text",
                    text = cmd
                }
            };
            var messages = LineWebhookService.GetReplyMessages(@event);
            return Ok(messages.ToJson());
        }
    }
}