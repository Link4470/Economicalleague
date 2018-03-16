using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Infrastructure.Redis
{
    public class RedisOpearteResult
    {
        public string token { get; set; }
        public bool isok { get; set; }
        public int code { get; set; }
        public object data { get; set; }
        public string result { get; set; }

    }

    public static class RedisHashEnum
    {
        /// <summary>
        /// 存储后台用户信息
        /// </summary>
        public static string UserInfo = "UserInfo";
        /// <summary>
        /// 存储客户信息
        /// </summary>
        public static string CustomerInfo = "CustomerInfo";
        /// <summary>
        /// 存储被挤掉的客户信息
        /// </summary>
        public static string OtherWhereLoginInfo = "OtherWhereLoginInfo";
        /// <summary>
        /// 存储图片验证码
        /// </summary>
        public static string ValidateCode = "ValidateCode";

        /// <summary>
        /// 存储手机验证码
        /// </summary>
        public static string TelCode = "TelCode";
        /// <summary>
        /// 存储获取分页数据的时间(用于过滤分页中数据更新导致重复数据的使用)
        /// </summary>
        public static string PageingTime = "PageingTime";

    }
    public class RedisOpearteResult<T>
    {
        public string token { get; set; }
        public bool isok { get; set; }
        public int code { get; set; }
        public T data { get; set; }
        public string result { get; set; }

    }

}
