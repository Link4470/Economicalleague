using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Economicalleague.Common;
using Economicalleague.Common.Util;
using Economicalleague.DatabaseDAL.FavoritItems;
using Economicalleague.DatabaseDAL.PlatformOrders;
using Economicalleague.Domain.FavoritItems;
using Economicalleague.EntityFramework;
using Economicalleague.RedisDAL.Customer;
using Economicalleague.Services.TaoBao;
using Top.Api.Domain;
using Top.Api.Request;
using Top.Api.Response;


namespace Economicalleague.Services.FavoritItems
{
    public class FavoritItemsSrv : BaseService
    {
        private readonly FavoritItemsDal _favoritItemsDbDal = new FavoritItemsDal();
        private readonly PlatformOrdersDal _platformOrdersDal = new PlatformOrdersDal();
        private readonly TokenRedisDal _tokenRedisDal = new TokenRedisDal();

        #region 老代码已弃用
        //public ResponseDataList<UatmFavoritItems> GetItemsesList(long typeId, int pageSize = 10, int pageIndex = 1)
        //{
        //    var list = _favoritItemsDbDal.GetFavoritItemsesList(typeId, pageSize, pageIndex);
        //    return list;
        //}


        ///// <summary>
        ///// 把淘宝客商品信息数据保存到本地数据库
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //public bool AddFavoritesDetail(List<TbkFavoritesDetail> info)
        //{
        //    var dataList = new List<UatmFavoritItems>();
        //    foreach (var temp in info)
        //    {
        //        //小图转换成字符串用，隔开
        //        var strSmallImages = string.Join(",", temp.SmallImages.ToArray());
        //        var cendTime = Convert.ToDateTime(temp.CouponEndTime);
        //        var cStartTime = Convert.ToDateTime(temp.CouponStartTime);

        //        var data = new UatmFavoritItems
        //        {
        //            num_iid = temp.NumIid,
        //            title = temp.Title,
        //            pic_url = temp.PictUrl,
        //            small_images = strSmallImages,
        //            reserve_price = Convert.ToDouble(temp.ReservePrice),
        //            zk_final_price = Convert.ToDouble(temp.ZkFinalPrice),
        //            provcity = temp.Provcity,
        //            item_url = temp.ItemUrl,
        //            click_url = temp.ClickUrl,
        //            nick = temp.Nick,
        //            seller_id = temp.SellerId,
        //            volume = temp.Volume,
        //            tk_rate = Convert.ToDouble(temp.TkRate),
        //            zk_final_price_wap = Convert.ToDouble(temp.ZkFinalPriceWap),
        //            category = temp.Category,
        //            coupon_click_url = temp.CouponClickUrl,
        //            coupon_end_time = cendTime,
        //            coupon_info = temp.CouponInfo,
        //            coupon_start_time = cStartTime,
        //            coupon_total_count = temp.CouponTotalCount,
        //            coupon_remain_count = temp.CouponRemainCount,
        //            commission_rate = temp.CommissionRate,
        //            event_end_time = temp.EventEndTime,
        //            event_start_time = temp.EventStartTime,
        //            shop_title = temp.ShopTitle,
        //            status = temp.Status,
        //            type = temp.Type

        //        };
        //        dataList.Add(data);

        //    }
        //    var isSuccess = _favoritItemsDbDal.AddTbItemsList(dataList);
        //    return isSuccess;
        //}

