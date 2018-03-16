using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http.Controllers;
using Economicalleague.EntityFramework;
using Economicalleague.Infrastructure.Request;
using Economicalleague.Domain;
using Economicalleague.Domain.Customer;

namespace Economicalleague.Api.Functions
{
    public static class HttpRequestMessageExtensions
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        private const string RequestID = "Economicalleague_RequestID";
        private const string RequestHead = "Economicalleague_RequestHead";
        private const string LoginCustomer = "Economicalleague_LoginCustomer";
        private const string WXLoginUser = "WX_LoginUser";
        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            // Web-hosting. Needs reference to System.Web.dll
            if (request.Properties.ContainsKey(HttpContext))
            {
                var ctx = request.Properties[HttpContext] as HttpContextWrapper;
                if (ctx != null)
                {
                    HttpRequestBase httpRequest = ctx.Request;

                    string ip = httpRequest.Headers["x-real-ip"];
                    if (!string.IsNullOrEmpty(ip) && !string.Equals("unknown", ip, StringComparison.OrdinalIgnoreCase))
                    {
                        return ip;
                    }

                    return httpRequest.UserHostAddress;
                }
            }

            // Self-hosting. Needs reference to System.ServiceModel.dll. 
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                var remoteEndpoint = request.Properties[RemoteEndpointMessage] as RemoteEndpointMessageProperty;
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            // Self-hosting using Owin. Needs reference to Microsoft.Owin.dll. 
            if (request.Properties.ContainsKey(OwinContext))
            {
                dynamic owinContext = request.Properties[OwinContext];
                if (owinContext != null)
                {
                    return owinContext.Request.RemoteIpAddress;
                }
            }

            return null;
        }

        #region RequestID

        public static void SetRequestID(this HttpRequestMessage request, string requestID)
        {
            request.Properties[RequestID] = requestID;
        }

        public static string GetRequestID(this HttpRequestMessage request)
        {
            object requestID = null;

            if (request != null)
            {
                request.Properties.TryGetValue(RequestID, out requestID);
            }

            return requestID?.ToString();
        }

        #endregion

        #region RequestHead

        public static RequestHead GetRequestHead(this HttpRequestMessage request)
        {
            return request.GetRequestHead(null);
        }

        public static RequestHead GetRequestHead(this HttpRequestMessage request, HttpActionContext actionContext)
        {
            if (request == null)
            {
                return null;
            }

            object requestHead = null;
            if (!request.Properties.TryGetValue(RequestHead, out requestHead))
            {
                HttpMethod method = request.Method;
                if (method == HttpMethod.Get)
                {
                    //var dict = request.GetQueryNameValuePairs().ToLookup(t => t.Key.ToLower(), t => t.Value)
                    //    .ToDictionary(pair => pair.Key, pair => pair.FirstOrDefault());
                    //requestHead = GetRequestHeadFromQueryString(dict);

                    IEnumerable<string> tokens;
                    var tokensExist = request.Headers.TryGetValues("token", out tokens);
                    if (tokensExist)
                    {
                        RequestHead req = new RequestHead {Token = tokens.FirstOrDefault()};
                        requestHead = req;
                    }
                }
                else if (method == HttpMethod.Post)
                {
                    var req = actionContext.ActionArguments.FirstOrDefault();
                    requestHead = GetRequestHeadFromRequestContext(req);
                }

                request.Properties[RequestHead] = requestHead;
            }

            return requestHead as RequestHead;
        }

        #endregion

        #region LoginCustomer

        public static void SetLoginCustomer(this HttpRequestMessage request, CustomerDetail loginCustomer)
        {
            request.Properties[LoginCustomer] = loginCustomer;
        }
        public static void WXSetLoginUser(this HttpRequestMessage request, UserInfo loginUser)
        {
            request.Properties[WXLoginUser] = loginUser;
        }

        public static CustomerDetail GetLoginCustomer(this HttpRequestMessage request)
        {
            object loginCustomer = null;

            if (request != null)
            {
                request.Properties.TryGetValue(LoginCustomer, out loginCustomer);
            }

            return loginCustomer as CustomerDetail;
        }
        public static UserInfo WXGetLoginUser(this HttpRequestMessage request)
        {
            object loginUser = null;

            if (request != null)
            {
                request.Properties.TryGetValue(WXLoginUser, out loginUser);
            }

            return loginUser as UserInfo;
        }
        #endregion

        #region 私有方法

        private static RequestHead GetRequestHeadFromQueryString(Dictionary<string, string> dict)
        {
            string appType;
            string appVersion;
            string apiType;
            string apiVersion;
            string token = string.Empty;

            dict.TryGetValue("apptype", out appType);
            dict.TryGetValue("appversion", out appVersion);
            dict.TryGetValue("apitype", out apiType);
            dict.TryGetValue("apiversion", out apiVersion);

            if (dict.ContainsKey("token"))
            {
                dict.TryGetValue("token", out token);
            }
            else if (dict.ContainsKey("head.token"))
            {
                dict.TryGetValue("head.token", out token);
            }

            RequestHead requestHead = new RequestHead
            {
                AppType = !string.IsNullOrEmpty(appType) ? int.Parse(appType) : 0,
                AppVersion = appVersion,
                ApiType = !string.IsNullOrEmpty(apiType) ? int.Parse(apiType) : 0,
                ApiVersion = apiVersion,
                Token = token,
            };

            return requestHead;
        }

     

        private static RequestHead GetRequestHeadFromRequestContext(KeyValuePair<string, object> req)
        {
            dynamic reqContext = req.Value;

            if (reqContext != null)
            {
                try
                {
                    return reqContext.Head;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        #endregion
    }
}