using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common;
using Economicalleague.EntityFramework;

namespace Economicalleague.Infrastructure.Redis
{
    /// <summary>
    /// Token管理
    /// </summary>
    public class BaseTokenManager
    {
        /// <summary>
        /// 刷新登录用户信息
        /// </summary>
        /// <param name="tokenID"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static RedisOpearteResult RefreshLoginTokenData(String tokenID, CustomerInfo val)
        {
            string key = GenerateRedisKey(RedisHashEnum.CustomerInfo, val.CustomerTel, tokenID);
            RedisOpearteResult result = new RedisOpearteResult
            {
                isok = RedisCommon.getInstance.SetObj<CustomerInfo>(key, val),
                token = tokenID,
                result = JsonConvert.SerializeObject(val)
            };
            return result;
        }

        /// <summary>
        /// 生成Redis存储的key
        /// </summary>
        /// <param name="typeKey">分类</param>
        /// <param name="telNo">手机号</param>
        /// <param name="token">令牌ID</param>
        /// <returns></returns>
        protected static string GenerateRedisKey(string typeKey, string telNo, string token)
        {
            return string.Format("{0}{1}_{2}", typeKey, telNo, token);
        }
    }
}
