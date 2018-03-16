using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using Economicalleague.Common.Models;
using Economicalleague.Common;
using Economicalleague.Infrastructure.Response;
using Economicalleague.Infrastructure.ProjectException;

namespace Economicalleague.Api.Functions
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var loginUser = context.Request.WXGetLoginUser();
            string requestID = context.Request.GetRequestID();
            string remoteAddr = context.Request.GetClientIpAddress();
            string actionName = context.ActionContext.ActionDescriptor.ActionName;
            string controllerName = context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;

            var exceptionLog = new ExceptionLog
            {
                ControllerName = controllerName,
                ActionName = actionName,
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace,
                RemoteAddr = remoteAddr,
                RequestID = requestID?.ToString(),
                LoginUser = loginUser?.userid.ToString(),
                OccurTime = DateTime.Now
            };

            if (context.Exception is ResponseContextException)
            {
                var responseContext = (context.Exception as ResponseContextException).ResponseContext;
                LogHelper.Info(JsonConvert.SerializeObject(responseContext));
                context.Response = context.Request.CreateResponse(HttpStatusCode.OK, responseContext);
                return;
            }
            else if (context.Exception is OperationCanceledException)
            {
                //已取消该操作不记录异常日志(客户端终止请求)
                LogHelper.Info(JsonConvert.SerializeObject(exceptionLog));
            }
            else
            {
                LogHelper.Exception(exceptionLog, context.Exception);
            }

            var customerError = new ResponseContext
            {
                Head = new ResponseHead
                {
                    Ret = -1,
                    Code = ErrCode.InnerError
                }
            };
            context.Response = context.Request.CreateResponse(HttpStatusCode.OK, customerError);
        }
    }
}