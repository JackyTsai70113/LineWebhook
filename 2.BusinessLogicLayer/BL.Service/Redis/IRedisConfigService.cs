#nullable enable
namespace BL.Service.Redis;

/// <summary>
/// Redis 設定服務，用於集中管理應用程式設定。
/// Key 格式: config:{env}:{key}，例如 config:prod:Line:ChannelAccessToken
/// </summary>
public interface IRedisConfigService
{
    /// <summary>
    /// 取得設定值（字串）
    /// </summary>
    /// <param name="key">設定 key</param>
    /// <returns>設定值，不存在時回傳 null</returns>
    string? Get(string key);

    /// <summary>
    /// 取得設定值並反序列化為 T
    /// </summary>
    /// <typeparam name="T">目標型別</typeparam>
    /// <param name="key">設定 key</param>
    /// <returns>反序列化後的物件，不存在時回傳 default(T)</returns>
    T? Get<T>(string key);

    /// <summary>
    /// 寫入設定值（字串）
    /// </summary>
    /// <param name="key">設定 key</param>
    /// <param name="value">設定值</param>
    /// <returns>是否成功</returns>
    bool Set(string key, string value);

    /// <summary>
    /// 寫入設定值（物件，會序列化為 JSON）
    /// </summary>
    /// <typeparam name="T">來源型別</typeparam>
    /// <param name="key">設定 key</param>
    /// <param name="value">設定值</param>
    /// <returns>是否成功</returns>
    bool Set<T>(string key, T value);

    /// <summary>
    /// 刪除設定
    /// </summary>
    /// <param name="key">設定 key</param>
    /// <returns>是否成功</returns>
    bool Delete(string key);

    /// <summary>
    /// 檢查設定是否存在
    /// </summary>
    /// <param name="key">設定 key</param>
    /// <returns>是否存在</returns>
    bool Exists(string key);

    /// <summary>
    /// 取得目前環境下所有設定 key
    /// </summary>
    /// <returns>key 列表</returns>
    IEnumerable<string> GetAllKeys();
}
