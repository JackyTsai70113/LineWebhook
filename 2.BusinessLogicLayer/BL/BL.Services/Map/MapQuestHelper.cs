using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Web;
using Core.Domain.Utilities;
using Microsoft.Extensions.Configuration;

namespace BL.Services.Map {

    /// <summary>
    /// MapQuest服務的Map API，15000 transactions per month for free
    /// https://developer.mapquest.com/documentation/
    /// </summary>
    public class MapQuestService {

        /// <summary>
        /// mapQuest 的 Api key
        /// </summary>
        private readonly string _apiKey;

        public MapQuestService(IConfiguration config) {
            _apiKey = config["MapQuest_Key"];
        }

        /// <summary>
        /// 透過地址取得經緯度
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>經緯度</returns>
        public Core.Domain.DTO.Map.LatLng GetLatLngFromAddress(string address) {
            LatLng latLng = new LatLng();
            address = "臺灣" + address;
            var encodedAddress = HttpUtility.UrlEncode(address, Encoding.GetEncoding("UTF-8"));
            string uri = "http://www.mapquestapi.com/geocoding/v1/address?" +
                "key=" + _apiKey + "&" +
                "inFormat=kvp&" +
                "outFormat=json&" +
                "location=" + encodedAddress + "&" +
                "thumbMaps=false";
            string responseStr = RequestUtility.GetStringFromGetRequest(uri);
            var response = JsonSerializer.Deserialize<Response>(responseStr);

            List<Location> locations = response.results[0].locations;
            foreach (Location location in locations) {
                if (location.latLng.lat != default && location.latLng.lng != default) {
                    latLng.lat = location.latLng.lat;
                    latLng.lng = location.latLng.lng;
                    break;
                }
            }

            return new Core.Domain.DTO.Map.LatLng {
                lat = latLng.lat,
                lng = latLng.lng
            };
        }

        private class Response {
            public List<Result> results { get; set; }
        }

        private class Result {
            public List<Location> locations { get; set; }
        }

        /// <summary>
        ///
        /// </summary>
        private class Location {
            public string adminArea1 { get; set; }
            public LatLng latLng { get; set; }
        }

        private class LatLng {

            /// <summary>
            /// 緯度
            /// </summary>
            public float lat { get; set; }

            /// <summary>
            /// 經度
            /// </summary>
            public float lng { get; set; }
        }
    }
}