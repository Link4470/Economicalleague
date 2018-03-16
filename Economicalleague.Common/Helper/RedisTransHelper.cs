using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using ServiceStack.Redis;

namespace Economicalleague.Common
{
    public class RedisTransHelper : IDisposable
    {
        private static readonly string RedisUrl = ConfigurationManager.AppSettings["Redis_Server"];
        private static readonly string RedisPort = ConfigurationManager.AppSettings["Redis_Port"];
        private static readonly string RedisPwd = ConfigurationManager.AppSettings["Redis_Pwd"];
        private static readonly string RedisHost = $"{RedisPwd}@{RedisUrl}:{RedisPort}";
        private static readonly PooledRedisClientManager Prcm;

        private readonly IRedisClient _client;
        private IRedisTransaction _transaction;

        static RedisTransHelper()
        {
            Prcm = CreateManager(RedisHost);
        }

        public RedisTransHelper()
        {
            _client = Prcm.GetClient();
        }

        public void Dispose()
        {
            _client?.Dispose();
            _transaction?.Dispose();
        }

        ~RedisTransHelper()
        {
            Dispose();
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

        /// <summary>
        /// 监视key
        /// </summary>
        /// <param name="keys"></param>
        public void Watch(params string[] keys)
        {
            _client.Watch(keys);
        }

        public void CreateTransaction()
        {
            _transaction = _client.CreateTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            bool result = _transaction != null && _transaction.Commit();
            _transaction = null;
            return result;
        }

        /// <summary>
        /// 从Hash中获取字符串
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key"></param> 
        /// <returns></returns>
        public T HashGet<T>(string hashId, string key)
        {
            try
            {
                var redisTypedClient = _client.As<T>();
                return redisTypedClient.GetValueFromHash(redisTypedClient.GetHash<string>(hashId), key);
            }
            catch (Exception)
            {
                var redisTypedClient = _client.As<T>();
                return redisTypedClient.GetValueFromHash(redisTypedClient.GetHash<string>(hashId), key);
            }
        }

        /// <summary>
        /// 设置Hash中某个字段的值自增
        /// </summary>
        /// <param name="hashId">hashId</param>
        /// <param name="key">键</param>
        /// <param name="increment">增量</param>
        /// <returns></returns>
        public long HashSetIncre(string hashId, string key, int increment)
        {
            if (_transaction != null)
            {
                _transaction.QueueCommand(r => r.IncrementValueInHash(hashId, key, increment));
                return 0;
            }
            else
            {
                return _client.IncrementValueInHash(hashId, key, increment);
            }
        }

        /// <summary>
        /// 向Set添加item
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="value"></param>
        public void AddItemToSet<T>(string setId, T value)
        {
            if (_transaction != null)
            {
                _transaction.QueueCommand(r =>
                {
                    var redisTypedClient = r.As<T>();
                    redisTypedClient.AddItemToSet(redisTypedClient.Sets[setId], value);
                });
            }
            else
            {
                var redisTypedClient = _client.As<T>();
                redisTypedClient.AddItemToSet(redisTypedClient.Sets[setId], value);
            }
        }

        /// <summary>
        /// 添加到 SortedSet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            if (_transaction != null)
            {
                _transaction.QueueCommand(r =>
                {
                    var redisTypedClient = r.As<T>();
                    redisTypedClient.AddItemToSortedSet(redisTypedClient.SortedSets[key], value, score);
                });
                return true;
            }
            else
            {
                var redisTypedClient = _client.As<T>();
                redisTypedClient.AddItemToSortedSet(redisTypedClient.SortedSets[key], value, score);
                return true;
            }
        }

        /// <summary>
        /// 从SortedSet删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool RemoveItemFromSortedSet<T>(string key, T value)
        {
            if (_transaction != null)
            {
                _transaction.QueueCommand(r =>
                {
                    var redisTypedClient = r.As<T>();
                    redisTypedClient.RemoveItemFromSortedSet(redisTypedClient.SortedSets[key], value);
                });
                return true;
            }
            else
            {
                var redisTypedClient = _client.As<T>();
                return redisTypedClient.RemoveItemFromSortedSet(redisTypedClient.SortedSets[key], value);
            }
        }
    }
}
