using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Economicalleague.Api.Functions.HttpContextShim.SelfHost;

namespace Economicalleague.Api.Functions.HttpContextShim
{
    public class HttpContextHandler : DelegatingHandler
    {
        private const string HttpContextProperty = "MS_HttpContext";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            object contextPropertyValue;
            request.Properties.TryGetValue(HttpContextProperty, out contextPropertyValue);
            var context = contextPropertyValue as HttpContextBase;

            HttpContext.Current = new SelfHostHttpContext(request);
            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                var result = task.Result;
                if (context == null)
                {
                    ((SelfHostHttpContext)HttpContext.Current).SetResponse(result);
                }
                return result;
            });
        }
    }
}

