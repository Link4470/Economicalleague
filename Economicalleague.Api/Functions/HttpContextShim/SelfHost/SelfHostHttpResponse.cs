using System.Net.Http;

namespace Economicalleague.Api.Functions.HttpContextShim.SelfHost
{
    public class SelfHostHttpResponse : IHttpResponse
    {
        public SelfHostHttpResponse(HttpResponseMessage response)
        {
            Inner = response;
        }

        public object Inner { get; private set; }
    }
}