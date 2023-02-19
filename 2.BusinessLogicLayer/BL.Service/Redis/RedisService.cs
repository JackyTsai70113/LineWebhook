using System.Net;
using System.Text.Json;
using StackExchange.Redis;

namespace BL.Service.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redis;

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    public bool Set<T>(string key, T value)
    {
        string valueStr = JsonSerializer.Serialize(value);
        RedisValue redisValue = new(valueStr);
        return _db.StringSet(key, redisValue);
    }

    public bool Set<T>(string key, T value, TimeSpan timeout)
    {
        string valueStr = JsonSerializer.Serialize(value);
        RedisValue redisValue = new(valueStr);
        return _db.StringSet(key, redisValue, timeout);
    }

    /// <summary>
    /// 透過pattern取得所有符合的key (*: 任意字串)
    /// </summary>
    /// <param name="pattern">欲符合的pattern</param>
    /// <returns>key列表</returns>
    public IEnumerable<string> GetKeys(string pattern)
    {
        var endPoints = _redis.GetEndPoints();
        foreach (var endPoint in endPoints)
        {
            var server = _redis.GetServer(endPoint);
            var keys = server.Keys(pattern: pattern);
            foreach (var key in keys)
            {
                yield return key;
            }
        }
    }

    public T Get<T>(string key)
    {
        RedisValue redisValue = _db.StringGet(key);
        return JsonSerializer.Deserialize<T>(redisValue.ToString());
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (ExistKeyValue(key))
        {
            RedisValue redisValue = _db.StringGet(key);
            value = JsonSerializer.Deserialize<T>(redisValue.ToString());
            return true;
        }

        value = default;
        return false;
    }

    public bool Delete(string key)
    {
        return _db.KeyDelete(key);
    }

    /// <summary>
    /// KeyValue是否存在
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>是否存在</returns>
    public bool ExistKeyValue(string key)
    {
        return _db.StringGet(key).HasValue;
    }

    public EndPoint[] GetEndPoints()
    {
        return _redis.GetEndPoints();
    }

    public HashEntry[] GetHashEntrys(string pattern)
    {
        return _db.HashGetAll(pattern);
    }

    public IServer GetEndPoints(EndPoint endPoint)
    {
        return _redis.GetServer(endPoint);
    }
}