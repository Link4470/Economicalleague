using Economicalleague.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace Economicalleague.Services.TaoBao
{
    public class TaoBaoClient
    {
        public static ITopClient Client;
        public static TbkUatmFavoritesItemGetRequest Req;
        public static string appkey = TaoBaoConfig.AppKey;
        public static string secret = TaoBaoConfig.AppSecret;
        public static string url = TaoBaoConfig.Url;
        public static string format = "json";
        public TaoBaoClient()
        {
            Client = new DefaultTopClient(url, appkey, secret, format);
        }
        /// <summary>
        /// 淘宝联盟分类信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public TbkUatmFavoritesGetResponse GetTaoBaoFavorites(TbkUatmFavoritesGetRequest req)
        {
            if (req == null)
            {
                return null;
            }
            return Client.Execute(req);
        }
        /// <summary>
        /// 淘宝联盟商品列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public TbkUatmFavoritesItemGetResponse GetFavoritesList(TbkUatmFavoritesItemGetRequest req)
        {
            if (req == null)
            {
                return null;
            }
            return Client.Execute(req);
        }
        /// <summary>
        /// 淘口令
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public TbkTpwdCreateResponse CreateTbkpwd(TbkTpwdCreateRequest req)
        {
            if (req == null)
            {
                return null;
            }
            return Client.Execute(req);
        }

        /// <summary>
        /// 获取商铺信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public TbkShopGetResponse GetShopInfo(TbkShopGetRequest req)
        {
            if (req == null)
            {
                return null;
            }
            return Client.Execute(req);
        }

        /// <summary>
        /// 淘宝客广告位创建
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public TbkAdzoneCreateResponse CreatePid(TbkAdzoneCreateRequest req)
        {
            if (req == null)
            {
                return null;
            }
            return Client.Execute(req);
        }

        public TbkCouponGetResponse GetCouponInfo(TbkCouponGetRequest req)
        {
            if (req == null)
            {
                return null;
            }
            return Client.Execute(req);
        }
    }
}
