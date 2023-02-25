using BL.Service.MapQuest;

namespace BL.Service.Tests.Map
{
    /// <summary>
    /// here服務的 API [Project Name: Freemium 2020-09-19]
    /// 25000 transactions per month for free
    /// 2.5GB data hub data tranfer per month for free
    /// 5GB studio and data hub database storage per month for free
    /// https://developer.here.com/documentation
    /// </summary>
    public class FakeMapQuestService : IMapQuestService
    {
        public Task<GetRouteResponse> GetRouteAsync(LatLng l1, LatLng l2)
        {
            return new Task<GetRouteResponse>(() => new GetRouteResponse());
        }

        /// <summary>
        /// 透過地址取得經緯度
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>經緯度</returns>
        public Task<LatLng> GetLatLngAsync(string address)
        {
            return new Task<LatLng>(() => new LatLng
            {
                Lat = 0,
                Lng = 0
            });
        }

        /// <summary>
        /// 將 目標地址列表 依 來源地址的遠近 排序，越近越前面
        /// </summary>
        /// <param name="sourceAddress">來源地址</param>
        /// <param name="targetAddresses">目標地址列表</param>
        /// <returns>地址列表</returns>
        public Task<List<string>> GetAddressInOrderAsync(string sourceAddress, List<string> targetAddresses)
        {
            return new Task<List<string>>(() => new List<string>());
        }
    }
}