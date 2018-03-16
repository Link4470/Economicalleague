using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Domain.PlatformOrders;
using Economicalleague.EntityFramework;

namespace Economicalleague.DatabaseDAL.PlatformOrders
{
    public class PlatformOrdersDal : BaseDbDal
    {

        /// <summary>
        /// 保存平台分享的订单信息到本地数据库
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool PlatformOrderAdd(List<TBrebateOrders> order)
        {
            EconomicalleagueEntity.TBrebateOrders.AddRange(order);
            return EconomicalleagueEntity.SaveChanges() > 0;
        }
        /// <summary>
        /// 更新订单信息
        /// </summary>
        /// <param name="tradid"></param>
        public void PlatformOrderUpdate(long tradid)
        {
            var entity = EconomicalleagueEntity.TBrebateOrders.FirstOrDefault(x => x.TradeId == tradid);
            EconomicalleagueEntity.TBrebateOrders.AddOrUpdate(entity);
        }

        /// <summary>
        /// 判断订单在数据库中是否已经存在
        /// </summary>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        public bool IsOrderExist(long tradeId)
        {
            return EconomicalleagueEntity.TBrebateOrders.Any(x => x.TradeId == tradeId);
        }
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public List<TBrebateOrders> GetOrderList(long pid)
        {
            var result = EconomicalleagueEntity.TBrebateOrders
                .Where(x => x.AdzoneId == pid && x.TkStatus.Contains("订单结算")).ToList();
            return result;
        }
        public PlatformOrderPageList GetOrderList(long tradeid, long pid, int pageIndex = 1, int pageSize = 10)
        {
            //PlatformOrderPageList result = new PlatformOrderPageList();
            var listTemp = EconomicalleagueEntity.TBrebateOrders.AsNoTracking().AsQueryable();
            if (tradeid > 0)
            {

                //listTemp = listTemp.Where(x => SqlFunctions.PatIndex("%"+ tradeid.ToString() + "%", x.TradeId.ToString()) > 0);
                listTemp = listTemp.Where(x => x.TradeId.ToString().Contains(tradeid.ToString()));
            }
            if (pid > 0)
            {
                listTemp = listTemp.Where(x => x.AdzoneId.ToString().Contains(pid.ToString()));
            }
            var listTemp2 = listTemp.OrderByDescending(x => x.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var resultlist = listTemp2.Select(x => new Orders
            {
                OrderId = x.TradeId.ToString(),
                AdzoneId = x.AdzoneId.ToString(),
                TkStatus = x.TkStatus,
                ItemText = x.ItemText,
                Price = x.Price.ToString(),
                ItemNum = x.ItemNum.ToString(),
                PayPrice = x.PayPrice.ToString(),
                StatementPrice = x.StatementPrice.ToString(),
                PrePay = x.PrePay.ToString(),
                Commission = x.Commission.ToString()
            });
            return new PlatformOrderPageList
            {
                totalcount = listTemp.Count(),
                list = resultlist.ToList()
            };
        }
        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        public TBrebateOrders GetOrderDetail(long tradeId)
        {
            var result = EconomicalleagueEntity.TBrebateOrders
                .FirstOrDefault(x => x.TradeId == tradeId);
            return result;
        }
        /// <summary>
        /// 通过openid找到pid
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public UserInfo GetPid(string openId)
        {
            var result = EconomicalleagueEntity.UserInfo.FirstOrDefault(x => x.openid == openId);
            return result;
        }
    }
}
