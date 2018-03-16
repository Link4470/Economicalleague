using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApiThrottle;
using Economicalleague.Api.Functions;

namespace Economicalleague.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            //所有webapi请求附加数据到Request
            config.Filters.Add(new RequestAppenderFilterAttribute());
            //所有webapi请求记录日志
            config.Filters.Add(new ApiLogTrackFilterAttribute());
            //所有webapi请求捕获异常
            config.Filters.Add(new ApiExceptionFilterAttribute());
            //api访问频率限制
            config.Filters.Add(new ApiThrottlingFilter()
            {
                Policy = new ThrottlePolicy()
                {
                    //ip配置区域
                    IpThrottling = true,
                    //端点限制策略配置会从EnableThrottling特性中获取。
                    EndpointThrottling = true
                }
            });
            //跨域
            config.EnableCors();
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "Home", action = "Index", id = RouteParameter.Optional }
            );

        }
    }
}
