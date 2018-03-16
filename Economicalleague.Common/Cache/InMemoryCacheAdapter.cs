using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace Economicalleague.Common.Cache
{
    class InMemoryCacheAdapter : ICacheAdapter
    {
        protected static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public void Set(string key, object value)
        {
            if (value != null)
            {
                _cache.Set(key, value);
            }
        }

        public void Set(string key, object value, TimeSpan slidingExpiration)
        {
            if (value != null)
            {
                _cache.Set(key, value, slidingExpiration);
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public T Get<T>(string key)
        {
            return (T)Get(key);
        }

        public bool Exist(string key)
        {
            return _cache.Get(key) != null;
        }
    }
}
