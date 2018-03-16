using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using StackExchange.Redis;
using Rongzi.Infrastructure.Config.Cache;
using Rongzi.Infrastructure.Config;
using Rongzi.Infrastructure.Util;
using ServiceStack;
using Economicalleague.Common.Compress;

namespace Economicalleague.Common
{
    public class RedisCache : IDisposable
    {
        #region
        protected static string server = ConfigurationManager.AppSettings["RedisCacheServer"];
        //private static int CompressFlag = string.IsNullOrEmpty(ConfigurationManager.AppSettings["RedisCache_Flag_Zip"]) ? 0:Convert.ToInt32(ConfigurationManager.AppSettings["RedisCache_Flag_Zip"]);
        //private static int CompressThreshold = string.IsNullOrEmpty(ConfigurationManager.AppSettings["RedisCache_Size_CompressThreshold"]) ? 64000 : Convert.ToInt32(ConfigurationManager.AppSettings["RedisCache_Size_CompressThreshold"]);
        //private static int ConnectFlag = string.IsNullOrEmpty(ConfigurationManager.AppSettings["RedisCache_ConnectFlag"]) ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["RedisCache_ConnectFlag"]);
        //private static int Port = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Redis_Port"]) ? 6379 : Convert.ToInt32(ConfigurationManager.AppSettings["Redis_Port"]);
        //private static string Pwd = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Redis_Pwd"])
        //    ? ""
        //    : ConfigurationManager.AppSettings["Redis_Pwd"].ToString();
        //private static int Ssl = string.IsNullOrEmpty(ConfigurationManager.AppSettings["Redis_Port"]) ? 6379 : Convert.ToInt32(ConfigurationManager.AppSettings["Redis_Port"]);

        private static RedisSetting setting = new RedisSetting();
        private StringBuilder sbLastMsg = new StringBuilder();
        private StringBuilder sbInnerMsg = new StringBuilder();
        private static ConnectionMultiplexer connection1 = null;
        private IDatabase cache = null;
        private static object objLock = new object();

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            if (connection1 != null)
            {
                LogHelper.Error("Redis connection has already existed");
                return connection1;
            }

            ConfigurationOptions opt = new ConfigurationOptions
            {
                Ssl = RedisSetting.Ssl,
                AbortOnConnectFail = RedisSetting.AbortOnConnectFail, //for reconnect
                Password = RedisSetting.Pwd,
                KeepAlive = RedisSetting.KeepAlive, // keep connection alive (ping every minute)
                ConnectRetry = RedisSetting.ConnectRetry, // retry connection if broken
                SyncTimeout = RedisSetting.SyncTimeout, // 8 seconds timeout for each get/set/remove operation
                ConnectTimeout = RedisSetting.ConnectTimeout // 20 seconds to connect to the cache
            };

            opt.EndPoints.Add(new DnsEndPoint(RedisSetting.Server, RedisSetting.Port));
            connection1 = ConnectionMultiplexer.Connect(opt);

            LogHelper.Error("connect to Redis");

            return connection1;
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        protected string LastMsg
        {
            get { return sbLastMsg.Append(sbInnerMsg).ToString(); }
        }

        public RedisCache()
        {

        }
        public RedisCache(string srv)
        {
            server = srv;
            LogHelper.Error("RedisCache");
            //lazyConnection = 
            //new Lazy<ConnectionMultiplexer>(() =>
            //{
            //    return ConnectionMultiplexer.Connect(server);
            //});
        }
        #endregion
        #region methods
        private bool ConnectServer()
        {
            try
            {
                var stopwatch = new Stopwatch();
                #region todel
                //        // 1
                //        if (connection == null || (!connection.IsConnected))
                //        {
                //            var stopwatchLock = new Stopwatch();
                //            stopwatchLock.Start();
                //            sbLastMsg.Append("bigin to lock...");

                //            lock (objLock)
                //            {
                //                if (connection == null || !connection.IsConnected)
                //                {
                //                    var w = new StringWriter();
                //                    stopwatch.Start();
                //                    if (RedisConnectFlag == 1)
                //                    {
                //                        connection = ConnectionMultiplexer.Connect(server, w);
                //                        sbInnerMsg.Append("ConnectionMultiplexer.Connect.Msg")
                //                                    .AppendFormat(",msg={0}", w.ToString());
                //                    }
                //                    else
                //                    {
                //                        connection = ConnectionMultiplexer.Connect(server);
                //                    }
                //                    stopwatch.Stop();
                //                    sbLastMsg.Append("获取连接：ConnectionMultiplexer.Connect")
                //                        .AppendFormat(",耗时:{0}", stopwatch.ElapsedMilliseconds);
                //                }
                //            }
                //            stopwatchLock.Stop();
                //            sbLastMsg.Append("end to lock")
                //                     .AppendFormat(",耗时={0}", stopwatchLock.ElapsedMilliseconds);
                //        }
                #endregion
                stopwatch.Restart();
                cache = Connection.GetDatabase();
                stopwatch.Stop();
                sbLastMsg.Append("GetDatabase")
                         .AppendFormat(",耗时:{0};", stopwatch.ElapsedMilliseconds);

                return true;
            }
            catch (Exception ex)
            {
                sbLastMsg.Append("Redis异常：").Append(ex.Message);
                throw ex;
            }
        }
        public string StringGet(string key)
        {
            CheckKey(key);
            if (!ConnectServer())
            {
                return null;
            }

            return cache.StringGet(key);
        }

