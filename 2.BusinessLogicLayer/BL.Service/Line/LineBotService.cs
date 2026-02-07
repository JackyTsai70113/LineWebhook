using System.Net;
using System.Text.Json;
using BL.Service.Redis;
using BL.Service.Telegram;
using isRock.LineBot;
using Microsoft.Extensions.Logging;

namespace BL.Service.Line
{
    public class LineBotService : ILineBotService
    {
        private readonly ILogger<LineBotService> _logger;
        private readonly IRedisConfigService _configService;
        private readonly ITelegramService _telegramService;
        private readonly Bot _bot;

        /// <summary>
        /// LineBotService 建構子
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configService"></param>
        /// <param name="telegramService"></param>
        /// <remarks>API doc: https://developers.line.biz/en/docs/messaging-api/</remarks>
        public LineBotService(ILogger<LineBotService> logger, IRedisConfigService configService, ITelegramService telegramService)
        {
            _logger = logger;
            _configService = configService;
            _telegramService = telegramService;
            var token = configService.Get("Line:ChannelAccessToken");
            if (string.IsNullOrEmpty(token))
            {
                logger.LogWarning("Line:ChannelAccessToken not found in Redis");
            }
            _bot = new Bot(token);
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
                    _logger.LogError(ex, "ReplyMessage 錯誤, replyToken: {Token}, response: {Response}", token, responseStr);
                    PushToJacky($"ReplyMessage 錯誤, replyToken: {token}, response: {responseStr}");
                }
                else
                {
                    _logger.LogError(ex, "ReplyMessage 錯誤, replyToken: {token}, messages: {messages}, ex: {ex}", token, JsonSerializer.Serialize(messages), ex);
                }
                return false;
            }
        }

        /// <summary>
        /// 透過 Telegram 通知 Jacky
        /// </summary>
        public bool PushToJacky(string msg)
        {
            try
            {
                if (msg.Length > 4096)
                {
                    _logger.LogWarning("Message length is greater than 4096, truncating");
                    msg = msg[..4096];
                }
                _telegramService.NotifyByMessage(msg);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PushToJacky via Telegram 錯誤");
                return false;
            }
        }
    }
}