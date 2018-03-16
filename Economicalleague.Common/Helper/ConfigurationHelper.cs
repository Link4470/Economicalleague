using System.Configuration;

namespace Economicalleague.Common
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public class ConfigurationHelper
    {
        public static object Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetString(string key)
        {
            return GetValue(key) ?? "";
        }

        public static string GetString(string key, string defaultvalue)
        {
            return ConfigurationHelper.GetValue(key) ?? defaultvalue;
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            int value = defaultValue;
            if (int.TryParse(ConfigurationManager.AppSettings[key], out value))
            {
                return value;
            }
            return defaultValue;
        }

        public static long GetLong(string key, long defaultValue = 0)
        {
            long value = defaultValue;
            if (long.TryParse(ConfigurationManager.AppSettings[key], out value))
            {
                return value;
            }
            return defaultValue;
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            bool value = defaultValue;
            if (bool.TryParse(ConfigurationManager.AppSettings[key], out value))
            {
                return value;
            }
            return defaultValue;
        }

        public static float GetFloat(string key, float defaultValue = 0)
        {
            float value = defaultValue;
            if (float.TryParse(ConfigurationManager.AppSettings[key], out value))
            {
                return value;
            }
            return defaultValue;
        }

        protected static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}