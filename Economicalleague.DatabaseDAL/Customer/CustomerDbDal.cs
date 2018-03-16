using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common;
using Economicalleague.Domain.Customer;
using Economicalleague.EntityFramework;
using Economicalleague.Infrastructure.ProjectException;
using Economicalleague.Infrastructure.Response;

namespace Economicalleague.DatabaseDAL.Customer
{
    public class CustomerDbDal : BaseDbDal
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="customer">用户信息</param>
        public long AddCustomerInfo(CustomerInfo customer)
        {
            CustomerInfo addCustomer = JsonHelper.ObjectToObject<CustomerInfo>(customer);
            var insertInfo = EconomicalleagueEntity.GetInsertSql(addCustomer, nameof(CustomerInfo.CustomerId));
            StringBuilder sqlSb = new StringBuilder();
            sqlSb.Append(" IF EXISTS(SELECT 1 FROM dbo.CustomerInfo WHERE CustomerName='" + addCustomer.CustomerName + "' AND IsDeleted=0 ) ");
            sqlSb.Append("  SELECT CAST(0 AS BIGINT); ELSE BEGIN");
            sqlSb.Append(insertInfo.Item1);
            sqlSb.Append("SELECT CAST(@@IDENTITY AS BIGINT); END");
            var ds = EconomicalleagueEntity.MultipleResults(sqlSb.ToString(), insertInfo.Item2.ToArray(), System.Data.CommandType.Text)
                .AddResult<long>()
                .Execute();
            long[] customerIDs = ds.Count > 0 ? ds.First().Value as long[] : new long[0];
            if (customerIDs != null && customerIDs.Length > 0 && customerIDs[0] != 0)
            {
                return customerIDs[0];
            }
            else
            {
                throw new ResponseContextException(ErrCode.customerNameAlreadyExist);
            }
        }
        /// <summary>
        /// 根据账号获取用户信息
        /// </summary>
        /// <param name="customerName">账号</param>
        /// <returns></returns>
        public CustomerInfo GetCustomerInfo(string customerName)
        {
            return EconomicalleagueEntity.CustomerInfo.FirstOrDefault(x => x.CustomerName == customerName && !x.IsDeleted);
        }

        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public CustomerInfo GetCustomerInfo(long customerId)
        {
            return EconomicalleagueEntity.CustomerInfo.FirstOrDefault(x => x.CustomerId == customerId && !x.IsDeleted);
        }
        /// <summary>
        /// 检查账号是否存在
        /// </summary>
        /// <param name="customerName">账号</param>
        /// <returns></returns>
        public bool IsCustomerNameExists(string customerName)
        {
            return EconomicalleagueEntity.CustomerInfo.Any(x => x.CustomerName == customerName && !x.IsDeleted);
        }

        /// <summary>
        /// 根据用户id来判断是否存在账号
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public bool IsCustomerNameExists(long customerId)
        {
            return EconomicalleagueEntity.CustomerInfo.Any(x => x.CustomerId == customerId && !x.IsDeleted);
        }
        /// <summary>
        /// 检查账号是否存在
        /// </summary>
        /// <param name="customerName">账号</param>
        /// <param name="customerId">用户编号</param>
        /// <returns></returns>
        public bool IsCustomerNameExists(string customerName, out long customerId)
        {
            customerId = EconomicalleagueEntity.CustomerInfo
                .Where(x => x.CustomerName == customerName && !x.IsDeleted)
                .Select(x => x.CustomerId).FirstOrDefault();
            return customerId != 0;
        }

        public void AddUserInfo(UserInfo userInfo)
        {
            EconomicalleagueEntity.UserInfo.Add(userInfo);
            EconomicalleagueEntity.SaveChangesAsync();
        }
        public void UpdateUserInfo(UserInfo userInfo)
        {
            var user = EconomicalleagueEntity.UserInfo.FirstOrDefault(x => x.openid == userInfo.openid);
            user.adzoneid = userInfo.adzoneid;
            EconomicalleagueEntity.SaveChangesAsync();
        }
        public UserInfo WxGetUserInfo(string openId)
        {
            return EconomicalleagueEntity.UserInfo.FirstOrDefault(x => x.openid == openId);
        }
    }
}
