using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Common.Helper
{
    public class FilterHelper
    {
        public static string GetRequestContent(HttpContent httpContent)
        {
            string content = "";
            if (httpContent != null)
            {
                try
                {
                    Task<Stream> t = httpContent.ReadAsStreamAsync();
                    Encoding encoding = Encoding.UTF8;
                    Stream stream = t.Result;
                    stream.Position = 0;
                    StreamReader reader = new StreamReader(stream, encoding);
                    content = reader.ReadToEnd().ToString();
                    stream.Position = 0;
                }
                catch(Exception ex)
                {
                    
                }

            }
            return content;
        }
    }
}
