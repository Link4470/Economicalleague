using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Domain.PlatformOrders
{
    public class OrderDetail:OrderList
    {
        public string CreateTime { get; set; }
        //平台服务费
        public string PlatformFee { get; set; }
        //佣金金额
        public string Commission { get; set; }
    }
}
