using Economicalleague.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Domain.PlatformOrders
{
    public class PlatformOrderPageList
    {
        public int totalcount { get; set; }
        public List<Orders> list { get; set; }
    }
    public class Orders 
    {
        public string OrderId { get; set; }
        public string AdzoneId { get; set; }
        public string TkStatus { get; set; }
        public string ItemText { get; set; }
        public string Price { get; set; }
        public string ItemNum { get; set; }
        public string PayPrice { get; set; }
        public string StatementPrice { get; set; }
        public string PrePay { get; set; }
        public string Commission { get; set; }
    }
}
