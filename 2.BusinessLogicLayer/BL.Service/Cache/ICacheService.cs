namespace BL.Service.Cache;

public interface ICacheService
{
    bool Set<T>(string key, T value);

    bool Set<T>(string key, T value, TimeSpan timeout);

    /// <summary>
    /// 透過pattern取得所有符合的key (*: 任意字串)
    /// </summary>
    /// <param name="pattern">欲符合的pattern</param>
    /// <returns>key列表</returns>
    IEnumerable<string> GetKeys(string pattern);

    T Get<T>(string key);

    bool Delete(string key);

    bool ExistKeyValue(string key);
}