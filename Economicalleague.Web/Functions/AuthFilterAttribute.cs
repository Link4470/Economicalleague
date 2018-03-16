using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using Economicalleague.Common.Cache;
using Economicalleague.Services;
using System.Net;

namespace Economicalleague.Web.Functions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthFilterAttribute : ActionFilterAttribute
    {
        public bool IsCheck { get; set; }
        public AuthFilterAttribute()
        {
            IsCheck = true;
        }
        public AuthFilterAttribute(bool isCheck = true)
        {
            IsCheck = isCheck;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsCheck)
            {
                return;
            }
            string token = string.Empty;
            try
            {
                token = context.HttpContext.Request.Headers["token"];
            }
            catch { }
            if (string.IsNullOrEmpty(token) || token == "null")
            {
                //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new HttpUnauthorizedResult("没有访问权限!");
            }
            var userId =
           CacheManager.Instance.Get<string>(IdentityCacheKeys.UserToken.AppendSuffix(token));
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new HttpUnauthorizedResult("没有访问权限!");
            }
            base.OnActionExecuting(context);
        }
    }
}