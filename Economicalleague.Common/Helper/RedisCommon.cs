using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using System.Configuration;
using ServiceStack.Redis.Generic;
using Newtonsoft.Json;

namespace Economicalleague.Common
{
    public class RedisCommon
    {
        private static readonly Lazy<RedisCommon> _instance = new Lazy<RedisCommon>(() => new RedisCommon());
        private static readonly string redisUrl = ConfigurationManager.AppSettings["Redis_Server"];
        private static readonly string redisPort = ConfigurationManager.AppSettings["Redis_Port"];
        private static readonly string redisPwd = ConfigurationManager.AppSettings["Redis_Pwd"];
        private static readonly bool redisSsl = ConfigurationHelper.GetBool("Redis_Ssl");
        private RedisCommon()
        {

        }
        public static RedisCommon getInstance
        {
            get
            {

                return _instance.Value;
            }
        }

        public IRedisClient getRedisClient()
        {
            if(string.IsNullOrEmpty(redisPwd))
            {
                return new RedisClient(redisUrl, int.Parse(redisPort));
            }
            else
            {
                return new RedisClient(redisUrl, int.Parse(redisPort), redisPwd);
            }
        }

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        public bool Exist<T>(string hashId, string key)
        {
            bool result = false;
            using (var redis = this.getRedisClient())
            {
                result = redis.HashContainsEntry(hashId, key);
            }
            return result;
        }
        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        public bool Set<T>(string hashId, string key, T t)
        {
            bool result = false;
            try
            {
                using (var redis = this.getRedisClient())
                {
                    var value = JsonConvert.SerializeObject(t);
                    result = redis.SetEntryInHash(hashId, key, value);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Redis存储数据到hash表失败", ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        public bool ExistObj<T>(string key)
        {
            bool result = false;
            using (var redis = this.getRedisClient())
            {
                result = redis.ContainsKey(key);
            }
            return result;
        }

        /// <summary>
        /// 存储对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="val">存储的对象</param>
        /// <returns></returns>
        public bool SetObj<T>(string key, T val)
        {
            bool result = false;
            try
            {
                using (var redis = this.getRedisClient())
                {
                    result = redis.Set<T>(key, val);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Redis存储对象失败", ex);
                result = false;
            }
            return result;
        }

        public bool SetObj<T>(string key, T val, DateTime dateTime)
        {
            bool result = false;
            try
            {
                using (var redis = this.getRedisClient())
                {
                    result = redis.Set<T>(key, val, dateTime);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Redis存储对象(含过期时间)失败", ex);
                result = false;
            }
            return result;
        }

        public T GetObj<T>(string key)
        {
            T result;
            try
            {
                using (var redis = this.getRedisClient())
                {
                    result = redis.Get<T>(key);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Redis获取对象失败", ex);
                throw ex;
            }
            return result;
        }

        public bool UpdateStr(string key, string val)
        {

            bool result = false;
            try
            {
                using (var redis = this.getRedisClient())
                {
                    result = redis.Set<string>(key, val);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Redis修改字符串失败", ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 移除Redis中的某值
        /// </summary>
        public bool RemoveObj(string key)
        {
            bool result = false;
            try
            {
                using (var redis = this.getRedisClient())
                {
                    result = redis.Remove(key);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Redis移除某值失败", ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 根据条件获取keys
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public IList<string> GetKeysByCondition(Func<string, bool> predicate)
        {
            using (var redis = this.getRedisClient())
            {
                IList<string> result = new List<string>();
                var list = redis.GetAllKeys().Where(predicate).ToList();
                if (list != null && list.Count > 0)
                {
                    result = list;
                }
                return result;
            }
        }

        /// <summary>
        /// 删除指定的key对象
        /// </summary>
        /// <param name="keys">key集合</param>
        /// <returns></returns>
        public bool RemoveObjectList(IList<string> keys)
        {
            bool result = false;
            try
            {
                if (keys.Count > 0)
                {
                    using (var redis = this.getRedisClient())
                    {
                        redis.RemoveAll(keys);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Redis删除指定的key对象失败", ex);
                result = false;
            }
            return result;
        }

        public bool Update<T>(string key, T t)
        {
            bool result = false;
            using (var redis = this.getRedisClient())
            {
                result = redis.Set<T>(key, t);
            }
            return result;
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        public bool Remove(string hashId, string key)
        {
            bool result = false;
            try
            {
                using (var redis = this.getRedisClient())
                {
                    result = redis.RemoveEntryFromHash(hashId, key);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Redis移除hash中的某值失败", ex);
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 移除整个hash
        /// </summary>
        public bool RemoveKey(string key)
        {
            bool result = false;
            using (var redis = this.getRedisClient())
            {
                result = redis.Remove(key);
            }
            return result;

        }
        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        public T Get<T>(string hashId, string key)
        {
            using (var redis = this.getRedisClient())
            {
                string value = redis.GetValueFromHash(hashId, key);

                return JsonConvert.DeserializeObject<T>(value);
            }
        }

        public string GetObjStr(string key)
        {
            using (var redis = this.getRedisClient())
            {
                return redis.Get<string>(key);
            }
        }

        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        public List<T> GetAll<T>(string hashId)
        {
            using (var redis = this.getRedisClient())
            {
                var result = new List<T>();
                var list = redis.GetHashValues(hashId);
                if (list != null && list.Count > 0)
                {
                    list.ForEach(x =>
                    {
                        var value = JsonConvert.DeserializeObject<T>(x);
                        result.Add(value);
                    });
                }
                return result;
            }
        }
        /// <summary>
        /// 设置缓存过期
        /// </summary>
        public void SetExpire(string key, DateTime datetime)
        {
            using (var redis = this.getRedisClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }
    }
}