        ///// <summary>
        ///// 添加选品库信息到本地数据库
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //public bool AddTbItemsList(List<TbkFavorites> info)
        //{
        //    var datalist = new List<TBItemsList>();
        //    foreach (var temp in info)
        //    {
        //        //var data = new TBItemsList
        //        //{
        //        //    type = temp.Type,
        //        //    favorites_id = temp.FavoritesId,
        //        //    favorites_title = temp.FavoritesTitle
        //        //};
        //        //datalist.Add(data);
        //    }
        //    var isSuccess = _favoritItemsDbDal.AddTbItemsList(datalist);
        //    return isSuccess;
        //}
        #endregion
        /// <summary>
        /// 淘宝联盟后台获取分类信息
        /// </summary>
        /// <returns></returns>
        public ResponseDataList<TbkFavorites> GetTaoBaoFavorites()
        {
            var responseresult = new TbkUatmFavoritesGetResponse();
            var result = new ResponseDataList<TbkFavorites>();
            TaoBaoClient taoBaoClient = new TaoBaoClient();
            TbkUatmFavoritesGetRequest req = new TbkUatmFavoritesGetRequest
            {
                PageNo = 1L,
                PageSize = 50L,
                Fields = "favorites_title,favorites_id,type",
                Type = 1L
            };
            try
            {
                responseresult = taoBaoClient.GetTaoBaoFavorites(req);
                if (responseresult.IsError)
                {
                    LogHelper.Error(responseresult.ErrMsg);
                    return null;
                }
                result.Count = responseresult.TotalResults;
                result.DataList = responseresult.Results;
                //开一个线程插入数据库
                //System.Threading.Tasks.Task.Run(() => AddTabItems(responseresult));

            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="favoritesId">选品库ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ResponseDataList<UatmTbkItem> GetTaoBaoFavoritesItemsListAsync(long favoritesId, int pageSize = 10, int pageIndex = 1)
        {
            TaoBaoClient taoBaoClient = new TaoBaoClient();
            TbkUatmFavoritesItemGetResponse responseresult;
            var result = new ResponseDataList<UatmTbkItem>();
            var uid = Convert.ToInt64(TaoBaoConfig.UId);

            TbkUatmFavoritesItemGetRequest req = new TbkUatmFavoritesItemGetRequest()
            {
                AdzoneId = uid,
                FavoritesId = favoritesId,
                Fields = "num_iid,title,pict_url,small_images,reserve_price,zk_final_price,user_type,provcity,item_url,click_url,seller_id,volume,nick,shop_title,zk_final_price_wap,event_start_time,event_end_time,tk_rate,status,type,coupon_info,commission_rate,coupon_click_url,coupon_end_time",
                PageNo = pageIndex,
                PageSize = pageSize
            };
            try
            {
                responseresult = taoBaoClient.GetFavoritesList(req);
                if (responseresult.IsError)
                {
                    LogHelper.Error(responseresult.ErrMsg);
                    return null;
                }
                else
                {

                    //查找每一个商品到券信息
                    foreach (var data in responseresult.Results.Where(x => x.CouponInfo != null))
                    {
                        //var coupon = GetCouponInfo(data.NumIid);
                        var coupon = data.CouponInfo.Split('减')[1].Substring(0, data.CouponInfo.Split('减')[1].IndexOf("元", StringComparison.Ordinal));
                        var index = responseresult.Results.IndexOf(data);
                        if (!string.IsNullOrEmpty(coupon) && Convert.ToDouble(data.ZkFinalPrice) > Convert.ToDouble(coupon))
                        {
                            responseresult.Results[index].CouponInfo = coupon;
                        }
                        else
                        {
                            responseresult.Results[index].CouponInfo = "0";
                        }
                    }

                }
                result.DataList = responseresult.Results.Where(x => x.CouponInfo != null).ToList();
                result.Count = result.DataList.Count;

                //开一个线程插入数据库
                System.Threading.Tasks.Task.Run(() => AddItems(responseresult, favoritesId));

            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;

        }

        /// <summary>
        /// 从本地数据库拿商品列表数据
        /// </summary>
        /// <param name="favoritesId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ResponseDataList<UatmFavoritItems> GetLocalFavoritesItemsList(int favoritesId, int pageSize = 10,
            int pageIndex = 1)
        {
            var result = new ResponseDataList<UatmFavoritItems>();
            result.DataList = _favoritItemsDbDal.GetLocalFavoritItemsesList(favoritesId).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            result.Count = _favoritItemsDbDal.GetLocalFavoritItemsesList(favoritesId).Count;
            return result;
        }


        /// <summary>
        /// 获取点击的商品详情
        /// </summary>
        /// <param name="favoritesId">选品库ID</param>
        /// <param name="numiid">商品ID</param>
        /// <param name="adzoneid"></param>
        /// <returns></returns>
        public UatmTbkItem GetFavoritItemsDetail(int favoritesId, long numiid)
        {
            var taoBaoClient = new TaoBaoClient();
            var uid = Convert.ToInt64(TaoBaoConfig.UId);
            var result = new UatmTbkItem();
            try
            {
                var req = new TbkUatmFavoritesItemGetRequest()
                {
                    AdzoneId = uid,
                    FavoritesId = favoritesId,
                    Fields = "num_iid,title,pict_url,small_images,reserve_price,zk_final_price,user_type,provcity,item_url,click_url,seller_id,volume,nick,shop_title,zk_final_price_wap,event_start_time,event_end_time,tk_rate,status,type,coupon_info,commission_rate,coupon_click_url,coupon_end_time",
                    PageNo = 1,
                    PageSize = 20
                };
                var responseresult = taoBaoClient.GetFavoritesList(req);
                result = responseresult.Results.FirstOrDefault(x => x.NumIid == numiid);

                if (!string.IsNullOrEmpty(result?.CouponInfo))
                {
                    var coupon = result.CouponInfo.Split('减')[1]
                        .Substring(0, result.CouponInfo.Split('减')[1].IndexOf("元", StringComparison.Ordinal));
                    result.CouponInfo = coupon;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;
        }

        /// <summary>
        /// 从本地获取商品详情
        /// </summary>
        /// <param name="favoritesId"></param>
        /// <param name="numiid"></param>
        /// <returns></returns>
        public UatmFavoritItems GetLocalFavoritDetail(int favoritesId, long numiid)
        {
            return _favoritItemsDbDal.GetLocalFavoritDetail(favoritesId, numiid);
        }

        /// <summary>
        /// 生成淘口令
        /// </summary>
        /// <param name="url">口令跳转目标页</param>
        /// <param name="text">口令弹框内容</param>
        /// <param name="logo"></param>
        /// <returns></returns>
        public TbkTpwdCreateResponse TpwdCreate(string url, string text, string logo)
        {
            var taoBaoClient = new TaoBaoClient();
            var result = new TbkTpwdCreateResponse();
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(text))
            {
                LogHelper.Error("参数不能为空");
                return null;
            }
            try
            {
                var req = new TbkTpwdCreateRequest()
                {
                    Url = url,
                    Text = text,
                    Logo = logo
                };

                var responseresult = taoBaoClient.CreateTbkpwd(req);
                result = responseresult;
            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取商铺信息
        /// </summary>
        /// <param name="shopTitle"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public NTbkShop GetShopInfo(string shopTitle, long userId)
        {
            var taoBaoClient = new TaoBaoClient();
            var result = new NTbkShop();
            try
            {
                var req = new TbkShopGetRequest
                {
                    Fields = "user_id,shop_title,shop_type,seller_nick,pict_url,shop_url",
                    Q = shopTitle
                };

                var responseresult = taoBaoClient.GetShopInfo(req);

                result = responseresult.Results.FirstOrDefault(x => x.UserId == userId);
            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;
        }

        /// <summary>
        /// 通过关键字搜索返回列表
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public ResponseDataList<UatmTbkItem> GetSearchItemsList(string keywords)
        {
            var result = new ResponseDataList<UatmTbkItem>();
            var tempDataList = this.GetTaoBaoFavorites();
            var list = tempDataList.DataList;
            try
            {
                List<UatmTbkItem> listunion = new List<UatmTbkItem>();
                foreach (var ids in list)
                {
                    var templist = this.GetTaoBaoFavoritesItemsListAsync(Convert.ToInt32(ids.FavoritesId), 10, 1);
                    var searchlist = templist.DataList;
                    listunion = listunion.Union(searchlist).ToList();
                }
                var lisTbkItems = listunion.Where(x => x.Title.Contains(keywords)).ToList();
                result.DataList = lisTbkItems;
            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }

            return result;
        }

        public FavoritItemsPageList GetItemsPageList(string favoritesTitle, int pageIndex = 1, int pageSize = 10)
        {
            FavoritItemsDal favoritItemsDal=new FavoritItemsDal();
            var data = favoritItemsDal.GetfavoritesId(favoritesTitle);
            var result = favoritItemsDal.GetItemsPageList(data.FavoritesId, pageIndex, pageSize);
            return result;
        }


        /// <summary>
        /// 把淘宝联盟拉下来的数据添加到本地数据库
        /// </summary>
        /// <param name="info"></param>
        /// <param name="favoritesId"></param>
        public void AddItems(TbkUatmFavoritesItemGetResponse info, long favoritesId)
        {
            Thread.Sleep(10);
            List<UatmFavoritItems> result = new List<UatmFavoritItems>();

            foreach (var data in info.Results)
            {
                UatmFavoritItems ufItem = new UatmFavoritItems();
                var strSmallImages = string.Join(",", data.SmallImages.ToArray());
                ufItem.NumIid = data.NumIid;
                ufItem.Category = data.Category;
                ufItem.ClickUrl = data.ClickUrl;
                ufItem.CommissionRate = data.CommissionRate;
                ufItem.CouponClickUrl = data.CouponClickUrl;
                //ufItem.coupon_end_time = Convert.ToDateTime(data.CouponEndTime);
                ufItem.CouponInfo = data.CouponInfo;
                ufItem.CouponRemainCount = data.CouponRemainCount;
                //ufItem.coupon_start_time = Convert.ToDateTime(data.CouponStartTime);
                ufItem.CouponTotalCount = data.CouponTotalCount;
                //ufItem.event_end_time = data.EventEndTime;
                //ufItem.event_start_time = data.EventStartTime;
                ufItem.ItemUrl = data.ItemUrl;
                ufItem.Nick = data.Nick;
                ufItem.PictUrl = data.PictUrl;
                ufItem.Provcity = data.Provcity;
                ufItem.ReservePrice = Convert.ToDouble(data.ReservePrice);
                ufItem.SellerId = data.SellerId;
                ufItem.ShopTitle = data.ShopTitle;
                ufItem.SmallImages = strSmallImages;
                ufItem.Title = data.Title;
                ufItem.Status = data.Status;
                ufItem.ItemUrl = data.ItemUrl;
                ufItem.TkRate = Convert.ToDouble(data.TkRate);
                ufItem.Type = data.Type;
                ufItem.UserType = data.UserType;
                ufItem.Volume = data.Volume;
                ufItem.ZkFinalPrice = Convert.ToDouble(data.ZkFinalPrice);
                ufItem.ZkFinalPriceWap = Convert.ToDouble(data.ZkFinalPriceWap);
                ufItem.FavoritesTitle = _favoritItemsDbDal.GetTitle(favoritesId).FavoritesTitle;
                ufItem.FavoritesId = favoritesId;
                ufItem.CreateTime = DateTime.Now;
                //判断下数据库存不存在当前物品
                if (_favoritItemsDbDal.GetNumiid(data.NumIid) == 0)
                {
                    result.Add(ufItem);
                }
                else
                {
                    //存在就更新
                    _favoritItemsDbDal.UpdateItem(ufItem);
                }
            }
            _favoritItemsDbDal.AddTbItemsList(result);

        }

        /// <summary>
        /// 更新排序
        /// </summary>
        /// <param name="numiid"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool ChangeScore(long numiid,int score)
        {
            var data = _favoritItemsDbDal.GetLocalFavoritDetail(numiid);
            data.score = score;
            return _favoritItemsDbDal.UpdateItem(data);
        }


        public void AddTabItems(TbkUatmFavoritesGetResponse itemList)
        {
            Thread.Sleep(10);
            List<Favorites> retsult;
            retsult = new List<Favorites>();
            foreach (var data in itemList.Results)
            {
                Favorites temp = new Favorites();

                temp.FavoritesId = data.FavoritesId;
                temp.FavoritesTitle = data.FavoritesTitle;
                temp.Type = data.Type;

                if (_favoritItemsDbDal.GetFavorites(data.FavoritesId) == 0)
                {
                    retsult.Add(temp);
                }
                else
                {
                    _favoritItemsDbDal.UpdateFavorites(temp);
                }
            }
            _favoritItemsDbDal.AddFavorites(retsult);
        }




        /// <summary>
        /// 获取优惠券信息
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public TbkCouponGetResponse GetCouponInfo(long itemId)
        {
            var taoBaoClient = new TaoBaoClient();
            var result = new TbkCouponGetResponse();
            try
            {
                var req = new TbkCouponGetRequest
                {
                    ItemId = itemId
                };
                result = taoBaoClient.GetCouponInfo(req);
            }
            catch (Exception ex)
            {
                LogHelper.Exception(ex);
            }
            return result;
        }
     
    }
}
