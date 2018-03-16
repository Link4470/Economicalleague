using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common;
using Economicalleague.Domain.Constants;
using Economicalleague.Domain.Customer;
using Economicalleague.EntityFramework;
using Economicalleague.Infrastructure.Response;

namespace Economicalleague.RedisDAL.Customer
{
    public class TokenRedisDal : BaseRedisDal
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TokenRedisDal() : base()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity"></param>
        public TokenRedisDal(EconomicalleagueContainer entity) : base(entity)
        {

        }
        /// <summary>
        /// 微信登录用户Token列表Key
        /// </summary>
        private const string WXLoginListKey = RedisKeyConstants.WXLoginTokenList;
        /// <summary>
        /// 登录用户Token列表Key
        /// </summary>
        private const string LoginCustomerListKey = RedisKeyConstants.LoginCustomerTokenList;
        /// <summary>
        /// 登录用户账号列表Key
        /// </summary>
        private const string LoginCustomerNameListKey = RedisKeyConstants.LoginCustomerNameList;
        /// <summary>
        ///被挤掉的用户token列表key
        /// </summary>
        private const string CrowdedTokenKey = RedisKeyConstants.CrowdedToken;
        #region 用户token相关
        /// <summary>
        /// 获取新的用户token
        /// </summary>
        /// <param name="val">用户信息</param>
        /// <returns></returns>
        public TokenOpearteResult GetNewToken(CustomerInfo val)
        {
            string customerName = val.CustomerName;
            var oldtoken = RedisEntity.HashGet(LoginCustomerNameListKey, customerName);
            //被挤掉的用户token
            if (!string.IsNullOrEmpty(oldtoken))
            {
                var customerId = RedisEntity.HashGet(LoginCustomerListKey, oldtoken);
                CustomerRedisDal cusRedisDal = new CustomerRedisDal();
                CustomerDetail oldLoginInfo = cusRedisDal.GetCustomerDetail(long.Parse(customerId));
                //不是同一台设备
                if (oldLoginInfo != null)
                {
                    //添加到被挤掉的用户
                    string crowKey = CrowdedTokenKey + ":" + oldtoken;
                    RedisEntity.ItemSet<string>(crowKey, customerName);
                    RedisEntity.ItemSetExpire(crowKey, DateTime.Now.AddDays(1));
                }
                RedisEntity.HashRemove(LoginCustomerNameListKey, customerName);
                RedisEntity.HashRemove(LoginCustomerListKey, oldtoken);
                //记录删除的token列表
                LogHelper.Debug("移除旧的token:" + oldtoken + "|" + customerName);
            }
            string token = Guid.NewGuid().ToString();
            RedisEntity.HashSet(LoginCustomerListKey, token, val.CustomerId.ToString());
            RedisEntity.HashSet(LoginCustomerNameListKey, customerName, token);
            TokenOpearteResult result = new TokenOpearteResult
            {
                isok = true,
                token = token
            };
            return result;
        }

