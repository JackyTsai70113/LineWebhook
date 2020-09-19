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
    }
}