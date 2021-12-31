using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BL.Services.Cache.Redis {

    public class RedisCacheService : ICacheService {
        private static readonly string endpoint = ConfigService.Redis_Endpoint;
        private static readonly string password = ConfigService.Redis_Password;
        private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(endpoint + ",password=" + password);
        private static readonly IDatabase db = redis.GetDatabase();
        private static readonly IServer server = redis.GetServer(endpoint);

        public RedisCacheService() {
        }

        public bool Set<T>(string key, T value) {
            string valueStr = JsonConvert.SerializeObject(value);
            RedisValue redisValue = new RedisValue(valueStr);
            return db.StringSet(key, redisValue);
        }

        public bool Set<T>(string key, T value, TimeSpan timeout) {
            string valueStr = JsonConvert.SerializeObject(value);
            RedisValue redisValue = new RedisValue(valueStr);
            return db.StringSet(key, redisValue, timeout);
        }

        /// <summary>
        /// 透過pattern取得所有符合的key (*: 任意字串)
        /// </summary>
        /// <param name="pattern">欲符合的pattern</param>
        /// <returns>key列表</returns>
        public List<string> GetKeys(string pattern) {
            return server.Keys(pattern: pattern).Select(k => k.ToString()).ToList();
        }

        public T Get<T>(string key) {
            RedisValue redisValue = db.StringGet(key);
            return JsonConvert.DeserializeObject<T>(redisValue.ToString());
        }

        public bool TryGet<T>(string key, out T value) {
            if (ExistKeyValue(key)) {
                RedisValue redisValue = db.StringGet(key);
                value = JsonConvert.DeserializeObject<T>(redisValue.ToString());
                return true;
            }

            value = default;
            return false;
        }

        public bool Delete(string key) {
            return db.KeyDelete(key);
        }

        /// <summary>
        /// KeyValue是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>是否存在</returns>
        public bool ExistKeyValue(string key) {
            return db.StringGet(key).HasValue;
        }

        public EndPoint[] GetEndPoints() {
            return redis.GetEndPoints();
        }

        public HashEntry[] GetHashEntrys(string pattern) {
            return db.HashGetAll(pattern);
        }

        public IServer GetEndPoints(EndPoint endPoint) {
            return redis.GetServer(endPoint);
        }
    }
}