namespace BL.Service.MapQuest;

public interface IMapQuestService
{
    public Task<int> GetDurationAsync(LatLng l1, LatLng l2);

    /// <summary>
    /// 透過地址取得經緯度
    /// </summary>
    /// <param name="address">地址</param>
    /// <returns>經緯度</returns>
    public Task<LatLng> GetLatLngAsync(string address);

    /// <summary>
    /// 將 目標地址列表 依 來源地址的遠近 排序，越近越前面
    /// </summary>
    /// <param name="sourceAddress">來源地址</param>
    /// <param name="targetAddresses">目標地址列表</param>
    /// <returns>地址列表</returns>
    public Task<List<string>> GetAddressInOrderAsync(string sourceAddress, List<string> targetAddresses);
}