using Core.Domain.DTO.ResponseDTO.Line.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line {

    /// <summary>
    /// LINE 的 Reply Message 的 Request body
    /// </summary>
    public class ReplyMessageRequestBody {

        /// <summary>
        /// 是否接收到通知
        /// </summary>
        public bool notificationDisabled { get; set; }

        /// <summary>
        /// webhook接收到的回應權杖
        /// </summary>
        public string replyToken { get; set; }

        /// <summary>
        /// 回覆的訊息列表，最多五則
        /// </summary>
        public List<Message> messages { get; set; }
    }
}