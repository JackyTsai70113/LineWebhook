using BL.Service.Interface;
using Core.Domain.Utilities;
using System.Text.Json;
using Telegram.Bot;

namespace BL.Service
{
    public class TelegramWebhookService : ITelegramWebhookService
    {
        public TelegramWebhookService()
        {
        }

        /// <summary>
        /// 判讀TelegramServer來的請求物件後回應
        /// </summary>
        /// <param name="requestBody">TelegramServer來的請求物件</param>
        /// <returns>LOG紀錄</returns>
        public string Response()
        {
            return GetMe();
        }

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="message">通知訊息</param>
        public void NotifyByMessage(string message)
        {
            string uri = "https://api.telegram.org/bot1253249749:AAEhPVK8fvahMGCKee_ZtG8fOivf4CjKYsY/sendMessage?chat_id=1017180008&text=" + message;
            RequestUtility.GetStringFromGetRequest(uri);
        }

        /// <summary>
        /// 測試
        /// </summary>
        public void Test()
        {
            //botClient = new TelegramBotClient("1253249749:AAEhPVK8fvahMGCKee_ZtG8fOivf4CjKYsY");
            //ChatId chatId = new ChatId(1017180008);
            //var me = botClient.SendDiceAsync(chatId);
            //Message message = botClient.SendTextMessageAsync(
            //  chatId: 1017180008, // or a chat id: 123456789
            //  text: "Trying *all the parameters* of `sendMessage` method"
            ////parseMode: ParseMode.Markdown,
            ////disableNotification: true,
            ////replyToMessageId: e.Message.MessageId,
            ////replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
            ////  "Check sendMessage method",
            ////  "https://core.telegram.org/bots/api#sendmessage"
            ////))
            //).Result;
            //string uri = "https://api.telegram.org/bot1253249749:AAEhPVK8fvahMGCKee_ZtG8fOivf4CjKYsY/sendDice?chat_id=1017180008";
            //Console.WriteLine(RequestUtility.GetStringFromGetRequest(uri));
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

        private static string GetMe()
        {
            var botClient = new TelegramBotClient("1253249749:AAEhPVK8fvahMGCKee_ZtG8fOivf4CjKYsY");
            var me = botClient.GetMeAsync().Result;
            string result = JsonSerializer.Serialize(me);
            Console.WriteLine(result);
            return result;
        }
    }
}