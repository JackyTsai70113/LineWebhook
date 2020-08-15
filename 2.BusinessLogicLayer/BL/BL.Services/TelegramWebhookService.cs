using BL.Interfaces;
using BL.Services.Base;
using Utility.Telegram;

namespace BL.Services {
    public class TelegramWebhookService : BaseService, ITelegramWebhookService {
        private readonly TelegramResponseHandler telegramResponseHandler;
        public TelegramWebhookService() {
            this.telegramResponseHandler = new TelegramResponseHandler();
        }
        /// <summary>
        /// 判讀TelegramServer來的請求物件後回應
        /// </summary>
        /// <param name="requestBody">TelegramServer來的請求物件</param>
        /// <returns>LOG紀錄</returns>
        public string Response() {
            return TelegramResponseHandler.GetMe();
        }
    }
}