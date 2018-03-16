using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Domain.PlatformOrders
{
    public class OrderList
    {
        //商品标题
        public string Title { get; set;}
        //商品主图
        public string PicUrl { get; set; }
        //商品单价
        public string Price { get; set; }
        //预估收入
        public string PrePay { get; set; }
        public long TradeId { get; set; }

    }
}
