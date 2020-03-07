using System;
using System.Collections.Generic;

namespace Models.Line.Webhook
{
    public class Message
    {
    }

    public class TextMessage : Message
    {
        public string type { get; set; } =  "text";
        public string id { get; set; }
        public string text { get; set; }
    }

    public class LocationMessage : Message
    {
        public string type { get; set; } = "location";
        public string id { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

        /*
            {
                "type": "location",
                "id": "325708",
                "title": "my location",
                "address": "〒150-0002 東京都渋谷区渋谷２丁目２１−１",
                "latitude": 35.65910807942215,
                "longitude": 139.70372892916203
            }
        */
    }
}
