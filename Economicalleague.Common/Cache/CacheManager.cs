using System;
using System.Collections.Generic;
using System.Text;

namespace Economicalleague.Common.Cache
{
    public class CacheManager : ICache
    {
        private ICacheAdapter _defaultCache;
        // private ICacheAdapter _inMemCache;
        private static CacheManager _instance;

        private CacheManager()
        {
            _defaultCache = new InMemoryCacheAdapter();
            // _inMemCache = _defaultCache;
        }

        public static CacheManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new CacheManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public T Get<T>(CacheKey key, Func<T> loadFunc)
        {
            var cache = _defaultCache;
            var data = cache.Get<T>(key.UniqueKey);
            if (data == null)
            {
                lock (key)
                {
                    data = loadFunc();
                    if (data != null)
                    {
                        cache.Set(key.UniqueKey, data, key.SlidingExpiration);
                    }
                }
            }
            return data;
        }

        public T Get<T>(CacheKey key)
        {
            var cache = _defaultCache;
            var data = cache.Get<T>(key.UniqueKey);
            return data;
        }

        public void Set<T>(CacheKey key, T data)
        {
            if (data == null) return;
            lock (key)
            {
                _defaultCache.Remove(key.UniqueKey);
                _defaultCache.Set(key.UniqueKey, data, key.SlidingExpiration);
            }
        }

        public bool Exist(CacheKey key)
        {
            var cache = _defaultCache;
            return cache.Exist(key.UniqueKey);
        }

        public void Remove(CacheKey key)
        {
            var cache = _defaultCache;
            cache.Remove(key.UniqueKey);
        }

        public void Refresh(CacheKey key, object changedInstance)
        {
            throw new NotImplementedException();
        }

        private static readonly object Locker = new object();
    }
}
