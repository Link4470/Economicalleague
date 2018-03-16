using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Economicalleague.Infrastructure.Response;

namespace Economicalleague.Api.Functions
{
    /// <summary>
    /// api返回异常状态时响应处理
    /// </summary>
    public class ResponseDelegatingHandler : DelegatingHandler
    {
        async protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.MethodNotAllowed || response.StatusCode == HttpStatusCode.NotFound ||
                response.StatusCode == HttpStatusCode.BadRequest)
            {
                var customerError = new ResponseContext
                {
                    Head = new ResponseHead
                    {
                        Ret = -1,
                        Code = ErrCode.Failure,
                        Msg = (response.StatusCode == HttpStatusCode.MethodNotAllowed) ? "请求的资源上不允许请求方法（POST或GET）" :
                        (response.StatusCode == HttpStatusCode.NotFound ? "访问的页面不存在" : "请求错误")
                    }
                };
                response = request.CreateResponse(HttpStatusCode.OK, customerError);
            }
            return response;
        }
    }
}