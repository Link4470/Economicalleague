using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Domain.FavoritItems
{
    public class FavoritItemsPageList
    {

        public int totalcount { get; set; }
        public List<Items> list { get; set; }
    }

    public class Items
    {
        public long Numid { get; set; }
        //商品名称
        public string Title { get; set; }
        //商品链接
        public string ItemUrl { get; set; }
        //券后价
        public string CouponInfo { get; set; }
        //佣金
        public string Commission { get; set; }
        //销量
        public long? Volume { get; set; }
        //前端位置
        public int? Score { get; set; }
    }
}
