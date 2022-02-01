using isRock.LineBot;
using System.Collections.Generic;

namespace BL.Services.Interfaces {

    public interface ILineWebhookService {

        /// <summary>
        /// 依照RequestModel取得Line回應訊息
        /// </summary>
        /// <param name="lineRequestModel"></param>
        /// <returns>Line回應訊息</returns>
        List<MessageBase> GetReplyMessages(ReceivedMessage lineRequestModel);

        /// <summary>
        /// 依照字串內容給於不同的 LINE 回應
        /// </summary>
        /// <param name="text">字串內容</param>
        /// <returns>回應結果</returns>
        List<MessageBase> GetMessagesByText(string text);

        string GetReplyTextByText(string text);
    }
}