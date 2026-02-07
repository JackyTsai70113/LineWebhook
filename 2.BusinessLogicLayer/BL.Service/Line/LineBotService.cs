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
            _logger.LogInformation("LineBotService initialized");
            _bot = new Bot(token);
        }

        public bool ReplyMessage(string token, List<MessageBase> messages)
        {
            _logger.LogInformation("ReplyMessage 開始, replyToken: {Token}, messages count: {Count}", token, messages.Count);
            for (int i = 0; i < messages.Count; i++)
            {
                _logger.LogInformation("Message[{Index}] type: {Type}, content: {Content}", i, messages[i].GetType().Name, JsonSerializer.Serialize(messages[i]));
            }

            try
            {
                _bot.ReplyMessage(token, messages);
                _logger.LogInformation("ReplyMessage 成功, replyToken: {Token}", token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReplyMessage 失敗, replyToken: {Token}, messages: {Messages}", token, JsonSerializer.Serialize(messages));
                PushToJacky($"ReplyMessage 失敗, replyToken: {token}, ex: {ex.Message}");
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