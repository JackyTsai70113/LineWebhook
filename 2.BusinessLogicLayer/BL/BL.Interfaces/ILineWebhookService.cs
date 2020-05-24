using BL.Interfaces.Base;

namespace BL.Interfaces {

    public interface ILineWebhookService : IBaseService {

        /// <summary>
        /// 判讀LineServer來的請求物件後回應
        /// </summary>
        /// <param name="requestBody">LineServer來的請求物件</param>
        /// <returns>LOG紀錄</returns>
        string Response(dynamic requestBody);
    }
}