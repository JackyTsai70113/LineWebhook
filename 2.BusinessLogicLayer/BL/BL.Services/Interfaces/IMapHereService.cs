using Core.Domain.DTO.Map;

namespace BL.Services.Interfaces {

    public interface IMapHereService {

        /// <summary>
        /// 透過兩經緯度取得距離
        /// </summary>
        /// <param name="l1">經緯度</param>
        /// <param name="l2">經緯度</param>
        /// <returns>距離</returns>
        int GetDistanceFromTwoLatLng(LatLng l1, LatLng l2);

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