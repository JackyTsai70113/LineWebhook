using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages {

    /// <summary>
    /// 地點訊息
    /// </summary>
    public class LocationMessage : Message {

        public LocationMessage() {
            type = "location";
        }

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