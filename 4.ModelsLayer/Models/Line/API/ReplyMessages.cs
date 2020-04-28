using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Line.API {

    public class ReplyMessages {
        public string replyToken { get; set; }
        public List<dynamic> messages { get; set; }
    }
}