        /// <summary>
        /// 判断token是否存在
        /// </summary>
        /// <param name="token">token</param>
        public bool IsTokenExist(string token)
        {
            return !string.IsNullOrEmpty(RedisEntity.HashGet(LoginCustomerListKey, token));
        }
        /// <summary>
        /// 判断token是否存在
        /// </summary>
        /// <param name="token">token</param>
        public bool IsWXTokenExist(string token)
        {
            return !string.IsNullOrEmpty(RedisEntity.HashGet(WXLoginListKey, token));
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        public TokenOpearteResult TokenLogOff(string token)
        {
            var customerId = RedisEntity.HashGet(LoginCustomerListKey, token);
            string userHashId = string.Format(RedisKeyConstants.CustomerHashId, customerId);
            string customerName = RedisEntity.HashGet(userHashId, nameof(CustomerDetail.CustomerName));
            if (!string.IsNullOrEmpty(customerName))
            {
                RedisEntity.HashRemove(LoginCustomerNameListKey, customerName);
            }
            RedisEntity.HashRemove(LoginCustomerListKey, token);
            TokenOpearteResult result = new TokenOpearteResult
            {
                isok = true,
                result = "退出成功"
            };
            return result;
        }

        /// <summary>
        /// 通过token 获取用户信息
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="user"></param>
        /// <param name="isOtherWhereLogin"></param>
        /// <returns></returns>
        public bool GetUserByToken(string token, out CustomerDetail user, out bool isOtherWhereLogin)
        {
            bool isok = false;
            isOtherWhereLogin = false;
            user = null;
            //是否为被挤掉的用户
            if (!string.IsNullOrEmpty(RedisEntity.ItemGet<string>(CrowdedTokenKey + ":" + token)))
            {
                isOtherWhereLogin = true;
            }
            else
            {
                var customerId = RedisEntity.HashGet(LoginCustomerListKey, token);
                if (!string.IsNullOrEmpty(customerId))
                {
                    //获取用户信息
                    CustomerRedisDal cusRedisDal = new CustomerRedisDal();
                    CustomerDetail oldLoginInfo = cusRedisDal.GetCustomerDetail(long.Parse(customerId));
                    user = oldLoginInfo;
                }
                isok = true;
            }
            return isok;
        }

        public string WXGetUserByToken(string token)
        {
            var openId = RedisEntity.HashGet(WXLoginListKey, token);       
            return openId;
        }

        /// <summary>
        /// 获取token对应的CustomerID
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetCustomerIDByToken(string token)
        {
            return RedisEntity.HashGet(LoginCustomerListKey, token);
        }

        /// <summary>
        /// 判断是否为被挤掉的用户
        /// </summary>
        /// <param name="token">redis Key</param>
        /// <param name="deleteCrowdedToken">是否删除被挤掉的token数据</param>
        public bool IsOtherWhereLogin(string token, bool deleteCrowdedToken = false)
        {
            bool isCrowed = false;
            Guid tokenGuid;
            if (Guid.TryParse(token, out tokenGuid))
            {
                string key = CrowdedTokenKey + ":" + token;
                isCrowed = !string.IsNullOrEmpty(RedisEntity.ItemGet<string>(CrowdedTokenKey + ":" + token));

                if (isCrowed && deleteCrowdedToken)
                {
                    RedisEntity.ItemSetExpire(key, DateTime.Now.AddMinutes(5));
                }
            }
            return isCrowed;
        }

        /// <summary>
        /// 更新token信息
        /// </summary>
        /// <param name="customerName">用户账号</param>
        /// <param name="customerId">用户编号</param>
        public void UpdateTokenUser(string customerName, long customerId)
        {
            string token = RedisEntity.HashGet(LoginCustomerNameListKey, customerName);
            if (!string.IsNullOrEmpty(token))
            {
                RedisEntity.HashSet(LoginCustomerListKey, token, customerId.ToString());
            }
        }

        #endregion

        #region 微信登陆
        /// <summary>
        /// 获取新的用户token
        /// </summary>
        /// <param name="val">用户信息</param>
        /// <returns></returns>
        public TokenOpearteResult GetWXToken(WxLoginInfo val)
        {
            TimeSpan Expire = TimeSpan.FromDays(3);
            //string customerName = val.CustomerName;
            //var oldtoken = RedisEntity.HashGet(LoginCustomerNameListKey, customerName);
            ////被挤掉的用户token
            //if (!string.IsNullOrEmpty(oldtoken))
            //{
            //    var customerId = RedisEntity.HashGet(LoginCustomerListKey, oldtoken);
            //    CustomerRedisDal cusRedisDal = new CustomerRedisDal();
            //    CustomerDetail oldLoginInfo = cusRedisDal.GetCustomerDetail(long.Parse(customerId));
            //    //不是同一台设备
            //    if (oldLoginInfo != null)
            //    {
            //        //添加到被挤掉的用户
            //        string crowKey = CrowdedTokenKey + ":" + oldtoken;
            //        RedisEntity.ItemSet<string>(crowKey, customerName);
            //        RedisEntity.ItemSetExpire(crowKey, DateTime.Now.AddDays(1));
            //    }
            //    RedisEntity.HashRemove(LoginCustomerNameListKey, customerName);
            //    RedisEntity.HashRemove(LoginCustomerListKey, oldtoken);
            //    //记录删除的token列表
            //    LogHelper.Debug("移除旧的token:" + oldtoken + "|" + customerName);
            //}
            string token = Guid.NewGuid().ToString();

            RedisEntity.HashSet(WXLoginListKey, token, val.openid + ";" + val.session_key);
            RedisEntity.HashSetExpire(token, Expire);
            TokenOpearteResult result = new TokenOpearteResult
            {
                isok = true,
                token = token
            };
            return result;
        }

        /// <summary>
        /// 根据key获取openid
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string OpenId(string token)
        {
            string userHashId = string.Format(RedisKeyConstants.WXLoginTokenList);
            var temp = RedisEntity.HashGet(userHashId, token);
            string data = null;
            if (string.IsNullOrEmpty(temp))
            {
                ThrowResponseContextException(ErrCode.TokenPastDue);

            }
            else
            {
                data = RedisEntity.HashGet(userHashId, token).Split(';')[0];
            }
            return data;
        }
        #endregion
    }
}
