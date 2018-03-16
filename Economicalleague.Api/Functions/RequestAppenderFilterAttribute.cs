using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Economicalleague.EntityFramework;
using Economicalleague.Services.Customer;
using Economicalleague.Domain.Customer;

namespace Economicalleague.Api.Functions
{
    public class RequestAppenderFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            var httpRequest = actionContext.Request;

            if (httpRequest != null)
            {
                string requestID = Guid.NewGuid().ToString();
                httpRequest.SetRequestID(requestID);

                var requestHead = httpRequest.GetRequestHead(actionContext);
                string token = requestHead?.Token;
                if (!string.IsNullOrEmpty(token))
                {
                    UserInfo user;
                    //if (TokenSrv.GetUserByToken(token, out customer, out isOtherWhereLogin))
                    //{
                    //    actionContext.Request.SetLoginCustomer(customer);
                    //}
                    if (TokenSrv.WXGetUserByToken(token, out user))
                    {
                        actionContext.Request.WXSetLoginUser(user);
                    }
                }
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}