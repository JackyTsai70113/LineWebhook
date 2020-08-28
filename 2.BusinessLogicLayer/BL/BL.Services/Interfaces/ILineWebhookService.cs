using Core.Domain.DTO.ResponseDTO.Line.Messages;
using Models.Line;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services.Interfaces {

    public interface ILineWebhookService {

        /// <summary>
        /// 依照RequestModel取得Line回應訊息
        /// </summary>
        /// <param name="lineRequestModel"></param>
        /// <returns>Line回應訊息</returns>
        List<Message> GetReplyMessages(RequestModelFromLineServer lineRequestModel);

        /// <summary>
        /// 將requestBody轉換成Line的RequestModel
        /// </summary>
        /// <param name="requestBody">Line的將requestBody</param>
        /// <returns>Line的RequestModel</returns>
        RequestModelFromLineServer GetLineRequestModel(dynamic requestBody);

        /// <summary>
        /// 回覆Line Server
        /// </summary>
        /// <param name="replyToken">回覆token</param>
        /// <param name="messages">訊息列表</param>
        /// <returns>API結果</returns>
        string ResponseToLineServer(string replyToken, List<Message> messages);
    }
}