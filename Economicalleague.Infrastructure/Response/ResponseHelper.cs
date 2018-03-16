using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Economicalleague.Common;

namespace Economicalleague.Infrastructure.Response
{

    /// <summary>
    /// 工具类
    /// </summary>
    public class ResponseHelper
    {
        public static HttpResponseMessage GetResponse(string content, string format = "application/json")
        {

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, format)
            };
        }

        /// <summary>
        /// json方式返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetResponseJson<T>(T v, string format = "application/json")
        {
            string ret = JsonConvert.SerializeObject(v, new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
            //string ret = GetJson(v);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(ret, System.Text.Encoding.UTF8, format)
            };
        }

        public static string GetJson<T>(T obj)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, obj);
                string szJson = Encoding.UTF8.GetString(stream.ToArray());
                return szJson;
            }
        }


        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (obj.GetType() == typeof(DateTime))
            {
                DateTime dt = (DateTime)obj;
                if (dt == DateTime.MinValue)
                {
                    dt = DateTime.MinValue.ToUniversalTime();
                    return dt;
                }
                return dt;
            }
            if (obj == null)
            {
                return null;
            }
            var q = from p in obj.GetType().GetProperties()
                    where (p.PropertyType == typeof(DateTime)) && (DateTime)p.GetValue(obj, null) == DateTime.MinValue
                    select p;
            //q.ToList().ForEach(p =>
            //{
            //    p.GetValue(obj,)
            //    p.SetValue(obj, DateTime.MinValue.ToUniversalTime(), null);
            //});
            return obj;
        }

        public static string GetJson<T>(T v, string format = "application/json")
        {
            string ret = JsonConvert.SerializeObject(v, new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return ret;
        }

        public static T DeserializeResponseJson<T>(HttpResponseMessage httpResponseMessage)
        {
            StringContent sc = (StringContent)httpResponseMessage.Content;
            string str = sc.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// json方式返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetResponseJsonWithMicrosoftDate<T>(T v, string format = "application/json")
        {
            string ret = JsonConvert.SerializeObject(v, new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            });
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(ret, System.Text.Encoding.UTF8, format)
            };
        }

        /// <summary>
        /// 获取响应头
        /// </summary>
        /// <param name="ret"></param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ResponseHead GetResponseHead(int ret, ErrCode code, string msg = "", bool NotAutoCalcMsg = false)
        {
            ResponseHead head = new ResponseHead();
            head.Ret = ret;
            head.Code = code;
            head.NotAutoCalcMsg = NotAutoCalcMsg;
            head.Msg = EnumHelper.GetEnumDescription(ErrCode.Sucess);
            if (!string.IsNullOrEmpty(msg))
            {
                head.Msg = msg;
            }

            return head;
        }

        public static ResponseHead GetSucessResponseHead()
        {
            return GetResponseHead(0, ErrCode.Sucess);
        }
        public static ResponseHead GetErrorResponseHead(ErrCode code, string msg = "")
        {
            return GetResponseHead(-1, code, msg);
        }

        public static ResponseHead GetRealErrorResponseHead(ErrCode code, params string[] parameter)
        {
            return GetResponseHead(-1, code, string.Format(EnumHelper.GetEnumDescription(code), parameter), true);
        }
    }

}