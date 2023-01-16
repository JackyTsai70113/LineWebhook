using BL.Service.Interface;
using Core.Domain.DTO.Map;
using System.Collections.Generic;

namespace BL.Service.Tests.Map {

    /// <summary>
    /// here服務的 API [Project Name: Freemium 2020-09-19]
    /// 25000 transactions per month for free
    /// 2.5GB data hub data tranfer per month for free
    /// 5GB studio and data hub database storage per month for free
    /// https://developer.here.com/documentation
    /// </summary>
    public class FakeMapHereService : IMapHereService {

        /// <summary>
        /// 建構子，設定Api Key
        /// </summary>
        public FakeMapHereService() {
        }

        /// <summary>
        /// 將 目標地址列表 依 來源地址的遠近 排序，越近越前面
        /// </summary>
        /// <param name="sourceAddress">來源地址</param>
        /// <param name="targetAddresses">目標地址列表</param>
        /// <returns>地址列表</returns>
        public List<string> GetAddressInOrder(string sourceAddress, List<string> targetAddresses) {
            return targetAddresses;
        }

        /// <summary>
        /// 透過兩經緯度取得旅程時間(分)
        /// </summary>
        /// <param name="l1">經緯度</param>
        /// <param name="l2">經緯度</param>
        /// <returns>旅程時間(分)</returns>
        public int GetTravelTimeFromTwoLatLngs(LatLng l1, LatLng l2) {
            return 0;
        }

        /// <summary>
        /// 透過地址取得經緯度
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>經緯度</returns>
        public LatLng GetLatLngFromAddress(string address) {

            LatLng latLng = new LatLng() {
                lat = 0,
                lng = 0
            };

            return latLng;
        }
    }
}