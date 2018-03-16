using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Economicalleague.Common.Helper
{
    public class HttpHelper
    {
        public static string Get(string url, IDictionary<string, string> paramDic = null, string authToken = null, Action<string> logFn = null)
        {
            Guid requestId = Guid.Empty;
            string paramStr = SerializeDictionary(paramDic);
            if (!string.IsNullOrEmpty(paramStr))
            {
                url = url + "?" + paramStr;
            }
            string result;
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                if (!string.IsNullOrEmpty(authToken))
                {
                    request.Headers.Add("Authorization", authToken);
                }
                if (logFn != null)
                {
                    requestId = Guid.NewGuid();
                    logFn($"[{requestId}][Get] {url}");
                }
                Task<HttpResponseMessage> sendTask = client.SendAsync(request);
                sendTask.Wait();
                HttpResponseMessage response = sendTask.Result.EnsureSuccessStatusCode();
                string responseContent = response.Content.ReadAsStringAsync().Result;
                if (logFn != null)
                {
                    logFn($"[{requestId}][Response] {responseContent}");
                }
                result = responseContent;
            }
            return result;
        }
        public static string SerializeDictionary(IDictionary<string, string> dic)
        {
            string str = null;
            if (dic != null && dic.Count > 0)
            {
                List<string> parameters = new List<string>();
                foreach (KeyValuePair<string, string> item in dic)
                {
                    parameters.Add(item.Key + "=" + HttpUtility.UrlEncode(item.Value, Encoding.UTF8));
                }
                str = string.Join("&", parameters.ToArray());
            }
            return str;
        }
    }
}
