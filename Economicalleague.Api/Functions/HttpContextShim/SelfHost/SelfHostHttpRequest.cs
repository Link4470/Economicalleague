using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web.Http.Hosting;

namespace Economicalleague.Api.Functions.HttpContextShim.SelfHost
{
    public class SelfHostHttpRequest : IHttpRequest
    {
        private const string LoopbackAddress = "127.0.0.1";

        public SelfHostHttpRequest(HttpRequestMessage request)
        {
            object remote;
            if (!request.Properties.TryGetValue(RemoteEndpointMessageProperty.Name, out remote)) return;
            UserHostAddress = ((RemoteEndpointMessageProperty)remote).Address;
            IsLocal = IsLocalFromRequest(request) || UserHostAddress == LoopbackAddress || MatchToLocalAddress(UserHostAddress);
            Inner = request;
        }

        public bool IsLocal { get; private set; }
        public string UserHostAddress { get; private set; }
        public object Inner { get; private set; }

        private static bool IsLocalFromRequest(HttpRequestMessage request)
        {
            object isLocalValue;
            request.Properties.TryGetValue(HttpPropertyKeys.IsLocalKey, out isLocalValue);
            var isLocal = (Lazy<bool>)isLocalValue;
            return isLocal != null && isLocal.Value;
        }

        private static bool MatchToLocalAddress(string remoteAddress)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var addresses = host.AddressList.Select(a => a.ToString());
            return addresses.Any(address => remoteAddress == address);
        }
    }
}