using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common;
using Economicalleague.Common.Util;
using Economicalleague.Domain.FavoritItems;
using Economicalleague.EntityFramework;
using Economicalleague.Infrastructure.ProjectException;
using Economicalleague.Infrastructure.Response;

namespace Economicalleague.DatabaseDAL.FavoritItems
{
    public class FavoritItemsDal : BaseDbDal
    {
     

        /// <summary>
        /// 把淘宝客选品库信息保存到数据库
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool AddTbItemsList(List<UatmFavoritItems> info)
        {
            EconomicalleagueEntity.UatmFavoritItems.AddRange(info);
            EconomicalleagueEntity.SaveChanges();
            return true;
        }

        public bool UpdateItem(UatmFavoritItems item)
        {
            EconomicalleagueEntity.UatmFavoritItems.AddOrUpdate(item);
            EconomicalleagueEntity.SaveChanges();
            return true;
        }

        public bool UpdateFavorites(Favorites item)
        {
            EconomicalleagueEntity.Favorites.AddOrUpdate(item);
            EconomicalleagueEntity.SaveChanges();
            return true;
        }

        public int GetNumiid(long numiid)
        {
            var count = EconomicalleagueEntity.UatmFavoritItems
                .Count(x => x.NumIid == numiid);
            return count;
        }

        public string GetUrl(long numiid)
        {
            return EconomicalleagueEntity.UatmFavoritItems.Where(x => x.NumIid == numiid).Select(x => x.PictUrl)
                .FirstOrDefault();
        }

        public bool AddFavorites(List<Favorites> item)
        {
            EconomicalleagueEntity.Favorites.AddRange(item);
            EconomicalleagueEntity.SaveChanges();
            return true;
        }

        public int GetFavorites(long favoritesId)
        {
            var count = EconomicalleagueEntity.Favorites.Count(x => x.FavoritesId == favoritesId);
            return count;
        }

        public List<UatmFavoritItems> GetLocalFavoritItemsesList(long favoritesId)
        {
            var data =
                EconomicalleagueEntity.UatmFavoritItems
                .Where(x => x.FavoritesId == favoritesId && x.isSale == 1 && x.CouponInfo != null)
                .OrderByDescending(x => x.score).ToList();
            return data;
        }


        public UatmFavoritItems GetLocalFavoritDetail(long favoritesId, long numiid)
        {
            var data =
                EconomicalleagueEntity.UatmFavoritItems.FirstOrDefault(
                    x => x.FavoritesId == favoritesId && x.NumIid == numiid && x.isSale == 1);
            return data;
        }

        public UatmFavoritItems GetLocalFavoritDetail(long numiid)
        {
            return EconomicalleagueEntity.UatmFavoritItems.FirstOrDefault(x => x.NumIid == numiid);
        }

        /// <summary>
        /// 通过选品ID获取选品标题
        /// </summary>
        /// <param name="favoritesId"></param>
        /// <returns></returns>
        public Favorites GetTitle(long favoritesId)
        {
            var count = EconomicalleagueEntity.Favorites.FirstOrDefault(x => x.FavoritesId == favoritesId);
            return count;
        }

        /// <summary>
        /// 通过标题获取选品id
        /// </summary>
        /// <param name="favoritesTitle"></param>
        /// <returns></returns>
        public Favorites GetfavoritesId(string favoritesTitle)
        {
            var data = EconomicalleagueEntity.Favorites.FirstOrDefault(x => x.FavoritesTitle.Contains(favoritesTitle));
            return data;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="numiid"></param>
        public void Delete(long numiid)
        {
            var data = EconomicalleagueEntity.UatmFavoritItems.FirstOrDefault(x => x.NumIid == numiid);
            if (data != null) EconomicalleagueEntity.UatmFavoritItems.Remove(data);
        }

       

        public FavoritItemsPageList GetItemsPageList(long favoritesId, int pageIndex = 1, int pageSize = 10)
        {
            var listTemp = EconomicalleagueEntity.UatmFavoritItems.AsNoTracking().AsQueryable();
            if (favoritesId > 0)
            {
                listTemp = listTemp.Where(x => x.FavoritesId.ToString().Contains(favoritesId.ToString()) && x.CouponInfo != null);
            }
            var listOrderby = listTemp.OrderByDescending(x => x.CreateTime).Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToList();
            var result = listOrderby.Select(x => new Items
            {
                Numid = x.NumIid,
                Title = x.Title,
                ItemUrl = x.ItemUrl,
                CouponInfo = (x.ZkFinalPrice - Convert.ToDouble(x.CouponInfo)).ToString(),
                Commission = (x.ZkFinalPrice * (x.TkRate / 100)).ToString(),
                Volume = x.Volume,
                Score = x.score

            });
            return new FavoritItemsPageList
            {
                totalcount = result.Count(),
                list = result.ToList()
            };

        }
    }
}
