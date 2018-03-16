using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Common
{
    public class RedisSetting
    {
        #region
        public static int CompressFlag { get; set; }
        public static int CompressThreshold { get; set; }
        public static int ConnectFlag { get; set; }

        public static string Server { get; set; }
        public static int Port { get; set; }
        public static string Pwd { get; set; }
        public static bool Ssl { get; set; }
        public static bool AbortOnConnectFail { get; set; }
        public static int KeepAlive { get; set; }
        public static int ConnectRetry { get; set; }

        public static int SyncTimeout { get; set; }
        public static int ConnectTimeout { get; set; }

        #endregion
       

        static RedisSetting()
        {
            var tmp = string.Empty;

            tmp = "RedisCache_Flag_Zip";
            CompressFlag = ConfigurationHelper.GetInt(tmp);

            tmp = "RedisCache_Size_CompressThreshold";
            CompressThreshold = ConfigurationHelper.GetInt(tmp);

            tmp = "RedisCache_ConnectFlag";
            ConnectFlag = ConfigurationHelper.GetInt(tmp);

            tmp = "Redis_Server";
            Server = ConfigurationHelper.GetString(tmp);

            tmp = "Redis_Port";
            Port = ConfigurationHelper.GetInt(tmp, 6379);

            tmp = "Redis_Pwd";
            Pwd = ConfigurationHelper.GetString(tmp);

            tmp = "Redis_Ssl";
            Ssl = ConfigurationHelper.GetBool(tmp);

            tmp = "Redis_AbortOnConnectFail";
            AbortOnConnectFail = ConfigurationHelper.GetBool(tmp);

            tmp = "Redis_KeepAlive";
            KeepAlive = ConfigurationHelper.GetInt(tmp, 60);

            tmp = "Redis_ConnectRetry";
            ConnectRetry = ConfigurationHelper.GetInt(tmp, 3);

            tmp = "Redis_SyncTimeout";
            SyncTimeout = ConfigurationHelper.GetInt(tmp, 1000);

            tmp = "Redis_ConnectTimeout";
            ConnectTimeout = ConfigurationHelper.GetInt(tmp, 10000);

        }
    }
}