using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services.Cache {

    public interface ICacheService {

        void Set<T>(string key, T value);

        void Set<T>(string key, T value, TimeSpan timeout);

        T Get<T>(string key);

        bool Remove(string key);

        bool IsInCache(string key);
    }
}