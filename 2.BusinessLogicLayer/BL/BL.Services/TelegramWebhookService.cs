using BL.Services.Base;
using BL.Services.Interfaces;
using Core.Domain.Utilities;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BL.Services {

    public class TelegramWebhookService : BaseService, ITelegramWebhookService {
        //readonly TelegramBotClient botClient;

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

        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="message">通知訊息</param>
        public void NotifyByMessage(string message) {
            string uri = "https://api.telegram.org/bot1253249749:AAEhPVK8fvahMGCKee_ZtG8fOivf4CjKYsY/sendMessage?chat_id=1017180008&text=" + message;
            RequestUtility.GetStringFromGetRequest(uri);
        }
        
        private string GetMe() {
            var botClient = new TelegramBotClient("1253249749:AAEhPVK8fvahMGCKee_ZtG8fOivf4CjKYsY");
            var me = botClient.GetMeAsync().Result;
            string result = JsonConvert.SerializeObject(me);
            Console.WriteLine(result);
            return result;
        }
    }
}