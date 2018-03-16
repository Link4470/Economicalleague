using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using Economicalleague.Infrastructure.Response;
using Economicalleague.Services.Customer;

namespace Economicalleague.Api.Functions
{
    /// <summary>
    /// 验证请求的token及secret是否有效(非公开接口需要添加该特性)
    /// [ApiAuthFilter(false,false)]
    /// 参数1为是否验证token,参数2为是否验证HTTP head中的secret
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class ApiAuthFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public bool isDoCheck { get; set; }

        public bool isCheckSecret { get; set; }

        public ApiAuthFilterAttribute()
        {
            isDoCheck = true;
            isCheckSecret = true;
        }
        public ApiAuthFilterAttribute(bool isCheck = true, bool checkSecret = true)
        {
            isDoCheck = isCheck;
            isCheckSecret = checkSecret;
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null || actionContext.Request == null || actionContext.Request.RequestUri == null)
            {
                return;
            }

            //如果header中没有密钥则返回错误
            if (isCheckSecret)
            {
                string secret = ConfigurationManager.AppSettings["AppVisitSecret"];
                if (!string.IsNullOrEmpty(secret))
                {
                    IEnumerable<string> headers;
                    if (!actionContext.Request.Headers.TryGetValues("secret", out headers))
                    {
                        actionContext.Response = GernalUnauthorizedResponse(actionContext, ErrCode.Unauthorized);
                        return;
                    }
                    if (headers.Count() == 0 || headers.FirstOrDefault() != secret)
                    {
                        actionContext.Response = GernalUnauthorizedResponse(actionContext, ErrCode.Unauthorized);
                        return;
                    }
                }
            }

            var requestHead = actionContext.Request.GetRequestHead(actionContext);
            //IEnumerable<string> tokens;
            //var tokensExist=actionContext.Request.Headers.TryGetValues("token", out tokens);
            //if (tokensExist)
            //{
            //    token = tokens.FirstOrDefault();
            //}
            string token = requestHead?.Token;   
            bool isTokenEmpty = string.IsNullOrEmpty(token);

            if (isTokenEmpty && isDoCheck)
            {
                ResponseContext result = new ResponseContext();
                result.Head.Ret = -1;
                result.Head.Code = ErrCode.TokenIsNotAllowedEmpty;
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, result);
                return;
            }
            if (!isTokenEmpty)
            {
                bool isExist = TokenSrv.IsTokenExist(token);
                if (!isExist)
                {
                    if (isDoCheck)
                    {
                        ResponseContext result = new ResponseContext();
                        result.Head.Ret = -1;
                        result.Head.Code = ErrCode.TokenPastDue;
                        //if (TokenSrv.IsOtherWhereLogin(token))
                        //{
                        //    result.Head.Msg = "您的帐号已在其他地方登录，请重新登录！";
                        //}
                        actionContext.Response = GernalUnauthorizedResponse(actionContext, result);
                        base.OnActionExecuting(actionContext);
                        return;
                    }
                }
            }
            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// Token验证失败
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public static HttpResponseMessage GernalUnauthorizedResponse(HttpActionContext actionContext, ErrCode erroCode)
        {
            ResponseContext result = new ResponseContext();
            result.Head.Ret = -1;
            result.Head.Code = erroCode;
            return actionContext.Request.CreateResponse(HttpStatusCode.OK, result);

        }

        /// <summary>
        /// 异常响应消息
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public static HttpResponseMessage GernalUnauthorizedResponse(HttpActionContext actionContext, ResponseContext respContext)
        {
            return actionContext.Request.CreateResponse(HttpStatusCode.OK, respContext);
        }

        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}