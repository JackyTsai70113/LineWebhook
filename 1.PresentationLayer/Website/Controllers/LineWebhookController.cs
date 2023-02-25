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
        private readonly ILineBotService LineBotService;
        private readonly ILineWebhookService LineWebhookService;
        private readonly IChatGPTService ChatGPTService;

        public LineWebhookController(
            ILogger<LineWebhookController> logger
            , ILineBotService lineBotService
            , ILineWebhookService lineWebhookService
            , IChatGPTService chatGPTService)
        {
            Logger = logger;
            LineBotService = lineBotService;
            LineWebhookService = lineWebhookService;
            ChatGPTService = chatGPTService;
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
            var req = JsonSerializer.Serialize(receivedMessage);
            try
            {
                if (receivedMessage.events.Count == 0) return;
                Logger.LogInformation("request: {req}", req);

                var messages = LineWebhookService.GetReplyMessages(receivedMessage.events[0]);

                // Add 紀錄發至LineServer的requestBody
                Logger.LogInformation("response: {messages}", messages.ToJson());

                LineBotService.ReplyMessage(receivedMessage.events[0].replyToken, messages);
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

        /// <summary>
        /// Retrieves a specific product by unique id
        /// </summary>
        /// <remarks>Awesomeness!</remarks>
        /// <response code="200">Product retrieved</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Oops! Can't lookup your product right now</response>
        /// GET api/linewebhook/test
        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            Logger.LogInformation("Info!");
            Logger.LogWarning("Warning!");
            Logger.LogTrace("Trace!");
            Logger.LogDebug("Debug");
            Logger.LogCritical("Critical");
            Logger.LogError("Error");

            var result = ChatGPTService.CallChatGPT("你好嗎");
            return Ok(result);
        }
    }
}