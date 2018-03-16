using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Economicalleague.Infrastructure.Response
{
    /// <summary>
    /// 请求响应体
    /// </summary>
    [Serializable]
    [DataContract]
    public class ResponseContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResponseContext()
        {
            this.Head = new ResponseHead();
            this.Content = null;
        }

        /// <summary>
        /// 响应头
        /// </summary>
        [DataMember]
        public ResponseHead Head { get; set; }

        /// <summary>
        /// 响应体
        /// </summary>
        [DataMember]
        public object Content { get; set; }
        
        /// <summary>
        /// 设置错误代码
        /// </summary>
        /// <param name="errCode"></param>
        public void SetErrorCode(ErrCode errCode)
        {
            this.Head.Ret = -1;
            this.Head.Code = errCode;
            this.Content = null;
        }

    }

    [Serializable]
    [DataContract]
    public class ResponseContext<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResponseContext()
        {
            this.Head = new ResponseHead();
            this.Content = default(T);
        }

        /// <summary>
        /// 响应头
        /// </summary>
        [DataMember]
        public ResponseHead Head { get; set; }

        /// <summary>
        /// 响应体
        /// </summary>
        [DataMember]
        public T Content { get; set; }

        /// <summary>
        /// 设置错误代码
        /// </summary>
        /// <param name="errCode"></param>
        public void SetErrorCode(ErrCode errCode)
        {
            this.Head.Ret = -1;
            this.Head.Code = errCode;
            this.Content = default(T);
        }
    }
}