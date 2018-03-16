using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Http;
using WebApiThrottle;
using Economicalleague.Api.Functions;
using Economicalleague.Domain.Customer;
using Economicalleague.Infrastructure.Request;
using Economicalleague.Infrastructure.Response;
using Economicalleague.Services.Customer;
using Economicalleague.EntityFramework;
using Economicalleague.Services.Log;

namespace Economicalleague.Api.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomerApiController : BaseApiController
    {
        /// <summary>
        /// 保存注册信息
        /// </summary>
        /// <param name="req">请求对象</param>
        /// <returns></returns>
        [Route("api/user/register"), HttpPost]
        [ApiAuthFilter(false)]
        [EnableThrottling(PerMinute = 10)]
        public ResponseContext<LoginResponseInfo> Register(RequestContext<RegisterInfo> req)
        {
            ResponseContext<LoginResponseInfo> responseContext = new ResponseContext<LoginResponseInfo>();
            if (IsReqParaInvalid(out responseContext, req))
                return responseContext;
            var customer = req.Content;
            if (IsReqParaInvalid(out responseContext, customer, customer.CustomerName, customer.PassWord, customer.Email))
                return responseContext;
            LoginResponseInfo responseInfo;
            var customerInfo = new CustomerSrv().Register(out responseInfo, customer, AppType, false);
            var tokenRes = TokenSrv.GetNewToken(customerInfo);
            responseContext.Head.Token = tokenRes.token;
            responseContext.Content = responseInfo;

            return responseContext;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns> 
        [ApiAuthFilter(false)]
        [Route("api/user/login"), HttpPost]
        public ResponseContext<WxLoginResponseInfo> Login(RequestContext<LoginInfo> req)
        {
            ResponseContext<WxLoginResponseInfo> responseContext = new ResponseContext<WxLoginResponseInfo>();
            if (IsReqParaInvalid(out responseContext, req))
                return responseContext;
            var customer = req.Content;
            if (IsReqParaInvalid(out responseContext, customer, customer.code))
                return responseContext;
            //if (string.IsNullOrEmpty(customer.CustomerName))
            //{
            //    responseContext.Head.Ret = -1;
            //    responseContext.Head.Code = ErrCode.customerNameIsNotAllowedEmpty;
            //    return responseContext;
            //}
            string token;
            responseContext.Content = new CustomerSrv().Login(customer,out token);
            responseContext.Head.Token = token;
            //if (!string.IsNullOrEmpty(token))
            //{
            //    Log_LoginLog log = new Log_LoginLog
            //    {
            //        LoginCustomer = userid,
            //        OccurTime = DateTime.Now,
            //        //AppVersion = req.Head.AppVersion,
            //        //AppType = req.Head.AppType,
            //        //ApiVersion = req.Head.ApiVersion,
            //        //ApiType = req.Head.ApiType,
            //        RemoteAddr = Request.GetClientIpAddress()
            //    };
            //    new LogSrv().AddLoginLog(log);
            //}
            return responseContext;
        }
    }
}