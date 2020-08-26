using Models.Google.API;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Core.Domain.ExternalServices.Google {

    public class GoogleMapProvider {
        private static readonly string key = "AIzaSyAt8mx-_cf_K7SlutrTWNwuO0g4dPjqMNY";

        /// <summary>
        /// 打google Api 算地址的地理資訊
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>Geocoding</returns>
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

        /// <summary>
        /// 打google Api 算起點和終點間的距離及抵達時間
        /// </summary>
        /// <param name="destinationAddress">起點地址</param>
        /// <param name="originAddress">終點地址</param>
        /// <returns>DistanceMatrix</returns>
        public static DistanceMatrix GetDistanceMatrix(string destinationAddress, string originAddress) {
            string uri = "https://maps.googleapis.com/maps/api/distancematrix/json";
            uri += $"?origins={destinationAddress}";
            uri += $"&destinations={originAddress}";
            uri += $"&key={key}";
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Headers.Add("Content-Type", "application/json");

            var response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            var distanceMatrixStr = streamReader.ReadToEnd();

            // 去除換行字元
            distanceMatrixStr = distanceMatrixStr.Replace("\n", "");
            return JsonConvert.DeserializeObject<DistanceMatrix>(distanceMatrixStr);
        }
    }
}