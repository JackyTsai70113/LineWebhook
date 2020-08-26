using BL.Interfaces;
using BL.Services.Base;
using Core.Domain.ExternalServices.Telegram;

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
            return TelegramProvider.GetMe();
        }
    }
}