using System.Text;
using System.Text.Json;
using System.Web;
using Core.Domain.Utilities;
using Microsoft.Extensions.Configuration;

namespace BL.Service.MapQuest
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
        private readonly IHttpClientFactory _httpClientFactory;

        public MapQuestService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _apiKey = config["MapQuest_Key"];
            _httpClientFactory = httpClientFactory;
        }

        public async Task<int> GetDuration(Core.Domain.DTO.Map.LatLng l1, Core.Domain.DTO.Map.LatLng l2)
        {
            HttpRequestMessage httpRequestMessage = new(
                HttpMethod.Get,
                $"http://www.mapquestapi.com/directions/v2/route?" +
                $"key={_apiKey}&" +
                $"from={l1.Lat},{l1.Lng}&" +
                $"to={l2.Lat},{l2.Lng}&" +
                $"routeType=pedestrian");

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            var getRouteResponse = await JsonSerializer.DeserializeAsync<GetRouteResponse>(contentStream);
            return getRouteResponse.Route.Time;
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
            HttpRequestMessage httpRequestMessage = new(
                HttpMethod.Get,
                "http://www.mapquestapi.com/geocoding/v1/address?" +
                "key=" + _apiKey + "&" +
                "inFormat=kvp&" +
                "outFormat=json&" +
                "location=" + encodedAddress + "&" +
                "thumbMaps=false");

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = httpClient.SendAsync(httpRequestMessage).Result;
            httpResponseMessage.EnsureSuccessStatusCode();
            using var contentStream = httpResponseMessage.Content.ReadAsStreamAsync().Result;
            var response = JsonSerializer.Deserialize<Response>(contentStream);

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