using System;
using System.Collections.Generic;

namespace Website.Models
{
    public class ReplyMessages
    {
        public string replyToken { get; set; }
        public List<Message> messages { get; set; }
    }
}
