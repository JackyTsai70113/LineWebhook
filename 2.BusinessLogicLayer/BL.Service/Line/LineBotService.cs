using System.Net;
using System.Text.Json;
using BL.Service.Redis;
using isRock.LineBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BL.Service.Line
{
    public class LineBotService : ILineBotService
    {
        private readonly ILogger<LineBotService> _logger;
        private readonly IRedisConfigService _configService;
        private readonly Bot _bot;

        /// <summary>
        /// LineBotService 建構子
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configService"></param>
        /// <remarks>API doc: https://developers.line.biz/en/docs/messaging-api/</remarks>
        public LineBotService(ILogger<LineBotService> logger, IRedisConfigService configService)
        {
            _logger = logger;
            _configService = configService;
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
        /// 推播訊息至 Jacky（使用 Messaging API PushMessage，Line Notify 已於 2025/3/31 停用）
        /// </summary>
        public bool PushToJacky(string msg)
        {
            try
            {
                var userId = _configService.Get("Line:Jacky_userId");
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Line:Jacky_userId is null or empty, skip sending message to Jacky");
                    return false;
                }
                if (msg.Length > 5000)
                {
                    _logger.LogWarning("Message length is greater than 5000, truncating");
                    msg = msg[..5000];
                }
                _bot.PushMessage(userId, msg);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Notify_Jacky 錯誤");
                return false;
            }
        }

        /// <summary>
        /// 推播訊息至 Group
        /// </summary>
        public bool PushMessage_Group(string text)
        {
            try
            {
                var groupToken = _configService.Get("Line:NotifyBearerToken_Group");
                if (string.IsNullOrEmpty(groupToken))
                {
                    _logger.LogWarning("Line:NotifyBearerToken_Group is null or empty, skip sending message to Group");
                    return false;
                }
                _bot.PushMessage(groupToken, text);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("PushMessage_Group 錯誤: {ex}", ex);
                return false;
            }
        }

        /// <summary>
        /// 推播訊息至 Jessi
        /// </summary>
        public bool PushMessage_Jessi(string text)
        {
            try
            {
                var userId = _configService.Get("Line:Jessi_userId");
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Line:Jessi_userId is null or empty, skip sending message to Jessi");
                    return false;
                }
                _bot.PushMessage(userId, text);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("PushMessage_Jessi 錯誤: {ex}", ex);
                return false;
            }
        }
    }
}