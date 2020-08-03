using System;
using Core.Domain.Utilities;
using Telegram.Bot;

namespace Utility.Telegram {

    /// <summary>
    /// Handle Request
    /// </summary>
    public class TelegramResponseHandler {
        public static string GetMe() {
            var botClient = new TelegramBotClient("1253249749:AAEhPVK8fvahMGCKee_ZtG8fOivf4CjKYsY");
            var me = botClient.GetMeAsync().Result;
            string result = me.Serialize();
            Console.WriteLine(result);
            return result;
        }
    }
}