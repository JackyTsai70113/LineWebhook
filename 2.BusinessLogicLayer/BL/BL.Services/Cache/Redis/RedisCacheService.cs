using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace BL.Services.Cache.Redis {

    public class RedisCacheService : ICacheService {
        private readonly string _endpoint;
        private readonly string _password;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly IServer _server;

        public RedisCacheService(string endpoint, string password) {
            _endpoint = endpoint;
            _password = password;
            _redis = ConnectionMultiplexer.Connect(_endpoint + ",password=" + _password);
            _db = _redis.GetDatabase();
            _server = _redis.GetServer(_endpoint);
        }

        public bool Set<T>(string key, T value) {
            string valueStr = JsonSerializer.Serialize(value);
            RedisValue redisValue = new RedisValue(valueStr);
            return _db.StringSet(key, redisValue);
        }

        public bool Set<T>(string key, T value, TimeSpan timeout) {
            string valueStr = JsonSerializer.Serialize(value);
            RedisValue redisValue = new RedisValue(valueStr);
            return _db.StringSet(key, redisValue, timeout);
        }

        /// <summary>
        /// 透過pattern取得所有符合的key (*: 任意字串)
        /// </summary>
        /// <param name="pattern">欲符合的pattern</param>
        /// <returns>key列表</returns>
        public List<string> GetKeys(string pattern) {
            return _server.Keys(pattern: pattern).Select(k => k.ToString()).ToList();
        }

        public T Get<T>(string key) {
            RedisValue redisValue = _db.StringGet(key);
            return JsonSerializer.Deserialize<T>(redisValue.ToString());
        }

        public bool TryGet<T>(string key, out T value) {
            if (ExistKeyValue(key)) {
                RedisValue redisValue = _db.StringGet(key);
                value = JsonSerializer.Deserialize<T>(redisValue.ToString());
                return true;
            }

            value = default;
            return false;
        }

        public bool Delete(string key) {
            return _db.KeyDelete(key);
        }

        /// <summary>
        /// KeyValue是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>是否存在</returns>
        public bool ExistKeyValue(string key) {
            return _db.StringGet(key).HasValue;
        }

        public EndPoint[] GetEndPoints() {
            return _redis.GetEndPoints();
        }

        public HashEntry[] GetHashEntrys(string pattern) {
            return _db.HashGetAll(pattern);
        }

        public IServer GetEndPoints(EndPoint endPoint) {
            return _redis.GetServer(endPoint);
        }
    }
}