using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services.Cache {

    public interface ICacheService {

        void Set<T>(string key, T value);

        void Set<T>(string key, T value, TimeSpan timeout);

        /// <summary>
        /// 透過pattern取得所有符合的key (*: 任意字串)
        /// </summary>
        /// <param name="pattern">欲符合的pattern</param>
        /// <returns>key列表</returns>
        List<string> GetKeys(string pattern);

        T Get<T>(string key);

        bool Remove(string key);

        bool IsInCache(string key);
    }
}