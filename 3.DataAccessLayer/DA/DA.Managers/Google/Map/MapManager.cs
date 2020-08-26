using Models.Google.API;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace DA.Managers.Google.Map {

    public class MapManager {
        private static readonly string key = "AIzaSyAt8mx-_cf_K7SlutrTWNwuO0g4dPjqMNY";

        public static Geocoding GetGeocoding(string address) {
            string uri = "https://maps.googleapis.com/maps/api/geocode/json";
            uri += $"?address={address}";
            uri += $"&key={key}";
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Headers.Add("Content-Type", "application/json");

            var response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            var geocodingStr = streamReader.ReadToEnd();

            // 去除換行字元
            geocodingStr = geocodingStr.Replace("\n", "");
            return JsonConvert.DeserializeObject<Geocoding>(geocodingStr);
        }
    }
}