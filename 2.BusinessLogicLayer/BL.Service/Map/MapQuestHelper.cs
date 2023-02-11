using System.Text;
using System.Text.Json;
using System.Web;
using Core.Domain.Utilities;
using Microsoft.Extensions.Configuration;

namespace BL.Service.Map
{

    /// <summary>
    /// MapQuest服務的Map API，15000 transactions per month for free
    /// https://developer.mapquest.com/documentation/
    /// </summary>
    public class MapQuestService
    {

        /// <summary>
        /// mapQuest 的 Api key
        /// </summary>
        private readonly string _apiKey;

        public MapQuestService(IConfiguration config)
        {
            _apiKey = config["MapQuest_Key"];
        }

        /// <summary>
        /// 透過地址取得經緯度
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>經緯度</returns>
        public Core.Domain.DTO.Map.LatLng GetLatLngFromAddress(string address)
        {
            LatLng latLng = new();
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

            List<Location> locations = response.Results[0].Locations;
            foreach (Location location in locations)
            {
                if (location.LatLng.Lat != default && location.LatLng.Lng != default)
                {
                    latLng.Lat = location.LatLng.Lat;
                    latLng.Lng = location.LatLng.Lng;
                    break;
                }
            }

            return new Core.Domain.DTO.Map.LatLng
            {
                Lat = latLng.Lat,
                Lng = latLng.Lng
            };
        }

        private class Response
        {
            public List<Result> Results { get; set; }
        }

        private class Result
        {
            public List<Location> Locations { get; set; }
        }

        /// <summary>
        ///
        /// </summary>
        private class Location
        {
            public string AdminArea1 { get; set; }
            public LatLng LatLng { get; set; }
        }

        private class LatLng
        {

            /// <summary>
            /// 緯度
            /// </summary>
            public float Lat { get; set; }

            /// <summary>
            /// 經度
            /// </summary>
            public float Lng { get; set; }
        }
    }
}