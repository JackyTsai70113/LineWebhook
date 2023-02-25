using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BL.Service.Telegram
{
    /// <remarks>telegram api doc: https://core.telegram.org/bots/api</remarks>
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly ITelegramBotClient _bot;

        public TelegramService(ILogger<TelegramService> logger, ITelegramBotClient telegramBotClient)
        {
            _logger = logger;
            _bot = telegramBotClient;
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="message">通知訊息</param>
        public void NotifyByMessage(string message)
        {
            _bot.SendTextMessageAsync(1017180008, message);
        }

        /// <summary>
        /// 測試
        /// </summary>
        public async Task<IEnumerable<Message>> SendDiceAsync()
        {
            var msg1 = await _bot.SendDiceAsync(1017180008);
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

        public Message HandleUpdate(Update update)
        {
            Message result = new();
            if (update is { Message: { } message })
            {
                result = _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: message.Text).Result;

            }

            return result;
        }

        public Message BotOnMessageReceived(Message message)
        {
            _logger.LogInformation("Receive message type: {MessageType}", message.Type);
            if (message.Text is not { } messageText)
                Console.WriteLine("aaaa");
            Message sentMessage = SendInlineKeyboard(_bot, message);
            _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
            return sentMessage;
            // Send inline keyboard
            // You can process responses in BotOnCallbackQueryReceived handler
            static Message SendInlineKeyboard(ITelegramBotClient _bot, Message message)
            {
                _bot.SendChatActionAsync(
                    chatId: message.Chat.Id,
                    chatAction: ChatAction.Typing).Wait();

                // Simulate longer running task
                Task.Delay(500).Wait();

                InlineKeyboardMarkup inlineKeyboard = new(
                    new[]
                    {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    },
                    });

                var msg = _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: inlineKeyboard).Result;
                return msg;
            }
        }
    }
}