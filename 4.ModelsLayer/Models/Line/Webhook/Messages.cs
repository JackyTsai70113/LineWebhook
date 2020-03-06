using System;
using System.Collections.Generic;

namespace Models.Line.Webhook
{
    public class Message
    {
        public string type { get; set; }
        public string id { get; set; }
        public string text { get; set; }
    }
}
