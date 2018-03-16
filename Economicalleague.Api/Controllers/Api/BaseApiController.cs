using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web;
using Economicalleague.Infrastructure.Response;
using Economicalleague.Infrastructure.Request;
using Economicalleague.Domain.Customer;
using Economicalleague.Api.Functions;
using Economicalleague.Infrastructure.ProjectException;
using System.Configuration;
using Economicalleague.Common;
using Newtonsoft.Json.Linq;
using Economicalleague.Services.Customer;
using Economicalleague.EntityFramework;

namespace Economicalleague.Api.Controllers
{
    public  class BaseApiController : ApiController
    {

        protected ResponseContext result = new ResponseContext();

        protected DateTime Now = DateTime.Now;

        private UserInfo _loginUser;
        protected UserInfo LoginUser
        {
            get
            {
                if (_loginUser == null)
                {
                    _loginUser = Request.WXGetLoginUser();
                    if (_loginUser == null)
                    {
                        result.Head.Ret = -1;
                        result.Head.Code = ErrCode.TokenPastDue;

                        throw new ResponseContextException(result);
                    }
                }
                return _loginUser;
            }
        }

        protected long LoginUserId
        {
            get
            {
                return LoginUser.userid;
            }
        }

        private int _appType = -1;
        /// <summary>
        /// app类型 默认为0:Web端，1：Android ，2：IOS ，3：微信，4：Pc端exe
        /// </summary>
        protected int AppType
        {
            get
            {
                if (_appType == -1)
                {
                    RequestHead requestHead;
                    try
                    {
                        requestHead = ActionContext.Request.GetRequestHead(ActionContext);
                        _appType = requestHead.AppType;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Exception(ex);
                        _appType = 1;
                        return _appType;
                    }
                }
                return _appType;
            }
        }

        /// <summary>
        /// 请求Head
        /// </summary>
        private RequestHead _requestHead;
        /// <summary>
        /// APP请求Head
        /// </summary>
        protected RequestHead RequestHead
        {
            get
            {
                if (_requestHead == null)
                {
                    try
                    {
                        _requestHead = ActionContext.Request.GetRequestHead(ActionContext);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Exception(ex);
                    }
                }
                return _requestHead;
            }
        }

        /// <summary>
        /// 验证请求token是否合法
        /// </summary>
        /// <param name="token">用户登录token</param>
        /// <param name="response">response对象</param>
        /// <returns>true:非法</returns>
        protected bool IsTokenInvalid<T>(string token, out ResponseContext<T> response)
        {
            response = new ResponseContext<T>();
            ErrCode errCode;
            bool isInvalid = IsTokenInvalid(token, out errCode);

            if (isInvalid)
            {
                response.SetErrorCode(errCode);
            }

            return isInvalid;
        }

