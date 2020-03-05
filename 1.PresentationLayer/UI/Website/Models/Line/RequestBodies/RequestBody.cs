using System;
using System.Collections.Generic;

namespace Website.Models.Line
{
    public class LineRequestBody
    {
        public string destination { get; set; }
        public List<Event> events { get; set; }
    }

    public class Event
    {
        public string type { get; set; }
        public string replyToken { get; set; }
        public Source source { get; set; }
        public long timestamp { get; set; }
        public string mode { get; set; }
        public Website.Models.Line.Message message { get; set; }
    }

    public class Source
    {
        public string userId { get; set; }
        public string type { get; set; }
    }

    public class Message
    {
        public string userId { get; set; }
        public string type { get; set; }
    }
}
