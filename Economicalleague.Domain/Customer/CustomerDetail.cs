using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.EntityFramework;

namespace Economicalleague.Domain.Customer
{
    public class CustomerDetail : CustomerInfo
    {

    }
    public class TokenExpireInfo
    {
        /// <summary>
        /// 是否过期
        /// </summary>
        public bool IsExpire { get; set; }
        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; }
    }
}
