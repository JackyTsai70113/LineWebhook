using Newtonsoft.Json;
using System;
using Telegram.Bot;

namespace Core.Domain.ExternalServices.Telegram {

    public class TelegramProvider {

        public static string GetMe() {
            //string uri = "https://api.telegram.org/bot1253249749:AAEhPVK8fvahMGCKee_ZtG8fOivf4CjKYsY/sendMessage?chat_id=1017180008&text=HelloText";
            //RequestUtility.GetStringFromGetRequest(uri);
            var botClient = new TelegramBotClient("1253249749:AAEhPVK8fvahMGCKee_ZtG8fOivf4CjKYsY");
            var me = botClient.GetMeAsync().Result;
            string result = JsonConvert.SerializeObject(me);
            Console.WriteLine(result);
            return result;
        }
    }
}