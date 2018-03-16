using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common;
using Economicalleague.Common.Util;
using Economicalleague.EntityFramework;
using Economicalleague.FavoritItemJob;
using Economicalleague.Services.FavoritItems;
using Economicalleague.Services.TaoBao;
using log4net;
using Quartz;
using Top.Api.Domain;
using Top.Api.Request;
using Top.Api.Response;


namespace FavoritItemJob
{
    public class FavoritItemJobService:IJob
    {
         FavoritItemsSrv _favoritItemsSrv=new FavoritItemsSrv();
         Log log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
        
        public void Execute(IJobExecutionContext context)
        {
            log.log("开始执行Job任务" + "----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            log.log("获取淘宝联盟后台分类id" +"----"+ DateTime.Now.ToString(CultureInfo.InvariantCulture));
            TaoBaoClient taoBaoClient = new TaoBaoClient();
            TbkUatmFavoritesGetRequest req = new TbkUatmFavoritesGetRequest
            {
                PageNo = 1L,
                PageSize = 50L,
                Fields = "favorites_title,favorites_id,type",
                Type = 1L
            };
            var responseresulttap = taoBaoClient.GetTaoBaoFavorites(req);
            _favoritItemsSrv.AddTabItems(responseresulttap);


            foreach (var data in responseresulttap.Results)
            {
                log.log("循环淘宝联盟后台分类id" + "----" + data.FavoritesId);
                //var result = _favoritItemsSrv.GetTaoBaoFavoritesItemsListAsync(data.FavoritesId);
             
                TbkUatmFavoritesItemGetResponse responseresult;
                var result = new ResponseDataList<UatmTbkItem>();
                var uid = Convert.ToInt64(TaoBaoConfig.UId);

                TbkUatmFavoritesItemGetRequest req2 = new TbkUatmFavoritesItemGetRequest()
                {
                    AdzoneId = uid,
                    FavoritesId = data.FavoritesId,
                    Fields = "num_iid,title,pict_url,small_images,reserve_price,zk_final_price,user_type,provcity,item_url,click_url,seller_id,volume,nick,shop_title,zk_final_price_wap,event_start_time,event_end_time,tk_rate,status,type,coupon_info,commission_rate,coupon_click_url,coupon_end_time",
                    PageNo = 1,
                    PageSize = 500
                };
                responseresult = taoBaoClient.GetFavoritesList(req2);
                responseresult.Results = responseresult.Results.Where(x => x.CouponInfo != null).ToList();
               
                //查找每一个商品到券信息
                foreach (var tamp in responseresult.Results)
                {
                    //var coupon = GetCouponInfo(data.NumIid);
                    var coupon = tamp.CouponInfo.Split('减')[1].Substring(0, tamp.CouponInfo.Split('减')[1].IndexOf("元", StringComparison.Ordinal));
                    var index = responseresult.Results.IndexOf(tamp);
                    if (!string.IsNullOrEmpty(coupon) && Convert.ToDouble(tamp.ZkFinalPrice) > Convert.ToDouble(coupon))
                    {
                        responseresult.Results[index].CouponInfo = coupon;
                    }
                    else
                    {
                        responseresult.Results[index].CouponInfo = "0";
                    }
                    log.log("插入数据库的物品id" + "----" + tamp.NumIid);
                }              
                _favoritItemsSrv.AddItems(responseresult,data.FavoritesId);

                log.log("插入数据库的列表id" + "----" + data.FavoritesId);
            }



        }
    }
}
