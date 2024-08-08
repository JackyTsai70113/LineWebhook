using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BL.Service.Telegram
{
    /// <remarks>telegram api doc: https://core.telegram.org/bots/api, https://github.com/TelegramBots</remarks>
    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBotClient Bot;
        private readonly string AdminUserId;

        public TelegramService(IConfiguration config, ITelegramBotClient telegramBotClient)
        {
            Bot = telegramBotClient;
            AdminUserId = config["Telegram:AdminUserId"];
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="message">通知訊息</param>
        public void NotifyByMessage(string message)
        {
            Bot.SendTextMessageAsync(1017180008, message);
        }

        /// <summary>
        /// 測試
        /// </summary>
        public async Task<IEnumerable<Message>> SendDiceAsync()
        {
            var msg1 = await Bot.SendDiceAsync(AdminUserId);
            var msg2 = await Bot.SendTextMessageAsync(
                chatId: AdminUserId,
                text: "Trying *all the parameters* of `sendMessage` method"
            );
            return new Message[] { msg1, msg2 };
        }

        public User GetMe()
        {
            var user = Bot.GetMeAsync().Result;
            return user;
        }

        public Message HandleUpdate(Update update)
        {
            Message result = new();
            if (update is { Message: { } message })
            {
                result = Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: message.Text).Result;

            }

            return result;
        }
    }
}