using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace BL.Service.MapQuest
{
    /// <summary>
    /// MapQuest服務的Map API，15000 transactions per month for free
    /// https://developer.mapquest.com/documentation/
    /// </summary>
    public class MapQuestService : IMapQuestService
    {
        /// <summary>
        /// mapQuest 的 Api key
        /// </summary>
        private readonly string _apiKey;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiDomain = "http://www.mapquestapi.com";

        public MapQuestService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _apiKey = config["MapQuest_Key"];
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 將 目標地址列表 依 來源地址的遠近 排序，越近越前面
        /// </summary>
        /// <param name="sourceAddress">來源地址</param>
        /// <param name="targetAddresses">目標地址列表</param>
        /// <returns>地址列表</returns>
        public async Task<List<string>> GetAddressInOrderAsync(string sourceAddress, List<string> targetAddresses)
        {
            LatLng sourceLatLng = await GetLatLngAsync(sourceAddress);
            var res = targetAddresses.OrderBy(address =>
            {
                LatLng targetLatLng = GetLatLngAsync(address).Result;
                return GetRouteAsync(sourceLatLng, targetLatLng).Result.Route.Time;
            }).ToList();
            return res;
        }

        public async Task<GetRouteResponse> GetRouteAsync(LatLng l1, LatLng l2)
        {
            HttpRequestMessage httpRequestMessage = new(
                HttpMethod.Get,
                _apiDomain + "/directions/v2/route?" +
                "key=" + _apiKey +
                "&from=" + l1.Lat + "," + l1.Lng +
                "&to=" + l2.Lat + "," + l2.Lng +
                "&routeType=pedestrian"
                );
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<GetRouteResponse>(contentStream);
        }

        /// <summary>
        /// 透過地址取得經緯度
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>經緯度</returns>
        public async Task<LatLng> GetLatLngAsync(string address)
        {
            var encodedAddress = HttpUtility.UrlEncode("臺灣" + address, Encoding.GetEncoding("UTF-8"));
            HttpRequestMessage httpRequestMessage = new(
                HttpMethod.Get,
                _apiDomain + "/geocoding/v1/address?" +
                    "key=" + _apiKey + "&" +
                    "inFormat=kvp&" +
                    "outFormat=json&" +
                    "location=" + encodedAddress + "&" +
                    "thumbMaps=false"
                );

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var getAddressResponse = await JsonSerializer.DeserializeAsync<GetAddressResponse>(contentStream);

            var locations = getAddressResponse.Results[0].Locations;
            foreach (Location location in locations)
            {
                if (location.LatLng.Lat != default && location.LatLng.Lng != default)
                {
                    return location.LatLng;
                }
            }
            throw new Exception("address not found:" + address);
        }
    }
}