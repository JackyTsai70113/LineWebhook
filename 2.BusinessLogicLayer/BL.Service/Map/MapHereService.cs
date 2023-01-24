using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using BL.Service.Interface;
using Core.Domain.DTO.Map;
using Core.Domain.Utilities;
using Microsoft.Extensions.Configuration;

namespace BL.Service.Map {

    /// <summary>
    /// here服務的 API [Project Name: Freemium 2020-09-19]
    /// 25000 transactions per month for free
    /// 2.5GB data hub data tranfer per month for free
    /// 5GB studio and data hub database storage per month for free
    /// https://developer.here.com/documentation
    /// </summary>
    public class MapHereService : IMapHereService {

        /// <summary>
        /// here 的 Api key
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// 建構子，設定Api Key
        /// </summary>
        public MapHereService(IConfiguration config) {
            _apiKey = config["HereApi_Key"];
        }

        /// <summary>
        /// 將 目標地址列表 依 來源地址的遠近 排序，越近越前面
        /// </summary>
        /// <param name="sourceAddress">來源地址</param>
        /// <param name="targetAddresses">目標地址列表</param>
        /// <returns>地址列表</returns>
        public List<string> GetAddressInOrder(string sourceAddress, List<string> targetAddresses) {
            LatLng sourceLatLng = GetLatLngFromAddress(sourceAddress);
            List<string> orderedAddresses = targetAddresses.OrderBy(target => {
                LatLng targetLatLng = GetLatLngFromAddress(target);
                return GetTravelTimeFromTwoLatLngs(sourceLatLng, targetLatLng);
            }).ToList();
            return orderedAddresses;
        }

        /// <summary>
        /// 透過兩經緯度取得距離
        /// </summary>
        /// <param name="l1">經緯度</param>
        /// <param name="l2">經緯度</param>
        /// <returns>距離</returns>
        public int GetDistanceFromTwoLatLngs(LatLng l1, LatLng l2) {
            string uri = "https://route.ls.hereapi.com/routing/7.2/calculateroute.json" +
                "?apiKey=" + _apiKey +
                "&waypoint0=geo!" + l1.lat + "," + l1.lng +
                "&waypoint1=geo!" + l2.lat + "," + l2.lng +
                "&mode=fastest;pedestrian";
            string responseStr = RequestUtility.GetStringFromGetRequest(uri);
            var calculateRouteRootobject = JsonSerializer.Deserialize<CalculateRouteRootobject>(responseStr);
            return calculateRouteRootobject.response.route[0].summary.distance;
        }

        /// <summary>
        /// 透過兩經緯度取得旅程時間(分)
        /// </summary>
        /// <param name="l1">經緯度</param>
        /// <param name="l2">經緯度</param>
        /// <returns>旅程時間(分)</returns>
        public int GetTravelTimeFromTwoLatLngs(LatLng l1, LatLng l2) {
            string uri = "https://route.ls.hereapi.com/routing/7.2/calculateroute.json" +
                "?apiKey=" + _apiKey +
                "&waypoint0=geo!" + l1.lat + "," + l1.lng +
                "&waypoint1=geo!" + l2.lat + "," + l2.lng +
                "&mode=fastest;pedestrian";
            string responseStr = RequestUtility.GetStringFromGetRequest(uri);
            var calculateRouteRootobject = JsonSerializer.Deserialize<CalculateRouteRootobject>(responseStr);
            return calculateRouteRootobject.response.route[0].summary.travelTime;
        }

        /// <summary>
        /// 透過地址取得經緯度
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>經緯度</returns>
        public LatLng GetLatLngFromAddress(string address) {
            string addressWithCountry = "臺灣" + address;

            string uri = "https://geocoder.ls.hereapi.com/6.2/geocode.json" +
                "?apiKey=" + _apiKey +
                "&searchtext=" + addressWithCountry +
                "&gen=9";
            string responseStr = RequestUtility.GetStringFromGetRequest(uri);
            RootObject rootObject = JsonSerializer.Deserialize<RootObject>(responseStr);

            LatLng latLng = new LatLng {
                lat = rootObject.Response.View[0].Result[0].Location.DisplayPosition.Latitude,
                lng = rootObject.Response.View[0].Result[0].Location.DisplayPosition.Longitude
            };

            return latLng;
        }

        private class RootObject {
            public Response Response { get; set; }
        }

        private class Response {
            public Metainfo MetaInfo { get; set; }
            public View[] View { get; set; }
        }

        private class Metainfo {
            public string Timestamp { get; set; }
        }

        private class View {
            public string _type { get; set; }
            public int ViewId { get; set; }
            public Result[] Result { get; set; }
        }

        private class Result {
            public float Relevance { get; set; }
            public string MatchLevel { get; set; }
            public Matchquality MatchQuality { get; set; }
            public string MatchType { get; set; }
            public Location Location { get; set; }
        }

        private class Matchquality {
            public float Country { get; set; }
            public float City { get; set; }
            public float District { get; set; }
            public float[] Street { get; set; }
            public float HouseNumber { get; set; }
            public float PostalCode { get; set; }
        }

        private class Location {
            public string LocationId { get; set; }
            public string LocationType { get; set; }
            public Displayposition DisplayPosition { get; set; }
            public Navigationposition[] NavigationPosition { get; set; }
            public Mapview MapView { get; set; }
            public Address Address { get; set; }
        }

        private class Displayposition {
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }

        private class Mapview {
            public Topleft TopLeft { get; set; }
            public Bottomright BottomRight { get; set; }
        }

        private class Topleft {
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }

        private class Bottomright {
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }

        private class Address {
            public string Label { get; set; }
            public string Country { get; set; }
            public string County { get; set; }
            public string City { get; set; }
            public string District { get; set; }
            public string Street { get; set; }
            public string HouseNumber { get; set; }
            public string PostalCode { get; set; }
            public Additionaldata[] AdditionalData { get; set; }
        }

        private class Additionaldata {
            public string value { get; set; }
            public string key { get; set; }
        }

        private class Navigationposition {
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }
    }
}