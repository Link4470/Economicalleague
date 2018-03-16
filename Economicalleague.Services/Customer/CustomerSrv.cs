using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common;
using Economicalleague.Common.Helper;
using Economicalleague.DatabaseDAL.Customer;
using Economicalleague.Domain.Customer;
using Economicalleague.EntityFramework;
using Economicalleague.Infrastructure.Response;
using Economicalleague.RedisDAL.Customer;
using Economicalleague.Services.PlatformOrders;

namespace Economicalleague.Services.Customer
{
    public class CustomerSrv : BaseService
    {
        private readonly CustomerDbDal _customerDbDal = new CustomerDbDal();
        private readonly CustomerRedisDal _customerRedisDal = new CustomerRedisDal();
        /// <summary>
        /// 将用户所有数据刷到Redis中
        /// </summary>
        //public void UpdateAllUserToRedis()
        //{
        //    CustomerRedisDal custRedisDal = new CustomerRedisDal();
        //    List<long> allCustomerId = _customerDbDal.GetAllCustomerId();
        //    TokenRedisDal tokenDal = new TokenRedisDal();
        //    allCustomerId.ForEach(x =>
        //    {
        //        var info = _customerDbDal.GetCustomerDetail(x);
        //        long customerId = info.CustomerID;
        //        custRedisDal.UpdateUserInfoInRedis(info);

        //        //刷备注名
        //        var nickNameList = custDbDal.GetCustomerNickName(customerId);
        //        custRedisDal.UpdateCustomerNickNameToRedis(customerId, nickNameList);

