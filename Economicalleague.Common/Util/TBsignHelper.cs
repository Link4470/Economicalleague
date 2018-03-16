using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Economicalleague.Common.Util
{
    public class TBsignHelper
    {
        /// <summary>
        /// 用md5加密就行了
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="secret"></param>
        /// <param name="signMethod">“md5”</param>
        /// <returns></returns>
        public static string SignTopRequest(IDictionary<string, string> parameters, string secret)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();

            query.Append(secret);

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }

            // 第三步：使用MD5/HMAC加密
            query.Append(secret);
            var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            foreach (var t in bytes)
            {
                result.Append(t.ToString("X2"));
            }

            return result.ToString();
        }
    }
}
