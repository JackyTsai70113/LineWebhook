using BL.Service.Redis;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BL.Service.Telegram
{
    /// <remarks>telegram api doc: https://core.telegram.org/bots/api, https://github.com/TelegramBots</remarks>
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly IRedisConfigService _configService;
        private TelegramSettings _settings;
        private ITelegramBotClient _bot;

        public TelegramService(
            ILogger<TelegramService> logger,
            IRedisConfigService configService)
        {
            _logger = logger;
            _configService = configService;
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="message">通知訊息</param>
        public void NotifyByMessage(string message)
        {
            try
            {
                var settings = GetSettings();
                if (settings == null)
                {
                    _logger.LogError("Cannot notify by message because TelegramSettings is null");
                    return;
                }
                var bot = GetBot();
                if (bot == null)
                {
                    _logger.LogError("Cannot notify by message because TelegramBotClient is null");
                    return;
                }
                bot.SendMessage(settings.AdminChatId, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Telegram message");
            }
        }

        /// <summary>
        /// 測試
        /// </summary>
        public async Task<IEnumerable<Message>> SendDiceAsync()
        {
            var settings = GetSettings();
            var bot = GetBot();
            var msg1 = await bot.SendDice(settings.AdminChatId);
            var msg2 = await bot.SendMessage(
                chatId: settings.AdminChatId,
                text: "Trying *all the parameters* of `sendMessage` method"
            );
            return new Message[] { msg1, msg2 };
        }

        public User GetMe()
        {
            var user = GetBot().GetMe().Result;
            return user;
        }

        public Message HandleUpdate(Update update)
        {
            Message result = new();
            if (update is { Message: { } message })
            {
                result = GetBot().SendMessage(
                    chatId: message.Chat.Id,
                    text: message.Text).Result;
            }

            return result;
        }

        private TelegramSettings GetSettings()
        {
            if (_settings != null)
            {
                return _settings;
            }
            try
            {
                _settings = _configService.Get<TelegramSettings>(nameof(TelegramSettings));
                if (_settings == null)
                {
                    _logger.LogWarning("TelegramSettings not found in Redis");
                    return null;
                }
                return _settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting TelegramSettings from Redis");
                return null;
            }
        }

        private ITelegramBotClient GetBot()
        {
            if (_bot != null)
            {
                return _bot;
            }
            try
            {
                var settings = GetSettings();
                if (settings == null)
                {
                    _logger.LogError("Cannot create TelegramBotClient because TelegramSettings is null");
                    return null;
                }
                if (_bot == null)
                {
                    _bot = new TelegramBotClient(settings.Token);
                    _logger.LogInformation("TelegramBotClient created, token prefix: {Prefix}...", settings.Token[..Math.Min(10, settings.Token.Length)]);
                }
                return _bot;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating TelegramBotClient");
                return null;
            }
        }
    }
}