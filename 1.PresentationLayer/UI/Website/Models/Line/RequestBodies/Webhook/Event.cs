using System;
using System.Collections.Generic;

namespace Website.Models.Line.RequestBodies.Webhook
{
    public class Event
    {
        public string type { get; set; }
        public string replyToken { get; set; }
        public Source source { get; set; }
        public long timestamp { get; set; }
        public string mode { get; set; }
        public dynamic message { get; set; }
    }

    public class Source
    {
        public string userId { get; set; }
        public string type { get; set; }
    }
}
