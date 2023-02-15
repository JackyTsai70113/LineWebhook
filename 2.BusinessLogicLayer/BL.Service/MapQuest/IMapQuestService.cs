namespace BL.Service.MapQuest;

/// <summary>
/// MapQuest服務的Map API，15000 transactions per month for free
/// https://developer.mapquest.com/documentation/
/// </summary>
public interface IMapQuestService
{
    public int GetDuration(Core.Domain.DTO.Map.LatLng l1, Core.Domain.DTO.Map.LatLng l2);
    /// <summary>
    /// 透過地址取得經緯度
    /// </summary>
    /// <param name="address">地址</param>
    /// <returns>經緯度</returns>
    public Core.Domain.DTO.Map.LatLng GetLatLngFromAddress(string address);

    /// <summary>
    /// 將 目標地址列表 依 來源地址的遠近 排序，越近越前面
    /// </summary>
    /// <param name="sourceAddress">來源地址</param>
    /// <param name="targetAddresses">目標地址列表</param>
    /// <returns>地址列表</returns>
    public List<string> GetAddressInOrder(string sourceAddress, List<string> targetAddresses);
}