namespace BL.Services.Interfaces {

    public interface ITelegramWebhookService {

        /// <summary>
        /// 判讀TelegramServer來的請求物件後回應
        /// </summary>
        /// <param name="requestBody">TelegramServer來的請求物件</param>
        /// <returns>LOG紀錄</returns>
        string Response();
    }
}