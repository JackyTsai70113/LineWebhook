using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Line.Webhook {

    /// <summary>
    /// 回覆給line的Request body
    /// </summary>
    public class RequestBodyToLine {

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
        public List<dynamic> messages { get; set; }
    }
}