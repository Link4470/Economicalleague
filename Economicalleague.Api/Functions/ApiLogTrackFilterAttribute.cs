using System;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Economicalleague.Common;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Web;
using Economicalleague.Common.Helper;

namespace Economicalleague.Api.Functions
{
    public class ApiLogTrackFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            StringBuilder sbMsg = new StringBuilder();

            var httpRequest = actionContext.Request;
            string content = "无请求内容";
            string requestID = string.Empty;

            if (httpRequest != null)
            {
                requestID = httpRequest.GetRequestID();

                if (httpRequest.Content.IsMimeMultipartContent())
                {
                    var formData = HttpContext.Current.Request.Form;
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    foreach (var key in formData.AllKeys)
                    {
                        dict.Add(key, formData.Get(key));
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    content = "文件上传:" + js.Serialize(dict);
                }
                else if (httpRequest.Method != HttpMethod.Get)
                {
                    content = FilterHelper.GetRequestContent(httpRequest.Content);
                }
            }

            sbMsg.AppendLine($"RequestID:{ requestID }");
            sbMsg.AppendLine($"{ httpRequest.Method } { httpRequest.RequestUri } HTTP/{ httpRequest.Version }");
            sbMsg.AppendFormat("ControllerName={0},ActionName={1},Msg={2}",
                    actionContext.ControllerContext.ControllerDescriptor.ControllerName,
                    actionContext.ActionDescriptor.ActionName,
                    content);
            LogHelper.Info(sbMsg.ToString());
        }


        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            StringBuilder sbMsg = new StringBuilder();
            string content = string.Empty;
            string requestID = actionExecutedContext.Request.GetRequestID();

            sbMsg.AppendLine($"RequestID:{ requestID }");

            var httpResponse = actionExecutedContext.Response;
            if (httpResponse != null)
            {
                sbMsg.AppendLine($"HTTP/{ httpResponse.Version } { (int)httpResponse.StatusCode } { httpResponse.StatusCode }");
                content = FilterHelper.GetRequestContent(httpResponse.Content);
            }
            else
            {
                content = "无响应内容";
            }
            sbMsg.AppendFormat("ControllerName={0},ActionName={1},Msg={2}",
                    actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName,
                    actionExecutedContext.ActionContext.ActionDescriptor.ActionName,
                    content);
            LogHelper.Info(sbMsg.ToString());
        }
    }
}