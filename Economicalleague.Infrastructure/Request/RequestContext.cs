using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Economicalleague.Infrastructure.Request
{
    public class RequestContext<T>
    {
        /// <summary>
        /// 请求头
        /// </summary>
        public RequestHead Head { get; set; }

        /// <summary>
        /// 请求体
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public RequestContext()
        {
            this.Head = new RequestHead();
            this.Content = default(T);
        }

        #region
        public bool CheckValid()
        {
            if (Head == null || Content == null)
            {
                return false;
            }

            return true;
        }
        #endregion
    }

    /// <summary>
    /// 空的请求内容类型
    /// </summary>
    public class EmptyContent { }
}
