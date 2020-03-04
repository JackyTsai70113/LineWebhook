using System;
using System.Collections.Generic;

namespace Website.Models
{
    public class LineSource
    {
        public List<Event> events { get; set; }
    }

    public class Event
    {
        public string type { get; set; }
        public string replyToken { get; set; }
        public Source source { get; set; }
        public long timestamp { get; set; }
        public Message message { get; set; }
    }

    public class Source
    {
        public string userId { get; set; }
        public string type { get; set; }
    }

    public class Message
    {
        public string type { get; set; }
        public string id { get; set; }
        public string text { get; set; }
    }
}