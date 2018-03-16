using Economicalleague.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace WebApiThrottle
{
    /// <summary>
    /// api访问频率过滤器
    /// </summary>
    public class ApiThrottlingFilter : ThrottlingFilter
    {
        /// <summary>
        /// 自定义response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <param name="responseCode"></param>
        /// <param name="retryAfter"></param>
        /// <returns></returns>
        protected override HttpResponseMessage QuotaExceededResponse(HttpRequestMessage request, object content, HttpStatusCode responseCode, string retryAfter)
        {
            ResponseContext result = new ResponseContext();
            result.Head.Ret = -1;
            result.Head.Code = ErrCode.ApiRateLimits;
            return request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}