        //        //刷设备号
        //        if (!string.IsNullOrEmpty(info.DeviceTokensIos))
        //        {
        //            custRedisDal.UpdateCustomerDeviceToRedis(customerId, true, info.DeviceTokensIos);
        //        }
        //        else if (!string.IsNullOrEmpty(info.DeviceTokensAndroid))
        //        {
        //            custRedisDal.UpdateCustomerDeviceToRedis(customerId, false, info.DeviceTokensAndroid);
        //        }
        //        //将旧Redis用户token表更新
        //        tokenDal.UpdateTokenUser(info.CustomerTel, customerId);
        //    });
        //}

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="result">注册响应信息</param>
        /// <param name="user">用户信息</param>
        /// <param name="appType">注册来源</param>
        /// <param name="isFromShare">是否分享页面注册</param>
        /// <returns></returns>
        public CustomerInfo Register(out LoginResponseInfo result, RegisterInfo customer, int appType, bool isFromShare)
        {
            //if (!StringConvert.IsChinese(customer.CustomerName) || customer.CustomerName.Length < 2 || customer.CustomerName.Length > 6)
            //    ThrowResponseContextException(ErrCode.NameIsWrong);
            //SmsSendLogSrv smsSendSrv = new SmsSendLogSrv();
            //if (!smsSendSrv.IsRegSmsAuthenticated(customer.CustomerTel))
            //{
            //    //方便测试屏蔽
            //    //ThrowResponseContextException(ErrCode.InvalidCheckCode, "您没有通过短信验证,请先填写手机号进行短信验证");
            //}
            if (_customerDbDal.IsCustomerNameExists(customer.CustomerName))
                ThrowResponseContextException(ErrCode.customerNameAlreadyExist);

            DateTime now = DateTime.Now;
            CustomerInfo customerInfo = new CustomerInfo
            {
                Email = customer.Email,
                CustomerName = customer.CustomerName,
                PassWord = Md5.Encrypt(customer.PassWord),
                CreateTime = now,
                UpdateTime = now
            };
            customerInfo.CustomerId = _customerDbDal.AddCustomerInfo(customerInfo);

            CustomerDetail redisCustomerInfo = JsonHelper.ObjectToObject<CustomerDetail>(customerInfo);
            _customerRedisDal.AddCustomerInfo(redisCustomerInfo);

            result = new LoginResponseInfo
            {
                CustomerId = customerInfo.CustomerId,
                CustomerName = customerInfo.CustomerName,
            };

            return customerInfo;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginInfo">用户登录信息</param>
        /// <param name="token">用户token</param>
        /// <returns></returns>
        public WxLoginResponseInfo Login(LoginInfo loginInfo, out string token)
        {
            WxLoginResponseInfo result = null;
            try
            {
                ////检查手机号是否注册过
                //if (!userDbDal.IsCustomerNameExists(loginInfo.CustomerName, out userId))
                //{
                //    ThrowResponseContextException(ErrCode.AccountNotExist);
                //}

                //验证登录
                string weiXinSessionKeyUrl = ConfigurationManager.AppSettings["WeiXinSessionKeyUrl"];
                string weiXinAppId = ConfigurationManager.AppSettings["WeiXinAppId"];
                string weiXinSecret = ConfigurationManager.AppSettings["WeiXinSecret"];
                Dictionary<string, string> paramDic = new Dictionary<string, string>();
                paramDic.Add("appid", weiXinAppId);
                paramDic.Add("secret", weiXinSecret);
                paramDic.Add("js_code", loginInfo.code);
                paramDic.Add("grant_type", "authorization_code");
                var response = HttpHelper.Get(weiXinSessionKeyUrl, paramDic);
                if (string.IsNullOrEmpty(response) || response.Contains("errcode"))
                {
                    ThrowResponseContextException(ErrCode.TokenPastDue);
                }
                WxLoginInfo info = JsonHelper.JsonToObject<WxLoginInfo>(response);
                TokenOpearteResult tokenRes = TokenSrv.GetWXToken(info);
                if (!tokenRes.isok)
                {
                    token = string.Empty;
                    ThrowResponseContextException(ErrCode.TokenPastDue);
                }
                else
                {
                    token = tokenRes.token;
                    result = new WxLoginResponseInfo
                    {
                        thrdsession = tokenRes.token
                    };

                    Task.Run(() => AddWxUserInfo(loginInfo, info.openid));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
                throw;
            }
            return result;
        }

        private void AddWxUserInfo(LoginInfo info, string openId)
        {
            PlatformOrdersrv platsrv = new PlatformOrdersrv();

            CustomerDbDal customerDb = new CustomerDbDal();
            var userinfo = customerDb.WxGetUserInfo(openId);
            if (userinfo == null)
            {
                var adzone = platsrv.CreatePid();
                var user = new UserInfo()
                {

                    adzoneid = adzone.Data.Model,
                    //登陆成功保存下用户信息
                    avatarUrl = info.UserInfo.avatarUrl,
                    city = info.UserInfo.city,
                    country = info.UserInfo.country,
                    createtime = DateTime.Now,
                    gender = info.UserInfo.gender,
                    language = info.UserInfo.language,
                    nickName = info.UserInfo.nickName,
                    province = info.UserInfo.province,
                    openid = openId

                };
                //保存/更新登陆信息
                _customerDbDal.AddUserInfo(user);
            }
            else
            {
                if (string.IsNullOrEmpty(userinfo.adzoneid))
                {
                    var adzone = platsrv.CreatePid();
                    userinfo.adzoneid = adzone.Data.Model;
                }
                _customerDbDal.AddUserInfo(userinfo);
            }

        }

        /// <summary>
        /// 生成登录响应信息
        /// </summary>
        /// <param name="customer">用户信息</param>
        /// <param name="userRedisDal">userRedis数据访问对象</param>
        /// <param name="userDbDal">userDb数据访问对象</param>
        /// <returns></returns>
        private LoginResponseInfo GenerateLoginResponseInfo(CustomerInfo customer)
        {

            LoginResponseInfo responseInfo = new LoginResponseInfo
            {
                CustomerName = customer.CustomerName,
                CustomerTel = customer.CustomerTel,
                Email = customer.Email,
                Sex = customer.Sex,
                Avatar = customer.Avatar
            };

            return responseInfo;
        }
    }
}
