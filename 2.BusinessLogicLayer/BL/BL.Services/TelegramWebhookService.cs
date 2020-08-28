using BL.Services.Base;
using BL.Services.Interfaces;
using Newtonsoft.Json;
using System;
using Telegram.Bot;

namespace BL.Services {

    public class TelegramWebhookService : BaseService, ITelegramWebhookService {

        public TelegramWebhookService() {
        }

        /// <summary>
        /// 判讀TelegramServer來的請求物件後回應
        /// </summary>
        /// <param name="requestBody">TelegramServer來的請求物件</param>
        /// <returns>LOG紀錄</returns>
        public string Response() {
            return GetMe();
        }

        private string GetMe() {
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