using System;
using System.Collections.Generic;

namespace Website.Models
{
    public class ReplyMessages
    {
        public string replyToken { get; set; }
        public List<TextMessage> messages { get; set; }
    }
}
