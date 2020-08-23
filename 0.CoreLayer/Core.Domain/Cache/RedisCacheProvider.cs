using Core.Domain.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Cache {

    public class RedisCacheProvider {
        private readonly string config = ConfigurationUtility.RedisConfig;

        public RedisCacheProvider() {
            try {
                ConnectionMultiplexer muxer = ConnectionMultiplexer.Connect(config);
                IDatabase conn = muxer.GetDatabase();
                conn.StringSet("foo", "bar");
                var value = conn.StringGet("foo");
                Console.WriteLine(value);
            } catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }

        //RedisEndpoint _endPoint;

        //public RedisCacheProvider() {
        //    _endPoint = new RedisEndpoint(RedisConfigurationManager.Config.Host, RedisConfigurationManager.Config.Port, RedisConfigurationManager.Config.Password, RedisConfigurationManager.Config.DatabaseID);
        //}

        //public void Set<T>(string key, T value) {
        //    this.Set(key, value, TimeSpan.Zero);
        //}

        //public void Set<T>(string key, T value, TimeSpan timeout) {
        //    using (RedisClient client = new RedisClient(_endPoint)) {
        //        client.As<T>().SetValue(key, value, timeout);
        //    }
        //}

        //public T Get<T>(string key) {
        //    T result = default(T);

        //    using (RedisClient client = new RedisClient(_endPoint)) {
        //        var wrapper = client.As<T>();

        //        result = wrapper.GetValue(key);
        //    }

        //    return result;
        //}

        //public bool Remove(string key) {
        //    bool removed = false;

        //    using (RedisClient client = new RedisClient(_endPoint)) {
        //        removed = client.Remove(key);
        //    }

        //    return removed;
        //}

        //public bool IsInCache(string key) {
        //    bool isInCache = false;

        //    using (RedisClient client = new RedisClient(_endPoint)) {
        //        isInCache = client.ContainsKey(key);
        //    }

        //    return isInCache;
        //}
    }
}