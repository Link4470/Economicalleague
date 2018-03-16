using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Economicalleague.Common;
using Economicalleague.Common.Helper;
using Economicalleague.DatabaseDAL.FavoritItems;
using Economicalleague.DatabaseDAL.PlatformOrders;
using Economicalleague.Domain.PlatformOrders;
using Economicalleague.EntityFramework;
using Economicalleague.RedisDAL.Customer;
using Economicalleague.Services.TaoBao;
using Top.Api.Request;
using Top.Api.Response;

namespace Economicalleague.Services.PlatformOrders
{

    public class PlatformOrdersrv : BaseService
    {
        private readonly PlatformOrdersDal _platformOrdersDal = new PlatformOrdersDal();
        private readonly FavoritItemsDal _favoritItemsDbDal = new FavoritItemsDal();
        private readonly TokenRedisDal _tokenRedisDal = new TokenRedisDal();

        /// <summary>
        /// 添加订单数据到本地数据库
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool AddOrder(List<TBrebateOrders> order)
        {
            var datalist = new List<TBrebateOrders>();
            foreach (var temp in order)
            {
                var data = new TBrebateOrders
                {
                    TradeId = temp.TradeId,
                    NumIid = temp.NumIid,
                    ItemText = temp.ItemText,
                    ItemNum = temp.ItemNum,
                    Price = temp.Price,
                    PayPrice = temp.PayPrice,
                    SellerNick = temp.SellerNick,
                    SellerShopTitle = temp.SellerShopTitle,
                    TkStatus = temp.TkStatus,
                    IncomeRate = temp.IncomeRate,
                    Proportions = temp.Proportions,
                    OrderType = temp.OrderType,
                    PubSharePreFee = temp.PubSharePreFee,
                    StatementPrice = temp.StatementPrice,
                    PrePay = temp.PrePay,
                    Commission = temp.Commission,
                    CommissionRate = temp.CommissionRate,
                    SubsidyRate = temp.SubsidyRate,
                    SubsidyFee = temp.SubsidyFee,
                    SubsidyType = temp.SubsidyType,
                    TerminalType = temp.TerminalType,
                    Tk3rdType = temp.Tk3rdType,
                    Category = temp.Category,
                    Tk3rdPubId = temp.Tk3rdPubId,
                    Tk3rdPubName = temp.Tk3rdPubName,
                    AdzoneId = temp.AdzoneId,
                    AdzoneName = temp.AdzoneName,
                    ClickTime = temp.ClickTime,
                    CreateTime = temp.CreateTime,
                    EarningTime = temp.EarningTime
                };
                //判断该订单是否在数据库中存在,不存在添加，存在则更新
                var isExist = _platformOrdersDal.IsOrderExist(data.TradeId);
                if (!isExist)
                {
                    datalist.Add(data);
                }
                else
                {
                    _platformOrdersDal.PlatformOrderUpdate(data.TradeId);
                }

            }
            var isSuccess = _platformOrdersDal.PlatformOrderAdd(datalist);
            return isSuccess;
        }
        /// <summary>
        /// 把excel数据变成model数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool ExcelToData(string filePath)
        {
            List<TBrebateOrders> orderses = new List<TBrebateOrders>();
            ExcelHelper excelHelper = new ExcelHelper(filePath);
            var data = excelHelper.ExcelToDataTable("", true);
            for (int i = 0; i < data.Rows.Count; i++)
            {
                TBrebateOrders brebateOrders = new TBrebateOrders
                {
                    CreateTime = string.IsNullOrEmpty(data.Rows[i][0].ToString()) ? DateTime.Parse("1900-01-01") : DateTime.Parse(data.Rows[i][0].ToString()),
                    ClickTime = string.IsNullOrEmpty(data.Rows[i][1].ToString()) ? DateTime.Parse("1900-01-01") : DateTime.Parse(data.Rows[i][1].ToString()),
                    ItemText = data.Rows[i][2].ToString(),
                    NumIid = string.IsNullOrEmpty(data.Rows[i][3].ToString()) ? -1 : long.Parse(data.Rows[i][3].ToString()),
                    SellerNick = data.Rows[i][4].ToString(),
                    SellerShopTitle = data.Rows[i][5].ToString(),
                    ItemNum = string.IsNullOrEmpty(data.Rows[i][6].ToString()) ? -1 : long.Parse(data.Rows[i][6].ToString()),
                    Price = string.IsNullOrEmpty(data.Rows[i][7].ToString()) ? -1 : double.Parse(data.Rows[i][7].ToString()),
                    TkStatus = data.Rows[i][8].ToString(),
                    OrderType = data.Rows[i][9].ToString(),
                    IncomeRate = data.Rows[i][10].ToString(),
                    Proportions = data.Rows[i][11].ToString(),
                    PayPrice = string.IsNullOrEmpty(data.Rows[i][12].ToString()) ? -1 : double.Parse(data.Rows[i][12].ToString()),
                    PubSharePreFee = string.IsNullOrEmpty(data.Rows[i][13].ToString()) ? -1 : double.Parse(data.Rows[i][13].ToString()),
                    StatementPrice = string.IsNullOrEmpty(data.Rows[i][14].ToString()) ? -1 : double.Parse(data.Rows[i][14].ToString()),
                    PrePay = string.IsNullOrEmpty(data.Rows[i][15].ToString()) ? -1 : double.Parse(data.Rows[i][15].ToString()),
                    EarningTime = string.IsNullOrEmpty(data.Rows[i][16].ToString()) ? DateTime.Parse("1900-01-01") : DateTime.Parse(data.Rows[i][16].ToString()),
                    CommissionRate = data.Rows[i][17].ToString(),
                    Commission = string.IsNullOrEmpty(data.Rows[i][18].ToString()) ? -1 : double.Parse(data.Rows[i][18].ToString()),
                    SubsidyRate = data.Rows[i][19].ToString(),
                    SubsidyFee = string.IsNullOrEmpty(data.Rows[i][20].ToString()) ? -1 : double.Parse(data.Rows[i][20].ToString()),
                    SubsidyType = data.Rows[i][21].ToString(),
                    TerminalType = data.Rows[i][22].ToString(),
                    Tk3rdType = data.Rows[i][23].ToString(),
                    TradeId = string.IsNullOrEmpty(data.Rows[i][24].ToString()) ? -1 : long.Parse(data.Rows[i][24].ToString()),
                    Category = data.Rows[i][25].ToString(),
                    Tk3rdPubId = string.IsNullOrEmpty(data.Rows[i][26].ToString()) ? -1 : long.Parse(data.Rows[i][26].ToString()),
                    Tk3rdPubName = data.Rows[i][27].ToString(),
                    AdzoneId = string.IsNullOrEmpty(data.Rows[i][28].ToString()) ? -1 : long.Parse(data.Rows[i][28].ToString()),
                    AdzoneName = data.Rows[i][29].ToString()
                };
                orderses.Add(brebateOrders);
            }
            return AddOrder(orderses);
        }
        public bool UploadExcelOrder(HttpPostedFileBase file)
        {
            UploadHelper helper = new UploadHelper();
            string filepath = helper.SaveFile(file);
            return ExcelToData(filepath);
        }
        /// <summary>
        /// 创建广告位id
        /// </summary>
        /// <returns></returns>
        public TbkAdzoneCreateResponse CreatePid()
        {
            var taoBaoClient = new TaoBaoClient();
            var result = new TbkAdzoneCreateResponse();
            var siteid = Convert.ToInt64(TaoBaoConfig.TaoBaoSiteId);
            var name = TaoBaoConfig.AdzoneName;
            try
            {
                var req = new TbkAdzoneCreateRequest()
                {
                    AdzoneName = name,
                    SiteId = siteid
                };
                var responseresult = taoBaoClient.CreatePid(req);
                //pid只取最后一段信息
                string[] arr = responseresult.Data.Model.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);//按照‘-分割并去除空
                responseresult.Data.Model = arr[3];
                result = responseresult;
            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;
        }

