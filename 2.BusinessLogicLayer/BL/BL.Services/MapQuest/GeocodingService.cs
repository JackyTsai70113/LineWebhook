using Core.Domain.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;

namespace BL.Services.MapQuest {

    public class GeocodingService {
        private readonly string _mapQuest_Key;

        public GeocodingService() {
            _mapQuest_Key = ConfigService.MapQuest_Key;
        }

        public LatLng GetLatLngFromAddress(string address) {
            LatLng latLng = new LatLng();
            var encodedAddress = HttpUtility.UrlEncode(address, Encoding.GetEncoding("UTF-8"));
            string uri = "http://www.mapquestapi.com/geocoding/v1/address?" +
                "key=" + _mapQuest_Key + "&" +
                "inFormat=kvp&" +
                "outFormat=json&" +
                "location=" + encodedAddress + "&" +
                "thumbMaps=false";
            string ssss = RequestUtility.GetStringFromGetRequest(uri);
            var response = JsonConvert.DeserializeObject<Response>(ssss);
            List<Location> locations = response.results[0].locations;

            foreach (Location location in locations) {
                if (location.adminArea1 == "TW") {
                    latLng.lat = location.latLng.lat;
                    latLng.lng = location.latLng.lng;
                    break;
                }
            }
            if (latLng.lat == default || latLng.lng == default) {
                string msg = "該地址找不到對應的經緯度: " + address;
                Log.Error(msg);
            }
            return latLng;
        }

        private class Response {
            public List<Result> results { get; set; }
        }

        private class Result {
            public List<Location> locations { get; set; }
        }

        private class Location {
            public string adminArea1 { get; set; }
            public LatLng latLng { get; set; }
        }
    }

    public class LatLng {
        public float lat { get; set; }
        public float lng { get; set; }
    }
}