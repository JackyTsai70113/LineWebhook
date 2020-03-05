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
        public string mode { get; set; }
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

        // "address":"XXX台灣XX市XX區XX街XXX號",
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        /*
        "address":"231台灣新北市新店區光明街290號",
"latitude":24.96469585209084,
"longitude":121.54056127110071
        */
    }
}