        public PlatformOrdersModel GetIncomInfo(string adzoneid)
        {
            //再redis中找到openid
            //var userid = _tokenRedisDal.OpenId(openId);
            //根据微信openid找到对应的pid
            //var pidinfo = _platformOrdersDal.GetPid(openId);
            if (string.IsNullOrEmpty(adzoneid))
            {
                return null;
            }
            var pid = Convert.ToInt64(adzoneid);
            var result = new PlatformOrdersModel();
            try
            {
                //预估收入
                double? tempPrePay = 0.0;
                //结算金额
                double? temStatementPrice = 0.0;

                double? temBalance = 0.0;


                var list = _platformOrdersDal.GetOrderList(pid);
                foreach (var data in list)
                {
                    tempPrePay += data.PrePay;
                    temStatementPrice += data.StatementPrice;
                    temBalance += data.Commission;
                }
                result.PrePay = tempPrePay;
                result.StatementPrice = temStatementPrice;
                result.Balance = temBalance * 0.8;
            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;

        }
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="openId">微信openId</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<OrderList> GetBrebateOrderses(string openId, int pageSize = 10, int pageIndex = 0)
        {
            var result = new List<OrderList>();
            var orderdata = new OrderList();

            ////根据微信openid找到对应的pid
            //var pidinfo = _platformOrdersDal.GetPid(openId);
            //if (string.IsNullOrEmpty(pidinfo.adzoneid))
            //{
            //    return null;
            //}
            var pid = Convert.ToInt64(openId);
            try
            {
                var list = _platformOrdersDal.GetOrderList(pid).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                foreach (var temp in list)
                {
                    orderdata.Title = temp.ItemText;
                    orderdata.PrePay = temp.PrePay.ToString();
                    orderdata.Price = temp.Price.ToString();
                    orderdata.PicUrl = GetPicUrl(temp.NumIid);
                    orderdata.TradeId = temp.TradeId;
                    result.Add(orderdata);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;
        }
        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="tradeId">订单编号</param>
        /// <returns></returns>
        public OrderDetail GetOrderDetail(long tradeId)
        {
            var platformFee = Convert.ToDouble(TaoBaoConfig.PlatformFee);
            var result = new OrderDetail();
            try
            {
                var data = _platformOrdersDal.GetOrderDetail(tradeId);
                result.Commission = data.Commission.ToString(CultureInfo.InvariantCulture);
                result.CreateTime = data.CreateTime?.ToString("yyyy-MM-dd HH:mm") ?? "未知时间";
                result.PlatformFee = (data.Commission * platformFee).ToString(CultureInfo.InvariantCulture);
                result.PicUrl = GetPicUrl(data.NumIid);
                result.PrePay = data.PrePay.ToString();
                result.Price = data.Price.ToString();
                result.Title = data.ItemText;
                result.TradeId = data.TradeId;

            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;
        }


        //public TBrebateOrders GetBrebateOrderdetail(string openId, long traid)
        //{
        //    var result = new TBrebateOrders();
        //    //根据微信openid找到对应的pid
        //    var pidinfo = _platformOrdersDal.GetPid(openId);
        //    if (string.IsNullOrEmpty(pidinfo.adzoneid))
        //    {
        //        return null;
        //    }
        //    var pid = Convert.ToInt64(pidinfo.adzoneid);
        //    try
        //    {
        //        var data = _platformOrdersDal.GetOrderDetail(traid);
        //        result = data;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Exception(ex);
        //    }
        //    return result;
        //}

        public string GetPicUrl(long numIid)
        {
            return _favoritItemsDbDal.GetUrl(numIid);
        }

        public PlatformOrderPageList GetOrderList(long tradeid, long pid, int pageIndex = 1, int pageSize = 10)
        {
            PlatformOrdersDal platformOrdersDal = new PlatformOrdersDal();
            return platformOrdersDal.GetOrderList(tradeid, pid, pageIndex, pageSize);
        }
    }
}
