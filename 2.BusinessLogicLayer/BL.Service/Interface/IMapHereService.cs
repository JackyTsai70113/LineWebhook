using System.Collections.Generic;
using Core.Domain.DTO.Map;

namespace BL.Service.Interface {

    public interface IMapHereService {
        /// <summary>
        /// 將 目標地址列表 依 來源地址的遠近 排序，越近越前面
        /// </summary>
        /// <param name="sourceAddress">來源地址</param>
        /// <param name="targetAddresses">目標地址列表</param>
        /// <returns>地址列表</returns>
        List<string> GetAddressInOrder(string sourceAddress, List<string> targetAddresses);

        /// <summary>
        /// 透過兩經緯度取得旅程時間(分)
        /// </summary>
        /// <param name="l1">經緯度</param>
        /// <param name="l2">經緯度</param>
        /// <returns>旅程時間(分)</returns>
        int GetTravelTimeFromTwoLatLngs(LatLng l1, LatLng l2);

        /// <summary>
        /// 透過地址取得經緯度
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>經緯度</returns>
        LatLng GetLatLngFromAddress(string address);
    }
}