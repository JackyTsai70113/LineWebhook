using Core.Domain.Entities.TWSE_Stock;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BL.Services.Cache {

    public class RedisCacheService : ICacheService {
        private static readonly string _endpoint = ConfigService.Redis_Endpoint;
        private static readonly string _password = ConfigService.Redis_Password;
        private static readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(_endpoint + ",password=" + _password);
        private static readonly IDatabase _db = _redis.GetDatabase();
        private static readonly IServer _server = _redis.GetServer(_endpoint);

        public RedisCacheService() {
        }

        public void Set<T>(string key, T value) {
            string valueStr = JsonConvert.SerializeObject(value);
            RedisValue redisValue = new RedisValue(valueStr);
            _db.StringSet(key, redisValue);
        }

        public void Set<T>(string key, T value, TimeSpan timeout) {
            string valueStr = JsonConvert.SerializeObject(value);
            RedisValue redisValue = new RedisValue(valueStr);
            _db.StringSet(key, redisValue, timeout);
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
            return JsonConvert.DeserializeObject<T>(redisValue.ToString());
        }

        public bool TryGet<T>(string key, out T value) {
            if (IsInCache(key)) {
                RedisValue redisValue = _db.StringGet(key);
                value = JsonConvert.DeserializeObject<T>(redisValue.ToString());
                return true;
            }

            value = default;
            return false;
        }

        public bool Remove(string key) {
            return _db.KeyDelete(key);
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

        public bool IsInCache(string key) {
            return _db.StringGet(key).HasValue;
        }

        public void Test() {
            //             var dd = GetKeys("*");
            //             Set("valueTest", new DividendDistribution() {
            //                 CashDividendsFromLegalReserveAndCapitalSurplus = 2.563f
            //             });
            //             Set("valueTest2", new DividendDistribution() {
            //                 CashDividendsFromLegalReserveAndCapitalSurplus = 4.869f
            //             }, new TimeSpan(0, 1, 0));
            //             DividendDistribution valueTest = Get<DividendDistribution>("valueTest");
            // 
            //             bool b1 = IsInCache("valueTest2");
            //             DividendDistribution valueTest2 = Get<DividendDistribution>("valueTest2");
            //             bool b2 = IsInCache("valueTest2");
            //             if (b2) {
            //                 DividendDistribution valueTest25 = Get<DividendDistribution>("valueTest2");
            //             }
            //             var dd123 = GetKeys("*");
            //             Remove("valueTest");
            //             var dd456 = GetKeys("*");
            //             bool b3 = IsInCache("valueTest2");
        }
    }
}