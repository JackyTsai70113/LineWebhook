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

        private Bot _bot;

        /// <summary>
        /// LineBotService 建構子
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configService"></param>
        /// <param name="telegramService"></param>
        /// <remarks>API doc: https://developers.line.biz/en/docs/messaging-api/</remarks>
        public LineBotService(
            ILogger<LineBotService> logger,
            IRedisConfigService configService,
            ITelegramService telegramService)
        {
            _logger = logger;
            _configService = configService;
            _telegramService = telegramService;
            _logger.LogInformation("LineBotService initialized");
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
                var bot = GetBot();
                if (bot == null)
                {
                    _logger.LogError("Cannot reply message because Line Bot is not initialized");
                    return false;
                }
                bot.ReplyMessage(token, messages);
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

        private LineSettings GetSettings()
        {
            var settings = _configService.Get<LineSettings>("LineSettings");
            if (settings == null)
            {
                _logger.LogWarning("LineSettings not found in Redis");
                return new LineSettings();
            }
            return settings;
        }

        private Bot GetBot()
        {
            if (_bot != null)
            {
                return _bot;
            }
            try
            {
                var settings = GetSettings();
                if (string.IsNullOrEmpty(settings.ChannelAccessToken))
                {
                    throw new InvalidOperationException("Line ChannelAccessToken is empty");
                }

                if (_bot == null)
                {
                    _bot = new Bot(settings.ChannelAccessToken);
                    _logger.LogInformation("Line Bot created, token prefix: {Prefix}...", settings.ChannelAccessToken[..Math.Min(10, settings.ChannelAccessToken.Length)]);
                }
                return _bot;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Line Bot");
                return null;
            }
        }
    }
}