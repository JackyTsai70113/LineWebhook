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
        public List<Message> SendDice()
        {
            ChatId chatId = new(1017180008);
            var msg1 = _bot.SendDiceAsync(chatId).Result;
            var msg2 = _bot.SendTextMessageAsync(
             chatId: 1017180008, // or a chat id: 123456789
             text: "Trying *all the parameters* of `sendMessage` method"
            // parseMode: ParseMode.Markdown,
            // disableNotification: true,
            // replyToMessageId: e.Message.MessageId,
            // replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
            //  "Check sendMessage method",
            //  "https://core.telegram.org/bots/api#sendmessage"
            ).Result;
            return new List<Message> { msg1, msg2 };
        }

        //private async void Bot_OnMessage(object sender, MessageEventArgs e) {
        //    if (e.Message.Text != null) {
        //        Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

        //        await botClient.SendTextMessageAsync(
        //          chatId: e.Message.Chat,
        //          text: "You said:\n" + e.Message.Text
        //        );
        //    }
        //}

        public User GetMe()
        {
            var user = _bot.GetMeAsync().Result;
            return user;
        }
    }
}