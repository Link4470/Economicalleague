using System;
using System.Collections.Generic;
using System.Text;

namespace Economicalleague.Common.Cache
{
    public interface ICacheAdapter
    {
        T Get<T>(string key);

        object Get(string key);

        bool Exist(string key);

        void Set(string key, object data);

        void Set(string key, object data, TimeSpan slidingExpiration);

        void Remove(string key);
    }

    public interface ICache
    {
        T Get<T>(CacheKey key, Func<T> loadFunc);
        bool Exist(CacheKey key);
        void Remove(CacheKey key);
        void Refresh(CacheKey key, object changedInstance);
    }
}