        public bool StringSet(string key, string value, TimeSpan? ttl)
        {
            CheckKey(key);
            if (!ConnectServer())
            {
                return false;
            }

            return cache.StringSet(key, value, ttl);
        }

        public T BinaryGet<T>(string key)
        {
            try
            {
                CheckKey(key);
                CheckCacheable(typeof(T));

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                if (!ConnectServer())
                {
                    return default(T);
                }
                stopwatch.Stop();
                sbLastMsg.Append("获取数据：连接Redis").AppendFormat(",耗时:{0};", stopwatch.ElapsedMilliseconds);

                stopwatch.Restart();
                byte[] buffer = cache.StringGet(key);
                if (buffer == null)
                {
                    return default(T);
                }
                stopwatch.Stop();
                sbLastMsg.Append("数据缓存获取").AppendFormat(",耗时:{0};", stopwatch.ElapsedMilliseconds);

                stopwatch.Restart();
                buffer = UnZip(buffer, 0, buffer.Length);
                stopwatch.Stop();
                sbLastMsg.Append("数据解压").AppendFormat(",数据量:{0}", buffer.Length).AppendFormat(",耗时:{0};", stopwatch.ElapsedMilliseconds);

                stopwatch.Restart();
                var o = Deserialize<T>(buffer);
                stopwatch.Stop();
                sbLastMsg.Append("数据反序列化").AppendFormat(",耗时:{0};", stopwatch.ElapsedMilliseconds);

                return o;
            }
            catch (Exception ex)
            {
                sbLastMsg.Append(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// BinarySet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public bool BinarySet<T>(string key, T value, TimeSpan? ttl)
        {
            CheckKey(key);
            CheckCacheable(typeof(T));

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            if (!ConnectServer())
            {
                return false;
            }
            stopwatch.Stop();
            sbLastMsg.Append("存入数据：连接Redis").AppendFormat(",耗时:{0};", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            var buffer = Serialize(value);
            stopwatch.Stop();
            sbLastMsg.Append("数据序列化").AppendFormat(",数据量:{0}", buffer.Length).AppendFormat(",耗时:{0};", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            buffer = Zip(buffer, 0, buffer.Length);
            stopwatch.Stop();
            sbLastMsg.Append("数据压缩").AppendFormat(",数据量:{0}", buffer.Length).AppendFormat(",耗时:{0};", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            var ret = cache.StringSet(key, buffer, ttl);
            stopwatch.Stop();
            sbLastMsg.Append("数据放入缓存").AppendFormat(",耗时:{0};", stopwatch.ElapsedMilliseconds);

            return ret;
        }

        private void CheckCacheable(Type type)
        {
            if (!type.IsSerializable)
            {
                throw new ArgumentException("只有可序列化的类才能作为泛型的实参");
            }
        }

        private void CheckKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", "缓存的键必须为长度大于0的非空字符串");
            }
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, o);
                    byte[] buffer = memoryStream.ToArray();

                    return buffer;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Zip
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static byte[] Zip(byte[] buffer, long index, long size)
        {
            //if (buffer.Length < RedisSetting.CompressThreshold
            //    || RedisSetting.CompressFlag != 1)
            //{
            //    return buffer;
            //}

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            //CacheProviderConfigurationElement cacheProviderConfig = ConfigHelper.GetCacheProvider();
            //var ass = cacheProviderConfig.Type.Split(',');
            //var compress = Helper.CreateObj<ICompress>(ass[0], ass[1]);
            //var retBuffer = compress.Compress(buffer, 0, buffer.LongLength);
            //using (MemoryStream outputStream = new MemoryStream())
            //{
            //    outputStream.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 0, 4);
            //    outputStream.Write(retBuffer, 0, retBuffer.Length);

            //    retBuffer = outputStream.ToArray();
            //}

            //stopwatch.Stop();
            //var span = stopwatch.ElapsedMilliseconds;

            //return retBuffer;
            throw new NullReferenceException();
        }
        private static byte[] UnZip(byte[] buffer, long index, long count)
        {
            ////var ms = new MemoryStream();
            //var bufferFlag = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            //bool compressFlag = (buffer[0] == bufferFlag[0])
            //                    && (buffer[1] == bufferFlag[1])
            //                    && (buffer[2] == bufferFlag[2])
            //                    && (buffer[3] == bufferFlag[3]);
            //if (!compressFlag)
            //{
            //    return buffer;
            //    //var retBuffer = UnZip(buffer, 4, buffer.Length - 4);
            //    //ms.Write(retBuffer, 0, retBuffer.Length);
            //}
            ////else
            ////{
            ////    ms.Write(buffer, 0, buffer.Length);
            ////}
            ////ms.Position = 0L;

            //index = 4;
            //count = buffer.Length - 4;

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            //CacheProviderConfigurationElement cacheProviderConfig = ConfigHelper.GetCacheProvider();
            //var ass = cacheProviderConfig.Type.Split(',');
            //var compress = Helper.CreateObj<ICompress>(ass[0], ass[1]);
            //var retBuffer = compress.DeCompress(buffer, index, count);

            //stopwatch.Stop();
            //var span = stopwatch.ElapsedMilliseconds;

            //return retBuffer;
            throw new NullReferenceException();
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private T Deserialize<T>(byte[] buffer)
        {
            if (buffer == null)
            {
                return default(T);
            }
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    return (T)binaryFormatter.Deserialize(ms);
                }
            }
            catch
            {
                return default(T);
            }
        }

        public void Dispose()
        {
            // 不释放
            //if ((connection != null) && connection.IsConnected)
            //{
            //    sbLastMsg.Append("Dispose");
            //    connection.Close();
            //}
            sbLastMsg.Append("Dispose");
            var stacks = new StackTrace().GetFrames();
            LogHelper.Error(GetStackTrace(stacks));
        }
        private string GetStackTrace(StackFrame[] stacks)
        {

            string result = string.Empty;
            foreach (StackFrame stack in stacks)
            {
                result += string.Format("Call Stack:{0} {1} {2} {3}\r\n",
                    stack.GetFileName(),
                    stack.GetFileLineNumber(),
                    stack.GetFileColumnNumber(),
                    stack.GetMethod().ToString());
            }
            return result;
        }
        #endregion

        public static T Get<T>(string key)
        {
            var msg = string.Empty;
            return Get<T>(key, out msg);
        }
        public static T Get<T>(string key, out string msg)
        {
            msg = string.Empty;
            var cache = new RedisCache();
            {
                //string json = cache.StringGet(key);
                //if (string.IsNullOrEmpty(json))
                //{
                //    return default(T);
                //}

                //return JsonConvert.DeserializeObject<T>(json);
                var ret = cache.BinaryGet<T>(key);
                msg = cache.LastMsg;
                return ret;
            }

        }

        public static bool Add<T>(string key, T value, TimeSpan? ttl, string tmpServer = "")
        {
            var msg = string.Empty;
            return Add<T>(key, value, ttl, out msg, tmpServer);
        }


        public static bool Add<T>(string key, T value, TimeSpan? ttl, out string msg, string tmpServer = "")
        {
            msg = string.Empty;
            var cache = string.IsNullOrEmpty(tmpServer) ? new RedisCache() : new RedisCache(tmpServer);
            {
                #region json
                //var json = JsonConvert.SerializeObject(value);
                //return cache.StringSet(key, json, ttl);
                #endregion

                var ret = cache.BinarySet<T>(key, value, ttl);
                msg = cache.LastMsg;
                return ret;
            }
        }

    }
}
