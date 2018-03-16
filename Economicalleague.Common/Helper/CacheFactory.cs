using System;
using System.Web;
using System.Web.Caching;

namespace Economicalleague.Common
{
    public static class CacheFactory
    {
        private static volatile IRedisBase _redisInstance;
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Redis访问接口调用(单例)
        /// </summary>
        public static IRedisBase Redis
        {
            get
            {
                if (_redisInstance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_redisInstance == null)
                        {
                            _redisInstance = new RedisHelper();
                        }
                    }
                }
                return _redisInstance;
            }
        }

        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache对象值
        /// </summary>
        /// <param name="cacheKey">索引键值</param>
        /// <returns>返回缓存对象</returns>
        public static object GetApplicationCache(string cacheKey)
        {
            try
            {
                var objCache = HttpRuntime.Cache;
                return objCache[cacheKey];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 设置以缓存依赖的方式缓存数据
        /// </summary>
        /// <param name="cacheKey">索引键值</param>
        /// <param name="objObject">缓存对象</param>
        /// <param name="dep">依赖对象</param>
        //public static void SetApplicationCache(string cacheKey, object objObject, CacheDependency dep)
        //{
        //    var objCache = HttpRuntime.Cache;
        //    objCache.Insert(
        //        cacheKey,
        //        objObject,
        //        dep,
        //        Cache.NoAbsoluteExpiration, //从不过期
        //        Cache.NoSlidingExpiration,  //禁用可调过期
        //        CacheItemPriority.Default,
        //        null);
        //}
    }
}
