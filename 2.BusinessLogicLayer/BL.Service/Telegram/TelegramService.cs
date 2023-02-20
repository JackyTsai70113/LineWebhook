using Telegram.Bot;
using Telegram.Bot.Types;

namespace BL.Service.Telegram
{
    /// <remarks>telegram api doc: https://core.telegram.org/bots/api</remarks>
    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBotClient _bot;

        public TelegramService(ITelegramBotClient telegramBotClient)
        {
            _bot = telegramBotClient;
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="message">通知訊息</param>
        public void NotifyByMessage(string message)
        {
            _bot.SendTextMessageAsync("1017180008", message);
        }

        /// <summary>
        /// 測試
        /// </summary>
        public async Task<IEnumerable<Message>> SendDiceAsync()
        {
            ChatId chatId = new(1017180008);
            var msg1 = await _bot.SendDiceAsync(chatId);
            var msg2 = await _bot.SendTextMessageAsync(
                chatId: 1017180008,
                text: "Trying *all the parameters* of `sendMessage` method"
            );
            return new Message[] { msg1, msg2 };
        }

        public User GetMe()
        {
            var user = _bot.GetMeAsync().Result;
            return user;
        }

        public Message UpdateWebhook(Update update)
        {
            return _bot.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "You said: " + update.Message.Text
            ).Result;
        }
    }
}