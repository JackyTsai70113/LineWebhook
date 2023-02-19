using System.Net;
using System.Text.Json;
using isRock.LineBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BL.Service.Line
{
    public class LineBotService : ILineBotService
    {
        private readonly ILogger<LineBotService> _logger;
        private readonly IConfiguration _config;
        private readonly Bot _bot;

        /// <summary>
        /// sss
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <remarks>API doc: https://notify-bot.line.me/doc/en/</remarks>
        public LineBotService(ILogger<LineBotService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _bot = new Bot(config["Line:ChannelAccessToken"]);
        }

        /// <summary>
        /// 推播至Group
        /// </summary>
        /// <param name="msg">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool PushMessage_Group(string msg)
        {
            return SendNotify(_config["Line:NotifyBearerToken_Group"], msg);
        }

        /// <summary>
        /// 推播至Jacky
        /// </summary>
        /// <param name="msg">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool Notify_Jacky(string msg)
        {
            var result = _bot.SendNotify(_config["Line:NotifyBearerToken_Jacky"]
                , msg
                , new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/240px-Cat03.jpg")
                , new Uri("https://images.hdqwalls.com/download/cat-4k-po-2048x2048.jpg")
                , 6362
                , 11087927
                , false
            );

            if (result.status != 200)
            {
                _logger.LogError("{message}", result.message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 推播至Jessi
        /// </summary>
        /// <param name="msg">推播字串</param>
        /// <returns>是否推播成功</returns>
        public bool PushMessage_Jessi(string msg)
        {
            return SendNotify(_config["Line:NotifyBearerToken_Jessi"], msg);
        }

        public bool ReplyMessage(string token, List<MessageBase> messages)
        {
            try
            {
                _bot.ReplyMessage(token, messages);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is WebException)
                {
                    int responseStartIndex = ex.ToString().IndexOf("Response") + "Response:".Length;
                    int responseEndIndex = ex.ToString().IndexOf("Endpoint");
                    string responseStr = ex.ToString()[responseStartIndex..responseEndIndex].Trim();
                    LineHttpPostException response = JsonSerializer.Deserialize<LineHttpPostException>(responseStr);
                    _logger.LogError(
                        "ReplyMessage 錯誤, replyToken: {token},\n" +
                        "messages: {messages},\n" +
                        "response: {response},\n" +
                        "ex: {ex}", token, JsonSerializer.Serialize(messages), JsonSerializer.Serialize(response), ex);
                    Notify_Jacky($"message: {response.Message},\n" +
                        $"details: {JsonSerializer.Serialize(response.Details)}");
                }
                return false;
            }
        }

        private bool SendNotify(string token, string msg)
        {
            var result = _bot.SendNotify(token, msg);
            if (result.status != 200)
            {
                _logger.LogError("{message}", result.message);
                return false;
            }
            return true;
        }
    }
}