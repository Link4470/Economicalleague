using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace Economicalleague.Common
{
    public interface IRedisBase
    {
        #region -- SetALL && GetALL --

        void SetAll<T>(Dictionary<string, T> a);
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);

        #endregion

        #region-- Key --

        long ItemIncr(string key);
        long ItemIncrBy(string key, int incr);
        long ItemDecr(string key);
        bool ItemContain(string key);
        bool ItemSet<T>(string key, T t);
        T ItemGet<T>(string key);
        bool ItemRemove(string key);
        bool ItemSetExpire(string key, TimeSpan ts);
        bool ItemSetExpire(string key, DateTime datetime);
        long ItemExists(string key);


        #endregion

        #region -- List --

        void ListAdd<T>(string key, T t);
        bool ListRemove<T>(string key, T t);
        void ListRemoveAll<T>(string key);
        long ListCount(string key);
        List<T> ListGetRange<T>(string key, int start, int count);
        List<T> ListGetList<T>(string key);
        List<T> ListGetList<T>(string key, int pageIndex, int pageSize);
        int ListGetIndex<T>(string key, T value);
        bool ListSetExpire(string key, TimeSpan ts);
        bool ListSetExpire(string key, DateTime datetime);
        void ListAddRange(string key, List<string> values);

        #endregion

        #region -- List Queue --

        void Enqueue<T>(string key, T value);
        T Dequeue<T>(string key);
        T BlockingDequeue<T>(string key);

        #endregion

        #region -- List Stack --

        long LPush(string key, object value);
        string LPop(string key);
        bool LTrim(string key, int start, int stop);

        #endregion

        #region -- Hash --

        void HashSetRange<T>(string hashId, Dictionary<string, T> pairs);
        bool HashSet(string hashId, string key, string value);
        string HashGet(string hashId, string key);
        //T HashGet<T>(string hashId) where T : class, new();
        List<string> HashGetValues(string hashId, params string[] keys);
        Dictionary<string, string> HashGetAll(string hashid);
        T HashGetEntity<T>(string hashId) where T : class;
        long HashGetLen(string hashid);
        bool HashRemove(string hashid, string key);
        bool HashExist(string hashid, string key);
        bool HashSetExpire(string key, TimeSpan ts);
        bool HashSetExpire(string dataKey, DateTime datetime);
        long HashSetIncre(string hashId, string key, int increment);
        List<string> HashSearchKeys(string key);
        List<string> HashSearchValues(string key);

        #endregion

        #region -- Set --

        void AddItemToSet(string setId, string value);
        void RemoveItemFromSet(string setId, string item);
        HashSet<string> GetAllItemsFromSet(string setId);
        bool SetContainsItem(string setId, string item);

        #endregion

        #region -- SortedSet --

        bool SortedSetContainsItem<T>(string key, T value);
        void SetSortedListScore<T>(string key, T t, double score);
        void SortedSetAdd<T>(string key, T t);
        void SortedSetAdd<T>(string key, T t, double score);
        bool SortedSetAdd(string key, string value, double score);
        bool RemoveItemFromSortedSet<T>(string key, T t);
        long RemoveRangeFromSortedSetByScore<T>(string key, double minScore, double maxScore);
        long RemoveRangeFromSortedSet<T>(string key, int start, int stop);
        long SortedSetCount<T>(string key);
        List<T> SortedSetGetPageListByIndex<T>(string key, int pageIndex, int pageSize);
        List<T> SortedSetGetPageListDescByIndex<T>(string key, int pageIndex, int pageSize);
        List<T> SortedSetGetListByIndex<T>(string key, int fromIndex, int toIndex);
        List<T> SortedSetGetListDescByIndex<T>(string key, int fromIndex, int toIndex);
        List<T> SortedSetGetListByScore<T>(string key, double minScore, double maxScore);
        List<T> SortedSetGetListDescByScore<T>(string key, double minScore, double maxScore);

        List<T> SortedSetGetListByScore<T>(string key, double minScore, double maxScore, int skip, int take);
        List<T> SortedSetGetListDescByScore<T>(string key, double minScore, double maxScore, int skip, int take);

        List<T> SortedSetGetListAll<T>(string key);
        List<T> SortedSetGetListAllDesc<T>(string key);

        IDictionary<T, double> SortedListAllWithScore<T>(string key);

        IDictionary<T, double> SortedListDescWithScoreByScore<T>(string key, double minScore, double maxScore, int skip, int take);
        IDictionary<T, double> SortedListWithScoreByScore<T>(string key, double minScore, double maxScore, int skip, int take);

        IDictionary<T, double> SortedListDescWithScoreByIndex<T>(string key, int pageIndex, int pageSize);
        IDictionary<T, double> SortedListWithScoreByIndex<T>(string key, int pageIndex, int pageSize);

        double SortedScoreByItem<T>(string key, T t);

        bool SortedSetSetExpire(string key, TimeSpan ts);
        bool SortedSetSetExpire(string key, DateTime datetime);

        IDictionary<T, double> GetItemSortedSetByIndex<T>(string key, int fromRank, int toRank);
        long GetItemIndexInSortedSet<T>(string key, T value);
        long GetItemIndexInSortedSetDesc<T>(string key, T value);
        T SortedSetGetValueByIndex<T>(string key, int index);
        IDictionary<T, double> GetAllWithScoresFromSortedSet<T>(string key);
        #endregion

        bool ExecTrans(Action<IRedisClient>[] actions, string[] watchKeys = null);

        #region Misc

        string RandomKey();
        List<string> SearchKeys(string key);

        #endregion
    }

    public class RedisHelper : IRedisBase
    {
        #region -- 连接信息 --

        private static readonly string redisUrl = ConfigurationManager.AppSettings["Redis_Server"];
        private static readonly string redisPort = ConfigurationManager.AppSettings["Redis_Port"];
        private static readonly string redisPwd = ConfigurationManager.AppSettings["Redis_Pwd"];
        private static readonly string redisHost = $"{redisPwd}@{redisUrl}:{redisPort}";

        private static PooledRedisClientManager _prcm;

        static RedisHelper()
        {
            _prcm = CreateManager(redisHost);
        }

        private static PooledRedisClientManager CreateManager(string host)
        {
            var config = new RedisClientManagerConfig
            {
                AutoStart = true,
                MaxReadPoolSize = 5000,
                MaxWritePoolSize = 5000
            };

            //超时时间都是毫秒
            var prcm = new PooledRedisClientManager(new[] { host }, new[] { host }, config)
            {
                PoolTimeout = 10000,
                ConnectTimeout = 10000,
                SocketSendTimeout = 10000,
                SocketReceiveTimeout = 10000
            };

            return prcm;
        }

        #endregion

        #region -- SetALL && GetALL --
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        public void SetAll<T>(Dictionary<string, T> a)
        {
            using (var redis = _prcm.GetClient())
            {
                redis.SetAll(a);
            }
        }

        /// <summary>
        /// 批量获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            using (var redis = _prcm.GetClient())
            {
                return redis.GetAll<T>(keys);
            }
        }
        #endregion

        #region -- Key --

        /// <summary>
        /// 设置单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param> 
        /// <returns></returns>
        public bool ItemSet<T>(string key, T t)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.Set(key, t); //, new TimeSpan(1, 0, 0));
            }
        }

        /// <summary>
        /// 自增
        /// </summary> 
        /// <param name="key"></param> 
        /// <returns></returns>
        public long ItemIncr(string key)
        {

            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.Incr(key);
            }

        }

        /// <summary>
        /// 自增
        /// </summary> 
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public long ItemIncrBy(string key, int count)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.IncrBy(key, count);
            }
        }

        /// <summary>
        /// 自减
        /// </summary> 
        /// <param name="key"></param> 
        /// <returns></returns>
        public long ItemDecr(string key)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.Decr(key);
            }
        }

        /// <summary>
        /// 设置单体
        /// </summary> 
        /// <param name="key"></param> 
        /// <returns></returns>
        public bool ItemContain(string key)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.ContainsKey(key);//, new TimeSpan(1, 0, 0));
            }
        }

        /// <summary>
        /// 获取单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ItemGet<T>(string key)
        {
            try
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.Get<T>(key);
                }
            }
            catch (Exception)
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.Get<T>(key);
                }
            }
        }

        /// <summary>
        /// 移除单体
        /// </summary>
        /// <param name="key"></param>
        public bool ItemRemove(string key)
        {

            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.Remove(key);
            }

        }

        /// <summary>
        /// 设置缓存过期的时间间隔
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ts"></param>
        public bool ItemSetExpire(string key, TimeSpan ts)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.ExpireEntryIn(key, ts);
            }
        }

        /// <summary>
        /// 设置缓存过期的时间间隔
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public bool ItemSetExpire(string key, DateTime datetime)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.ExpireEntryAt(key, datetime);
            }
        }

        /// <summary>
        /// 是否已经存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ItemExists(string key)
        {
            try
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.Exists(key);
                }
            }
            catch (Exception)
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.Exists(key);
                }
            }

        }

        #endregion

        #region -- List --
        /// <summary>
        /// 添加单体到List中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public void ListAdd<T>(string key, T t)
        {
            using (var redis = _prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.AddItemToList(redisTypedClient.Lists[key], t);
            }
        }

        /// <summary>
        /// 从List中删除单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool ListRemove<T>(string key, T t)
        {
            using (var redis = _prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.RemoveItemFromList(redisTypedClient.Lists[key], t) > 0;
            }
        }

        /// <summary>
        /// 删除List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public void ListRemoveAll<T>(string key)
        {
            using (var redis = _prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.Lists[key].RemoveAll();
            }
        }

        /// <summary>
        /// 获取List中的总数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListCount(string key)
        {
            using (var redis = _prcm.GetClient())
            {
                return redis.GetListCount(key);
            }
        }

        /// <summary>
        /// 从List中获取指定起止行的对应记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<T> ListGetRange<T>(string key, int start, int count)
        {
            using (var redis = _prcm.GetClient())
            {
                var c = redis.As<T>();
                return c.Lists[key].GetRange(start, start + count - 1);
            }
        }

        /// <summary>
        /// 获取全部的List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListGetList<T>(string key)
        {
            using (var redis = _prcm.GetClient())
            {
                var c = redis.As<T>();
                return c.Lists[key].GetRange(0, c.Lists[key].Count);
            }
        }

        /// <summary>
        /// 获取全部的List中指定值的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int ListGetIndex<T>(string key, T value)
        {
            using (var redis = _prcm.GetClient())
            {
                var c = redis.As<T>();
                return c.Lists[key].IndexOf(value);
            }
        }

        /// <summary>
        /// 分页提取对应的List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<T> ListGetList<T>(string key, int pageIndex, int pageSize)
        {
            int start = pageSize * (pageIndex - 1);
            return ListGetRange<T>(key, start, pageSize);
        }

        /// <summary>
        /// 批量添加值到List
        /// </summary>
        /// <param name="key">List键</param>
        /// <param name="values">值</param>
        public void ListAddRange(string key, List<string> values)
        {
            if (values.Count > 0)
            {
                using (var redis = _prcm.GetClient())
                {
                    redis.AddRangeToList(key, values);
                }
            }
        }

        /// <summary>
        /// 设置缓存过期的时间间隔
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ts"></param>
        public bool ListSetExpire(string key, TimeSpan ts)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.ExpireEntryIn(key, ts);
            }
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public bool ListSetExpire(string key, DateTime datetime)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.ExpireEntryAt(key, datetime);
            }
        }

        #endregion

        #region -- List Queue --

        public void Enqueue<T>(string key, T value)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.EnqueueItemOnList(redisTypedClient.Lists[key], value);
            }
        }

        public T Dequeue<T>(string key)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.DequeueItemFromList(redisTypedClient.Lists[key]);
            }
        }

        public T BlockingDequeue<T>(string key)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.BlockingDequeueItemFromList(redisTypedClient.Lists[key], new TimeSpan(0, 0, 5));
            }
        }

        #endregion

        #region -- List Stack --

        /// <summary>
        /// 入栈，插入单个字符串 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long LPush(string key, object value)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                byte[] val = Encoding.UTF8.GetBytes((string)value);
                return redis.LPush(key, val);
            }
        }

        /// <summary>
        /// 移除并返回列表 key 的头元素。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string LPop(string key)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                var val = redis.LPop(key);
                if (val == null || val.Length == 0) return string.Empty;
                return Encoding.UTF8.GetString(val);
            }
        }

        /// <summary>
        /// 对一个列表进行修剪，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <remarks>当 key 不是列表类型时，返回一个错误</remarks>
        public bool LTrim(string key, int start, int stop)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                try
                {
                    redis.LTrim(key, start, stop);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region -- Hash --

        /// <summary>
        /// 批量插入键值对数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="pairs"></param>
        public void HashSetRange<T>(string hashId, Dictionary<string, T> pairs)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                var fields = (from k in pairs.Keys
                              select k).ToArray();

                var values = (from v in pairs.Values
                              select v).ToArray();

                var fieldBytes = new byte[fields.Length][];
                for (var i = 0; i < fields.Length; ++i)
                {
                    fieldBytes[i] = Encoding.UTF8.GetBytes(fields[i]);

                }
                var valueBytes = new byte[values.Length][];
                for (var i = 0; i < values.Length; ++i)
                {
                    string s;
                    if (typeof(T) == typeof(string) || typeof(T) == typeof(String))
                    {
                        s = values[i].ToString();
                    }
                    else
                    {
                        s = JsonHelper.ObjectToJson(values[i]);
                    }
                    valueBytes[i] = Encoding.UTF8.GetBytes(s);
                }

                var p = redis.CreatePipeline();
                p.QueueCommand(r => ((RedisNativeClient)r).HMSet(hashId, fieldBytes, valueBytes));
                p.Flush();
                p.Dispose();
            }
        }

        /// <summary>
        /// 添加字符串（存入Hash） 如果该Key存在，则返回false
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>
        /// 如果 field 是哈希表中的一个新建域，并且值设置成功，返回 1 。
        /// 如果哈希表中域 field 已经存在且旧值已被新值覆盖，返回 0 。</returns>
        public bool HashSet(string hashId, string key, string value)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.SetEntryInHash(hashId, key, value);
            }
        }

        /// <summary>
        /// 设置Hash中某个字段的值自增
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public long HashSetIncre(string hashId, string key, int increment)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.HIncrby(hashId, Encoding.UTF8.GetBytes(key), increment);
            }
        }

        /// <summary>
        /// 从Hash中获取字符串
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param> 
        /// <returns></returns>
        public string HashGet(string hashId, string key)
        {
            try
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetValueFromHash(hashId, key);
                }
            }
            catch (Exception)
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetValueFromHash(hashId, key);
                }
            }

        }

        //public T HashGet<T>(string hashId) where T : class, new()
        //{
        //    using (var redis = (RedisClient)_prcm.GetClient())
        //    {
        //        Type type = typeof(T);
        //        T result = new T();
        //        foreach (PropertyInfo pi in type.GetProperties())
        //        {
        //            byte[] bytes = redis.HGet(hashId, Encoding.UTF8.GetBytes(pi.Name));
        //            if (bytes != null)
        //            {
        //                string value = Encoding.UTF8.GetString(bytes);
        //                if (type == typeof(string))
        //                {
        //                    pi.SetValue(result, value);
        //                }
        //                else if (type.IsValueType)
        //                {

        //                }
        //                else
        //                {
        //                    pi.SetValue(result, JsonHelper.JsonToObject<T>(value));
        //                }
        //            }
        //        }

        //        return result;
        //    }
        //}

        /// <summary>
        /// 从Hash中获取字符串集合
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public List<string> HashGetValues(string hashId, params string[] keys)
        {
            try
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetValuesFromHash(hashId, keys).Where(x => !string.IsNullOrEmpty(x)).ToList();
                }
            }
            catch (Exception)
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetValuesFromHash(hashId, keys).Where(x => !string.IsNullOrEmpty(x)).ToList();
                }
            }
        }

        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        /// <param name="hashid"></param> 
        /// <returns></returns>
        public Dictionary<string, string> HashGetAll(string hashid)
        {
            try
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetAllEntriesFromHash(hashid);
                }
            }
            catch (Exception)
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetAllEntriesFromHash(hashid);
                }
            }

        }
        
        public T HashGetEntity<T>(string hashId) where T : class
        {
            using (var redis = (RedisClient) _prcm.GetClient())
            {
                Type t = typeof (T);
                string[] keys = t.GetProperties().Select(p => p.Name).ToArray();
                List<string> values = redis.GetValuesFromHash(hashId, keys);
                T result = (T) t.Assembly.CreateInstance(t.FullName);
                var jSetting = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
                foreach (var propertyInfo in t.GetProperties())
                {
                    int index = Array.IndexOf(keys, propertyInfo.Name);
                    if (index < 0)
                    {
                        continue;
                    }

                    string value = values[index];
                    if (value == null)
                    {
                        continue;
                    }
                    if (propertyInfo.PropertyType == typeof (Guid))
                    {
                        propertyInfo.SetValue(result, new Guid(value));
                    }
                    else if (propertyInfo.PropertyType == typeof (string))
                    {
                        propertyInfo.SetValue(result, value);
                    }
                    else
                    {
                        propertyInfo.SetValue(result,
                            JsonConvert.DeserializeObject(value, propertyInfo.PropertyType, jSetting));
                    }
                }
                return result;
            }
        }


        /// <summary>
        /// 返回哈希表 key 中域的数量。
        /// </summary>
        /// <param name="hashid"></param>
        /// <returns></returns>
        public long HashGetLen(string hashid)
        {
            try
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetHashCount(hashid);
                }
            }
            catch (Exception)
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetHashCount(hashid);
                }
            }

        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="hashid"></param> 
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HashRemove(string hashid, string key)
        {
            if (string.IsNullOrEmpty(hashid) || string.IsNullOrEmpty(key))
                return true;
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.RemoveEntryFromHash(hashid, key);
            }
        }

        /// <summary>
        /// 判断某个值（字符串）是否已经被缓存
        /// </summary>
        /// <param name="hashid">hash表的ID</param>
        /// <param name="key">字段</param> 
        /// <returns></returns>
        public bool HashExist(string hashid, string key)
        {
            try
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.HashContainsEntry(hashid, key);
                }
            }
            catch (Exception)
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.HashContainsEntry(hashid, key);
                }
            }

        }

        /// <summary>
        /// 设置缓存过期的时间间隔
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ts"></param>
        public bool HashSetExpire(string key, TimeSpan ts)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.ExpireEntryIn(key, ts);
            }
        }

        /// <summary>
        /// 设置Hash缓存过期 待验证,不提倡使用
        /// </summary>
        /// <param name="dataKey"></param>
        /// <param name="datetime"></param>
        public bool HashSetExpire(string dataKey, DateTime datetime)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.ExpireEntryAt(dataKey, datetime);
            }
        }

        /// <summary>
        /// 获取指定HashId下所有的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> HashSearchKeys(string key)
        {
            try
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetHashKeys(key);
                }
            }
            catch (Exception)
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetHashKeys(key);
                }
            }

        }

        /// <summary>
        /// 获取指定HashId下所有的value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> HashSearchValues(string key)
        {
            try
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetHashValues(key);
                }
            }
            catch (Exception)
            {
                using (var redis = (RedisClient)_prcm.GetClient())
                {
                    return redis.GetHashValues(key);
                }
            }
        }

        #endregion

        #region -- Set --

        /// <summary>
        /// 向Set添加item
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        public void AddItemToSet(string setId, string value)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                redis.AddItemToSet(setId, value);
            }
        }

        /// <summary>
        /// 从Set移除item
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="item"></param>
        public void RemoveItemFromSet(string setId, string item)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                redis.RemoveItemFromSet(setId, item);
            }
        }

        /// <summary>
        /// 获取SET中所有的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setId"></param>
        /// <returns></returns>
        public HashSet<string> GetAllItemsFromSet(string setId)
        {
            HashSet<string> result;
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                result = redis.GetAllItemsFromSet(setId);
            }
            return result;
        }

        /// <summary>
        /// 判断Set中是否存在Item
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool SetContainsItem(string setId, string item)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.SetContainsItem(setId, item);
            }
        }

        #endregion

        #region -- SortedSet --

        /// <summary>
        /// 添加T到 SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public void SortedSetAdd<T>(string key, T t)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.AddItemToSortedSet(redisTypedClient.SortedSets[key], t);
            }
        }

        /// <summary>
        ///  添加T到 SortedSet，带Score
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="score"></param>
        public void SortedSetAdd<T>(string key, T t, double score)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.AddItemToSortedSet(redisTypedClient.SortedSets[key], t, score);
            }
        }

        /// <summary>
        /// 添加新值到SortedSet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool SortedSetAdd(string key, string value, double score)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.AddItemToSortedSet(key, value, score);
            }
        }

        /// <summary>
        /// 从SortedSet移除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool RemoveItemFromSortedSet<T>(string key, T t)
        {
            using (var redis = _prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.RemoveItemFromSortedSet(redisTypedClient.SortedSets[key], t);
            }
        }

        /// <summary>
        /// 修剪SortedSet 移除指定SCORE范围的项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <returns></returns>
        public long RemoveRangeFromSortedSetByScore<T>(string key, double minScore, double maxScore)
        {
            using (var redis = _prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.RemoveRangeFromSortedSetByScore(redisTypedClient.SortedSets[key], minScore, maxScore);
            }
        }

        /// <summary>
        /// 修剪SortedSet 移除指定Rank范围内的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="minRank"></param>
        /// <param name="maxRank"></param>
        /// <returns></returns>
        public long RemoveRangeFromSortedSet<T>(string key, int minRank, int maxRank)
        {
            using (var redis = _prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.RemoveRangeFromSortedSet(redisTypedClient.SortedSets[key], minRank, maxRank);
            }
        }

        /// <summary>
        /// 获取SortedSet的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetCount<T>(string key)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetSortedSetCount(redisTypedClient.SortedSets[key]);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetSortedSetCount(redisTypedClient.SortedSets[key]);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的分页数据 ,按页码分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<T> SortedSetGetPageListByIndex<T>(string key, int pageIndex, int pageSize)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSet(redisTypedClient.SortedSets[key], (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                }

            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSet(redisTypedClient.SortedSets[key], (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的分页数据 ,按页码分页,按SCORE 降序排列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<T> SortedSetGetPageListDescByIndex<T>(string key, int pageIndex, int pageSize)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    if (redisTypedClient == null) return new List<T>();
                    return redisTypedClient.GetRangeFromSortedSetDesc(redisTypedClient.SortedSets[key],
                                                                      (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    if (redisTypedClient == null) return new List<T>();
                    return redisTypedClient.GetRangeFromSortedSetDesc(redisTypedClient.SortedSets[key],
                                                                      (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的分页数据 ,按页码分页,按SCORE 升序排列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<T> SortedSetGetListByIndex<T>(string key, int fromIndex, int toIndex)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    if (redisTypedClient == null) return new List<T>();
                    return redisTypedClient.GetRangeFromSortedSet(redisTypedClient.SortedSets[key], fromIndex, toIndex);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    if (redisTypedClient == null) return new List<T>();
                    return redisTypedClient.GetRangeFromSortedSet(redisTypedClient.SortedSets[key], fromIndex, toIndex);
                }
            }
        }

        /// <summary>
        /// 获取SortedSet的分页数据 ,按页码分页,按SCORE 降序排列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<T> SortedSetGetListDescByIndex<T>(string key, int fromIndex, int toIndex)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    if (redisTypedClient == null) return new List<T>();
                    return redisTypedClient.GetRangeFromSortedSetDesc(redisTypedClient.SortedSets[key], fromIndex, toIndex);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    if (redisTypedClient == null) return new List<T>();
                    return redisTypedClient.GetRangeFromSortedSetDesc(redisTypedClient.SortedSets[key], fromIndex, toIndex);
                }
            }
        }

        /// <summary>
        /// 获取SortedSet的分页数据 withScore ,按页码分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IDictionary<T, double> SortedListDescWithScoreByIndex<T>(string key, int pageIndex, int pageSize)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeWithScoresFromSortedSetDesc(redisTypedClient.SortedSets[key],
                                                                      (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeWithScoresFromSortedSetDesc(redisTypedClient.SortedSets[key],
                                                                      (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的分页数据 withScore ,按页码分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IDictionary<T, double> SortedListWithScoreByIndex<T>(string key, int pageIndex, int pageSize)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeWithScoresFromSortedSet(redisTypedClient.SortedSets[key],
                                                                            (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeWithScoresFromSortedSet(redisTypedClient.SortedSets[key],
                                                                            (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的全部数据 按SCORE 范围取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <returns></returns>
        public List<T> SortedSetGetListByScore<T>(string key, double minScore, double maxScore)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSetByLowestScore(redisTypedClient.SortedSets[key], minScore,
                                                                               maxScore);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSetByLowestScore(redisTypedClient.SortedSets[key], minScore,
                                                                               maxScore);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的全部数据 按SCORE 范围取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<T> SortedSetGetListByScore<T>(string key, double minScore, double maxScore, int skip, int take)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSetByLowestScore(redisTypedClient.SortedSets[key], minScore,
                                                                               maxScore, skip, take);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSetByLowestScore(redisTypedClient.SortedSets[key], minScore,
                                                                               maxScore, skip, take);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的全部数据 按SCORE 范围取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public List<T> SortedSetGetListDescByScore<T>(string key, double minScore, double maxScore, int skip, int take)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSetByHighestScore(redisTypedClient.SortedSets[key], minScore,
                                                                               maxScore, skip, take);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSetByHighestScore(redisTypedClient.SortedSets[key], minScore,
                                                                               maxScore, skip, take);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的全部数据 按SCORE 范围取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <returns></returns>
        public List<T> SortedSetGetListDescByScore<T>(string key, double minScore, double maxScore)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSetByHighestScore(redisTypedClient.SortedSets[key], minScore,
                                                                               maxScore);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeFromSortedSetByHighestScore(redisTypedClient.SortedSets[key], minScore,
                                                                               maxScore);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的全部数据带SCORE 按SCORE 范围取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IDictionary<T, double> SortedListDescWithScoreByScore<T>(string key, double minScore, double maxScore, int skip, int take)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeWithScoresFromSortedSetByHighestScore(redisTypedClient.SortedSets[key],
                                                                                          minScore, maxScore, skip, take);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeWithScoresFromSortedSetByHighestScore(redisTypedClient.SortedSets[key],
                                                                                          minScore, maxScore, skip, take);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的全部数据带SCORE 按SCORE 范围取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IDictionary<T, double> SortedListWithScoreByScore<T>(string key, double minScore, double maxScore, int skip, int take)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeWithScoresFromSortedSetByLowestScore(redisTypedClient.SortedSets[key],
                                                                                          minScore, maxScore, skip, take);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetRangeWithScoresFromSortedSetByLowestScore(redisTypedClient.SortedSets[key],
                                                                                          minScore, maxScore, skip, take);
                }
            }

        }

        /// <summary>
        /// 有序队列是否存在指定对象
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SortedSetContainsItem<T>(string key, T value)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.SortedSetContainsItem(redisTypedClient.SortedSets[key], value);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.SortedSetContainsItem(redisTypedClient.SortedSets[key], value);
                }
            }
        }

        /// <summary>
        /// 有序队列分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="score"></param>
        public void SetSortedListScore<T>(string key, T t, double score)
        {
            using (var redis = _prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                var oldScore = redisTypedClient.GetItemScoreInSortedSet(redisTypedClient.SortedSets[key], t);
                if (double.IsNaN(oldScore))
                {
                    oldScore = 0;
                }
                redisTypedClient.IncrementItemInSortedSet(redisTypedClient.SortedSets[key], t, score - oldScore);
            }
        }

        /// <summary>
        /// 获取SortedSet的全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetGetListAll<T>(string key)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetAllItemsFromSortedSet(redisTypedClient.SortedSets[key]);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetAllItemsFromSortedSet(redisTypedClient.SortedSets[key]);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的全部数据 按SCORE 降序排列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetGetListAllDesc<T>(string key)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetAllItemsFromSortedSetDesc(redisTypedClient.SortedSets[key]);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetAllItemsFromSortedSetDesc(redisTypedClient.SortedSets[key]);
                }
            }

        }

        /// <summary>
        /// 获取SortedSet的全部数据 带SCORE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public IDictionary<T, double> SortedListAllWithScore<T>(string key)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetAllWithScoresFromSortedSet(redisTypedClient.SortedSets[key]);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetAllWithScoresFromSortedSet(redisTypedClient.SortedSets[key]);
                }
            }

        }

        /// <summary>
        /// 找出T 的SCORE 值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public double SortedScoreByItem<T>(string key, T t)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    var v = redisTypedClient.GetItemScoreInSortedSet(redisTypedClient.SortedSets[key], t);
                    return v;
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    var v = redisTypedClient.GetItemScoreInSortedSet(redisTypedClient.SortedSets[key], t);
                    return v;
                }
            }
        }

        /// <summary>
        /// 获取SortedSet指定Range区间的值（升序）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fromRank"></param>
        /// <param name="toRank"></param>
        /// <returns></returns>
        public IDictionary<T, double> GetItemSortedSetByIndex<T>(string key, int fromRank, int toRank)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    var dic = redisTypedClient.GetRangeWithScoresFromSortedSet(redisTypedClient.SortedSets[key], fromRank, toRank);
                    return dic;
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    var dic = redisTypedClient.GetRangeWithScoresFromSortedSet(redisTypedClient.SortedSets[key], fromRank, toRank);
                    return dic;
                }
            }
        }

        /// <summary>
        /// 设置缓存过期的时间间隔
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ts"></param>
        public bool SortedSetSetExpire(string key, TimeSpan ts)
        {
            using (var redis = _prcm.GetClient())
            {
                return redis.ExpireEntryIn(key, ts);
            }
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public bool SortedSetSetExpire(string key, DateTime datetime)
        {
            using (var redis = _prcm.GetClient())
            {
                return redis.ExpireEntryAt(key, datetime);
            }
        }

        /// <summary>
        /// 获取有序队列中值的插入顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long GetItemIndexInSortedSet<T>(string key, T value)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetItemIndexInSortedSet(redisTypedClient.SortedSets[key], value);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetItemIndexInSortedSet(redisTypedClient.SortedSets[key], value);
                }
            }
        }

        /// <summary>
        /// 根据插入顺序获取有序队列中的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public T SortedSetGetValueByIndex<T>(string key, int index)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    List<T> result = redisTypedClient.GetRangeFromSortedSet(redisTypedClient.SortedSets[key], index, index);
                    return result.Count > 0 ? result[0] : default(T);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    List<T> result = redisTypedClient.GetRangeFromSortedSet(redisTypedClient.SortedSets[key], index, index);
                    return result.Count > 0 ? result[0] : default(T);
                }
            }
        }

        /// <summary>
        /// 获取有序队列中所有的值及其Score
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public IDictionary<T, double> GetAllWithScoresFromSortedSet<T>(string key)
        {
            using (var redis = _prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.GetAllWithScoresFromSortedSet(redisTypedClient.SortedSets[key]);
            }
        }

        /// <summary>
        /// 获取有序队列中值的插入顺序(倒序)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long GetItemIndexInSortedSetDesc<T>(string key, T value)
        {
            try
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetItemIndexInSortedSetDesc(redisTypedClient.SortedSets[key], value);
                }
            }
            catch (Exception)
            {
                using (var redis = _prcm.GetClient())
                {
                    var redisTypedClient = redis.As<T>();
                    return redisTypedClient.GetItemIndexInSortedSetDesc(redisTypedClient.SortedSets[key], value);
                }
            }
        }

        #endregion

        public bool ExecTrans(Action<IRedisClient>[] actions, string[] watchKeys = null)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                if (watchKeys != null && watchKeys.Length > 0)
                {
                    redis.Watch(watchKeys);
                }

                IRedisTransaction trans = redis.CreateTransaction();
                try
                {
                    foreach (var action in actions)
                    {
                        trans.QueueCommand(action);
                    }
                    return trans.Commit();
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #region Misc

        /// <summary>
        /// 返回随机key(代理不支持此命令)
        /// </summary>
        /// <returns></returns>
        public string RandomKey()
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.RandomKey();
            }
        }

        /// <summary>
        /// 查询key 模糊查询该字符串下所有符合条件的KEY
        /// </summary>
        /// <param name="key"></param>
        public List<string> SearchKeys(string key)
        {
            using (var redis = (RedisClient)_prcm.GetClient())
            {
                return redis.SearchKeys(key);
            }
        }

        #region 判断服务器是否正常
        public bool RedisPing(string host)
        {
            var client = new RedisNativeClient(host);
            try
            {
                if (!client.Ping())
                    return false;
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #endregion
    }
}
