#nullable enable
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BL.Service.Redis;

/// <summary>
/// Redis 設定服務實作。
/// Key 格式: config:{env}:{key}
/// env 由環境變數 ASPNETCORE_ENVIRONMENT 決定：
///   - Production  → prod
///   - 其他 (Development/Staging) → dev
/// </summary>
public class RedisConfigService : IRedisConfigService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisConfigService> _logger;
    private readonly string _envPrefix;

    public RedisConfigService(
        IConnectionMultiplexer redis,
        ILogger<RedisConfigService> logger,
        IConfiguration config)
    {
        _redis = redis;
        _db = redis.GetDatabase();
        _logger = logger;

        var env = config["ASPNETCORE_ENVIRONMENT"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        _envPrefix = env == "Production" ? "prod" : "dev";
    }

    private string BuildKey(string key) => $"config:{_envPrefix}:{key}";

    public string? Get(string key)
    {
        try
        {
            var redisKey = BuildKey(key);
            RedisValue value = _db.StringGet(redisKey);
            if (value.IsNullOrEmpty)
            {
                _logger.LogDebug("Redis config key not found: {Key}", redisKey);
                return null;
            }
            return value.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 Redis config 失敗, key: {Key}", key);
            return null;
        }
    }

    public T? Get<T>(string key)
    {
        var value = Get(key);
        if (value is null) return default;

        try
        {
            // 若 T 是基本型別（string, int, bool 等），直接轉換
            var targetType = typeof(T);
            if (targetType == typeof(string))
                return (T)(object)value;
            if (targetType.IsPrimitive || targetType == typeof(decimal))
                return (T)Convert.ChangeType(value, targetType);

            // 複雜型別用 JSON 反序列化
            return JsonSerializer.Deserialize<T>(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "反序列化 Redis config 失敗, key: {Key}, value: {Value}", key, value);
            return default;
        }
    }

    public bool Set(string key, string value)
    {
        try
        {
            var redisKey = BuildKey(key);
            return _db.StringSet(redisKey, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "寫入 Redis config 失敗, key: {Key}", key);
            return false;
        }
    }

    public bool Set<T>(string key, T value)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(value);
            return Set(key, serialized);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "序列化並寫入 Redis config 失敗, key: {Key}", key);
            return false;
        }
    }

    public bool Delete(string key)
    {
        try
        {
            var redisKey = BuildKey(key);
            return _db.KeyDelete(redisKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除 Redis config 失敗, key: {Key}", key);
            return false;
        }
    }

    public bool Exists(string key)
    {
        try
        {
            var redisKey = BuildKey(key);
            return _db.KeyExists(redisKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查 Redis config 是否存在失敗, key: {Key}", key);
            return false;
        }
    }

    public IEnumerable<string> GetAllKeys()
    {
        var pattern = $"config:{_envPrefix}:*";
        var prefix = $"config:{_envPrefix}:";
        var endPoints = _redis.GetEndPoints();
        foreach (var endPoint in endPoints)
        {
            var server = _redis.GetServer(endPoint);
            foreach (var key in server.Keys(pattern: pattern))
            {
                // 回傳不含 prefix 的 key
                yield return key.ToString().Replace(prefix, "");
            }
        }
    }
}
