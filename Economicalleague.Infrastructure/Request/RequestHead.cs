using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Economicalleague.Infrastructure.Request
{
    public class RequestHead
    {
        /// <summary>
        /// Token信息
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// app类型 默认为0:Web端，1：Android ，2：IOS ，3：微信，4：Pc端exe
        /// </summary>
        public int AppType { get; set; }
        /// <summary>
        /// APP版本号
        /// </summary>
        public string AppVersion { get; set; }
        /// <summary>
        ///  Api版本号
        /// </summary>
        public string ApiVersion { get; set; }
        /// <summary>
        /// Api类型(可为空，留作扩展)
        /// </summary>
        public int ApiType { get; set; }
    }
}