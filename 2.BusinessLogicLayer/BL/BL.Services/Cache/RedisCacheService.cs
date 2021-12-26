using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BL.Services.Cache {

    public class RedisCacheService : ICacheService {
        private static readonly string endpoint = ConfigService.Redis_Endpoint;
        private static readonly string password = ConfigService.Redis_Password;
        private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(endpoint + ",password=" + password);
        private static readonly IDatabase db = redis.GetDatabase();
        private static readonly IServer server = redis.GetServer(endpoint);

        public RedisCacheService() {
        }

        public void Set<T>(string key, T value) {
            string valueStr = JsonConvert.SerializeObject(value);
            RedisValue redisValue = new RedisValue(valueStr);
            db.StringSet(key, redisValue);
        }

        public void Set<T>(string key, T value, TimeSpan timeout) {
            string valueStr = JsonConvert.SerializeObject(value);
            RedisValue redisValue = new RedisValue(valueStr);
            db.StringSet(key, redisValue, timeout);
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
            if (IsInCache(key)) {
                RedisValue redisValue = db.StringGet(key);
                value = JsonConvert.DeserializeObject<T>(redisValue.ToString());
                return true;
            }

            value = default;
            return false;
        }

        public bool Remove(string key) {
            return db.KeyDelete(key);
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

        public bool IsInCache(string key) {
            return db.StringGet(key).HasValue;
        }
    }
}