        /// <summary>
        /// 验证请求token是否合法
        /// </summary>
        /// <param name="token">用户登录token</param>
        /// <param name="response">response对象</param>
        /// <param name="customer">返回当前登录的用户信息</param>
        /// <returns>true:非法</returns>
        protected bool IsTokenInvalid(string token, out ResponseContext response, out CustomerDetail customer)
        {
            response = new ResponseContext();
            customer = null;
            if (string.IsNullOrEmpty(token))
            {
                response.Head.Ret = -1;
                response.Head.Code = ErrCode.TokenIsNotAllowedEmpty;
                return true;
            }
            bool isOtherWhereLogin = false;
            if (!TokenSrv.GetUserByToken(token, out customer, out isOtherWhereLogin))
            {
                response.Head.Ret = -1;
                response.Head.Code = isOtherWhereLogin ? ErrCode.OtherWhereLogin : ErrCode.TokenPastDue;
                return true;
            }
            if (customer == null)
            {
                response.Head.Ret = -1;
                response.Head.Code = ErrCode.TokenPastDue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证请求token是否合法
        /// </summary>
        /// <param name="token">用户登录token</param>
        /// <param name="errCode">错误码</param>
        /// <returns>true:非法</returns>
        protected bool IsTokenInvalid(string token, out ErrCode errCode)
        {
            if (string.IsNullOrEmpty(token))
            {
                errCode = ErrCode.TokenIsNotAllowedEmpty;
                return true;
            }
            if (!TokenSrv.IsTokenExist(token))
            {
                errCode = ErrCode.TokenPastDue;
                return true;
            }
            if (TokenSrv.IsOtherWhereLogin(token))
            {
                errCode = ErrCode.OtherWhereLogin;
                return true;
            }
            if (string.IsNullOrEmpty(TokenSrv.GetCustomerIDByToken(token)))
            {
                errCode = ErrCode.TokenPastDue;
                return true;
            }
            errCode = 0;
            return false;
        }

        /// <summary>
        /// 验证请求参数是否为空
        /// </summary>
        /// <param name="response"></param>
        /// <param name="paras"></param>
        /// <returns>true:非法</returns>
        protected bool IsReqParaInvalid(out ResponseContext response, params object[] paras)
        {
            response = new ResponseContext();
            foreach (var para in paras)
            {
                if (para == null)
                {
                    response.Head.Ret = -1;
                    response.Head.Code = ErrCode.ParametersIsNotAllowedEmpty;
                    return true;
                }
                if (para is string)
                {
                    if (string.IsNullOrEmpty(para as string))
                    {
                        response.Head.Ret = -1;
                        response.Head.Code = ErrCode.ParametersIsNotAllowedEmpty;
                        return true;
                    }
                }
                if (para is JToken)
                {
                    if (string.IsNullOrEmpty(para.ToString()))
                    {
                        response.Head.Ret = -1;
                        response.Head.Code = ErrCode.ParametersIsNotAllowedEmpty;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 验证请求参数是否为空
        /// </summary>
        /// <param name="response"></param>
        /// <param name="paras"></param>
        /// <returns>true:非法</returns>
        protected bool IsReqParaInvalid<T>(out ResponseContext<T> response, params object[] paras)
        {
            response = new ResponseContext<T>();
            foreach (var para in paras)
            {
                if (para == null)
                {
                    response.Head.Ret = -1;
                    response.Head.Code = ErrCode.ParametersIsNotAllowedEmpty;
                    return true;
                }
                if (para is string)
                {
                    if (string.IsNullOrEmpty(para as string))
                    {
                        response.Head.Ret = -1;
                        response.Head.Code = ErrCode.ParametersIsNotAllowedEmpty;
                        return true;
                    }
                }
                if (para is JToken)
                {
                    if (string.IsNullOrEmpty(para.ToString()))
                    {
                        response.Head.Ret = -1;
                        response.Head.Code = ErrCode.ParametersIsNotAllowedEmpty;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 验证参数是否不是长整形
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        protected bool IsReqIntParaInvalid<T>(out ResponseContext<T> response, params object[] paras)
        {
            response = new ResponseContext<T>();
            foreach (var para in paras)
            {
                if (para == null)
                {
                    response.Head.Ret = -1;
                    response.Head.Code = ErrCode.ParametersIsNotAllowedEmpty;
                    return true;
                }
                if (para is string)
                {
                    if (string.IsNullOrEmpty(para as string))
                    {
                        response.Head.Ret = -1;
                        response.Head.Code = ErrCode.ParametersIsNotAllowedEmpty;
                        return true;
                    }
                    int paraInt;
                    bool isInt = int.TryParse((para as string), out paraInt);
                    if (!isInt)
                    {
                        response.Head.Ret = -1;
                        response.Head.Code = ErrCode.ParameterError;
                        return true;
                    }
                }
                if (para is JToken)
                {
                    if (string.IsNullOrEmpty(para.ToString()))
                    {
                        response.Head.Ret = -1;
                        response.Head.Code = ErrCode.ParametersIsNotAllowedEmpty;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 验证secret是否正确
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected bool IsSecretInvalid<T>(out ResponseContext<T> response)
        {
            response = new ResponseContext<T>();
            string secret = ConfigurationManager.AppSettings["AppVisitSecret"];
            if (!string.IsNullOrEmpty(secret))
            {
                IEnumerable<string> headers;
                if (!Request.Headers.TryGetValues("secret", out headers) || headers.Count() == 0 || headers.FirstOrDefault() != secret)
                {
                    response.Head.Ret = -1;
                    response.Head.Code = ErrCode.Unauthorized;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加事件日志
        /// </summary>
        /// <param name="actionEvent">事件</param>
        /// <param name="requestHead">请求参数头</param>
        /// <param name="customerID"></param>
        //protected void AddActionEvent(ActionEvent actionEvent, RequestHead requestHead, long? customerID = null)
        //{
        //    Task.Run(() =>
        //    {
        //        var actionEventEntity = new Log_ActionEvent
        //        {
        //            Type = (int)actionEvent.Type,
        //            PageID = actionEvent.PageID,
        //            OccurTime = DateTime.Now,
        //            CreateTime = DateTime.Now,
        //            CreateUser = customerID?.ToString() ?? LoginCustomer?.CustomerID.ToString(),
        //            AppType = requestHead.AppType,
        //            AppVersion = requestHead.AppVersion,
        //            ApiVersion = requestHead.ApiVersion,
        //            RemoteAddr = Request.GetClientIpAddress(),
        //        };
        //        ActionEventLogSrv actionLogSrv = new ActionEventLogSrv();
        //        actionLogSrv.AddActionEventLog(actionEventEntity);
        //    });
        //}

        /// <summary>
        /// 获取上一次获取分页时间
        /// </summary>
        /// <param name="funcName">方法名称</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        //protected DateTime GetLastPageingTime(string funcName, int pageIndex)
        //{
        //    SysConfigSrv sysSrv = new SysConfigSrv();
        //    return sysSrv.GetLastPageingTime(funcName, pageIndex, LoginCustomer.CustomerTel);
        //}
    }
}