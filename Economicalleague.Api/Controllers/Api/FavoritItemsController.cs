using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Economicalleague.Api.Functions;
using Economicalleague.Common;
using Economicalleague.Common.Util;
using Economicalleague.EntityFramework;
using Economicalleague.Infrastructure.Response;
using Economicalleague.Services.FavoritItems;
using Top.Api.Domain;

namespace Economicalleague.Api.Controllers.Api
{
    public class FavoritItemsController : BaseApiController
    {

        [HttpGet]
        [Route("api/items/getitemlist")]
        [ApiAuthFilter(true)]
        public ResponseContext GetLocalItemList(long typeId, int pageSize = 10, int pageIndex = 1)
        {

            //if (pageSize <= 0)
            //{
            //    result.Head.Ret = -1;
            //    result.Head.Code = ErrCode.PageSizeIsNotAllowedLessThanZero;
            //    return result;
            //}
            //if (pageIndex <= 0)
            //{
            //    result.Head.Ret = -1;
            //    result.Head.Code = ErrCode.PageIndexIsNotAllowedLessThanZero;
            //    return result;
            //}
            //var itemsSrv = new FavoritItemsSrv();

            //result.Content = itemsSrv.GetItemsesList(typeId, pageSize, pageIndex);
            //return result;
            throw new NullReferenceException();
        }

        /// <summary>
        /// 获取选品库列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/goods/getfavorites")]
        [ApiAuthFilter(false)]
        public ResponseContext GetTaoBaoFavorites()
        {
            var itemsSrv = new FavoritItemsSrv();           
            result.Content =itemsSrv.GetTaoBaoFavorites() ;
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.GetItemsListFailed);
            }
            return result;

        }

        /// <summary>
        /// 获取选品库里的宝贝信息列表
        /// </summary>
        /// <param name="favoritesId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/goods/getfavoriteslist")]
        [ApiAuthFilter(false)]
        public ResponseContext GetTaoBaoFavoritesList(int favoritesId, int pageSize = 10, int pageIndex = 1)
        {
            var itemsSrv = new FavoritItemsSrv();

            //result.Content = itemsSrv.GetTaoBaoFavoritesItemsListAsync(favoritesId, pageSize, pageIndex);
            result.Content = itemsSrv.GetLocalFavoritesItemsList(favoritesId, pageSize, pageIndex);
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.GetItemsFailed);
            }
            return result;
        }

        [HttpGet]
        [Route("api/goods/gettest")]
        [ApiAuthFilter(false)]
        public ResponseContext Test(string favoritesTitle)
        {
            var itemsSrv = new FavoritItemsSrv();
            result.Content = itemsSrv.GetItemsPageList(favoritesTitle);
            return result;
        }

        /// <summary>
        /// 获取宝贝信息详情
        /// </summary>
        /// <param name="favoritesId"></param>
        /// <param name="numiid"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthFilter(false)]
        [Route("api/goods/getfavoritesdetail")]
        public ResponseContext GetTaoBaoUatmTbkItemDetail(int favoritesId, long numiid)
        {
            var itemsSrv = new FavoritItemsSrv();
            //result.Content = itemsSrv.GetFavoritItemsDetail(favoritesId, numiid);
            result.Content = itemsSrv.GetLocalFavoritDetail(favoritesId, numiid);
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.GetItemsFailed);
            }
            return result;
        }
        /// <summary>
        /// 获取商铺信息
        /// </summary>
        /// <param name="shopTitle">卖家id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/goods/getshopinfo")]
        [ApiAuthFilter(false)]
        public ResponseContext GetShopInfo(string shopTitle,long userId)
        {
            var itemsSrv = new FavoritItemsSrv();
            result.Content = itemsSrv.GetShopInfo(shopTitle,userId);
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.SearchShopFailed);
            }
            return result;
        }

        /// <summary>
        /// 生成淘口令
        /// </summary>
        /// <param name="url">淘口令链接跳转地址</param>
        /// <param name="text">口令弹框内容</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/goods/tbpwdcreate")]
        [ApiAuthFilter(true)]
        public ResponseContext TpwdCreate(string url, string text,string logo)
        {
            var itemsSrv = new FavoritItemsSrv();
            result.Content = itemsSrv.TpwdCreate(url, text,logo);
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.CreatePwdFailed);
            }
            return result;
        }

        /// <summary>
        /// 搜索宝贝信息
        /// </summary>
        /// <param name="searchwords"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/goods/searchresult")]
        [ApiAuthFilter(false)]
        public ResponseContext GetSearchResult(string searchwords)
        {
            var itemsSrv = new FavoritItemsSrv();
            result.Content = itemsSrv.GetSearchItemsList(searchwords);
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.SearchFailed);
            }
            return result;
        }
    }
}