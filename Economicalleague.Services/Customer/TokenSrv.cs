using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.DatabaseDAL.Customer;
using Economicalleague.DatabaseDAL.Log;
using Economicalleague.Domain.Customer;
using Economicalleague.EntityFramework;
using Economicalleague.RedisDAL.Customer;

namespace Economicalleague.Services.Customer
{
    /// <summary>
    /// 用户token业务操作类
    /// </summary>
    public class TokenSrv
    {
        /// <summary>
        /// 获取新的用户token
        /// </summary>
        /// <param name="val">用户信息</param>
        /// <returns></returns>
        public static TokenOpearteResult GetNewToken(CustomerInfo val)
        {
            TokenRedisDal tokenDal = new TokenRedisDal();
            return tokenDal.GetNewToken(val);
        }
        public static TokenOpearteResult GetWXToken(WxLoginInfo val)
        {
            TokenRedisDal tokenDal = new TokenRedisDal();
            return tokenDal.GetWXToken(val);
        }
        /// <summary>
        /// 判断token是否存在
        /// </summary>
        /// <param name="token">token</param>
        public static bool IsTokenExist(string token)
        {
            TokenRedisDal tokenDal = new TokenRedisDal();
            return tokenDal.IsWXTokenExist(token);
        }

        /// <summary>
        /// 判断用户token是否过期
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="customerId">用户编号</param>
        /// <returns>token过期与否信息</returns>
        public static TokenExpireInfo IsTokenExpired(string token, long customerId)
        {
            TokenRedisDal tokenDal = new TokenRedisDal();
            //CustomerRedisDal userDal = new CustomerRedisDal();
            LogDbDAL _logDbDal = new LogDbDAL();
            var isTokenExist = tokenDal.IsTokenExist(token);
            if (!isTokenExist)
            {
                DateTime now = DateTime.Now;
                string lastLoginIp;
                DateTime? lastLoginTime = _logDbDal.CustomerLastLoginTime(out lastLoginIp, customerId); //上一次操作时间
                string message;
                if (lastLoginTime.HasValue)
                {
                    string time = (lastLoginTime.Value.Date == now.Date) ? lastLoginTime.Value.ToString("HH:mm") : lastLoginTime.Value.ToString("yyyy年MM月dd日HH:mm");
                    message = string.Format("你的账号于{0}在{1}设备上登录，如非本人操作，请尽快在本机上登录以取回账号。", time,
                        lastLoginIp);
                }
                else
                {
                    message = "你的账号已在其他设备上登录，如非本人操作，请尽快在本机上登录以取回账号。";
                }

                return new TokenExpireInfo
                {
                    IsExpire = true,
                    Message = message
                };
            }
            else
            {
                return new TokenExpireInfo
                {
                    IsExpire = false,
                    Message = string.Empty
                };
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        public static TokenOpearteResult TokenLogOff(string token)
        {
            TokenRedisDal tokenDal = new TokenRedisDal();
            return tokenDal.TokenLogOff(token);
        }

        /// <summary>
        /// 通过token 获取用户信息
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="user">用户信息</param>
        /// <param name="isOtherWhereLogin">是否在别处登录</param>
        /// <returns></returns>
        public static bool GetUserByToken(string token, out CustomerDetail user, out bool isOtherWhereLogin)
        {
            TokenRedisDal tokenDal = new TokenRedisDal();
            return tokenDal.GetUserByToken(token, out user, out isOtherWhereLogin);
        }

        public static bool WXGetUserByToken(string token, out UserInfo user)
        {
            TokenRedisDal tokenDal = new TokenRedisDal();
            CustomerDbDal customerDb = new CustomerDbDal();
            user = null;
            var openIdTemp = tokenDal.WXGetUserByToken(token);
            if (string.IsNullOrEmpty(openIdTemp))
            {
                return false;
            }
            else
            {
                var openId = openIdTemp.Split(';')[0];
                user = customerDb.WxGetUserInfo(openId);
                return true;
            }
        }

        /// <summary>
        /// 通过token获取CustomerID
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GetCustomerIDByToken(string token)
        {
            TokenRedisDal tokenDal = new TokenRedisDal();
            return tokenDal.GetCustomerIDByToken(token);
        }

        /// <summary>
        /// 判断token是否为被挤掉的用户
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="isDeletedToken">是否删除被挤掉的token数据</param>
        public static bool IsOtherWhereLogin(string token, bool isDeletedToken = false)
        {
            TokenRedisDal tokenDal = new TokenRedisDal();
            return tokenDal.IsOtherWhereLogin(token, isDeletedToken);
        }
    }
}
