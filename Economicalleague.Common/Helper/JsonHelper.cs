using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using ServiceStack;
using Newtonsoft.Json;

namespace Economicalleague.Common
{
    public static class JsonHelper
    {
        /// <summary>
        /// 反序列化Json文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonFilePath"></param>
        /// <returns></returns>
        /// <remarks>Json文件中key必须是字符串，否则无法反序列化</remarks>
        public static T DeserializeFromFile<T>(string jsonFilePath) where T : class
        {
            if (!File.Exists(jsonFilePath)) return default(T);

            try
            {
                using (FileStream fs = File.Open(jsonFilePath, FileMode.Open))
                {
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    return JsonToObject<T>(Encoding.UTF8.GetString(bytes));
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="jsonDate"></param>
        /// <returns></returns>
        public static DateTime JsonToDateTime(string jsonDate)
        {
            try
            {
                string value = jsonDate.ToLower().Replace(@"\/date(", "").Replace(@")\/", "").Replace("\"", "");
                var kind = DateTimeKind.Utc;
                int index = value.IndexOf('+', 1);

                if (index == -1)
                    index = value.IndexOf('-', 1);

                if (index != -1)
                {
                    kind = DateTimeKind.Local;
                    value = value.Substring(0, index);
                }

                var javaScriptTicks = long.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);

                var initialJavaScriptDateTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

                var utcDateTime = new DateTime((javaScriptTicks * 10000) + initialJavaScriptDateTicks, DateTimeKind.Utc);

                DateTime dateTime;

                switch (kind)
                {
                    case DateTimeKind.Unspecified:
                        dateTime = DateTime.SpecifyKind(utcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
                        break;

                    case DateTimeKind.Local:
                        dateTime = utcDateTime.ToLocalTime();
                        break;

                    default:
                        dateTime = utcDateTime;
                        break;
                }

                return dateTime;
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 判断某个object有是否有名为PropertyName的属性
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">待查找的属性名</param>
        /// <returns>有则返回此属性值，没有则返回指定类型的默认值</returns>
        public static T GetObjectProperty<T>(object obj, string propertyName)
        {
            var returnValue = default(T);

            try
            {
                if (obj != null)
                {
                    var t = obj.GetType();
                    var p = t.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                    if (p.DeclaringType != null && t != p.DeclaringType) t = p.DeclaringType;

                    while (p != null && !p.CanRead)
                    {
                        t = t.BaseType;
                        if (t != null)
                            p = t.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                        else
                        {
                            p = null;
                            break;
                        }
                    }
                    if (p != null && p.CanRead)
                    {
                        returnValue = (T)(p.GetValue(obj, null));
                    }
                }

            }
            catch
            {
                returnValue = default(T);
            }

            return returnValue;
        }

        /// <summary>
        /// json字符串转换为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns></returns>
        public static T JsonToObject<T>(string jsonStr) where T : class
        {
            if (string.IsNullOrEmpty(jsonStr))
                return null;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonStr);
        }

        /// <summary>
        /// 对象转换为json字符串
        /// </summary>
        /// <param name="obj">需要转换的对象</param>
        /// <param name="isIgnoreNull">是否忽略Null节点</param>
        /// <returns></returns>
        public static string ObjectToJson(object obj, bool isIgnoreNull = true)
        {
            if (obj == null)
                return string.Empty;
            string jsonStr;
            if (isIgnoreNull)
            {
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                jsonStr = JsonConvert.SerializeObject(obj, jSetting);
            }
            else
            {
                jsonStr = JsonConvert.SerializeObject(obj);
            }
            return jsonStr;
        }

        /// <summary>
        /// 对象转换对象
        /// </summary>
        /// <param name="obj">需要转换的对象</param>
        /// <returns></returns>
        public static T ObjectToObject<T>(object obj) where T : class
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// 对象转换为属性字典
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="isIgnoreNull">是否忽略为null的值</param>
        /// <returns></returns>
        public static Dictionary<string, string> ObjectToDictionary(object obj, bool isIgnoreNull = true)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(obj);
                if (value == null)
                {
                    if (isIgnoreNull)
                        continue;
                    else
                    {
                        dict.Add(propertyInfo.Name, "");
                    }
                }
                else
                {
                    if (propertyInfo.PropertyType == typeof(String))
                    {
                        dict.Add(propertyInfo.Name, value.ToString());
                    }
                    else
                    {
                        dict.Add(propertyInfo.Name, ObjectToJson(value).TrimStart('"').TrimEnd('"'));
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// Redis Hash类型集合转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashEntity"></param>
        /// <returns></returns>
        public static T DictionaryToObject<T>(Dictionary<string, string> hashEntity)
        {
            var s = ObjectToJson(hashEntity);
            return s.FromJson<T>();
        }

        /// <summary>
        /// 将string转换为int(添加该方法以便ServiceStack.Common.dll生成时输出到目标文件夹)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int StringToInt(string value)
        {
            return StringExtensions.ToInt(value);
        }
    }
}
