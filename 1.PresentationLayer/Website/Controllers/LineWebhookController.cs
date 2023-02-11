using System;
using BL.Service.Interface;
using BL.Service.Line.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using isRock.LineBot;
using System.Collections.Generic;

namespace Website.Controllers
{
    /// <summary>
    /// LineWebhook控制器，Line Server 的 I/O
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class LineWebhookController : ControllerBase
    {
        private readonly ILogger<LineWebhookController> Logger;
        private readonly ILineBotService LineBotService;
        private readonly ILineWebhookService LineWebhookService;

        public LineWebhookController(
            ILogger<LineWebhookController> logger
            , ILineBotService lineBotService
            , ILineWebhookService lineWebhookService)
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
        public IActionResult Index(ReceivedMessage receivedMessage)
        {
            var req = JsonSerializer.Serialize(receivedMessage);
            try
            {
                if (receivedMessage.events.Count == 0) return Ok();
                Logger.LogDebug("request: {req}", req);

                var messages = LineWebhookService.GetReplyMessages(receivedMessage.events[0]);

                // Add 紀錄發至LineServer的requestBody
                Logger.LogDebug("response: {messages}", JsonSerializer.Serialize(messages));

                LineBotService.ReplyMessage(receivedMessage.events[0].replyToken, messages);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogError("[Index] request: {req}, ex: {ex}", req, ex);
                return Ok();
            }
        }

        [HttpPost]
        [Route("test_response")]
        public IActionResult TestForResponse(ReceivedMessage receivedMessage)
        {
            var req = JsonSerializer.Serialize(receivedMessage);
            try
            {
                var messages = LineWebhookService.GetReplyMessages(receivedMessage.events[0]);
                return Ok($"request: {JsonSerializer.Serialize(receivedMessage)}\nresponse: {JsonSerializer.Serialize(messages)}");
            }
            catch (Exception ex)
            {
                Logger.LogError("[TestForResponse] req: {req}, ex: {ex}", req, ex);
                return Content($"error, req: {req}, ex: {ex}");
            }
        }

        /// <summary>
        /// Set new android token for the current driver
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /SetToken?token=new_token
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
            return Ok(JsonSerializer.Serialize(messages));
        }

        /// <summary>
        /// Retrieves a specific product by unique id
        /// </summary>
        /// <remarks>Awesomeness!</remarks>
        /// <response code="200">Product retrieved</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Oops! Can't lookup your product right now</response>
        [HttpGet]
        [Route("test")]
        public ActionResult<Product> Test()
        {
            Logger.LogInformation("Info!");
            Logger.LogWarning("Warning!");
            Logger.LogTrace("Trace!");
            Logger.LogDebug("Debug");
            Logger.LogCritical("Critical");
            Logger.LogError("Error");
            // bool res = lineNotifyBotService.Notify_Jacky("Test line notify");
            List<string> sizes = new()
            {
                "123"
            };
            var p = new Product { Name = "name123", AvailableStock = 123, Sizes = sizes };
            return Ok(p);
        }

        public class Product
        {
            /// <summary>
            /// The name of the product
            /// </summary>
            /// <example>Men's basketball shoes</example>
            public string Name { get; set; }

            /// <summary>
            /// Quantity left in stock
            /// </summary>
            /// <example>10</example>
            public int AvailableStock { get; set; }

            /// <summary>
            /// The sizes the product is available in
            /// </summary>
            // <example>["Small", "Medium", "Large"]</example>
            public List<string> Sizes { get; set; }
        }
    }
}