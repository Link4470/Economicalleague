using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common;
using Economicalleague.Domain.Constants;
using Economicalleague.Domain.Customer;
using Economicalleague.EntityFramework;

namespace Economicalleague.RedisDAL.Customer
{
    public class CustomerRedisDal : BaseRedisDal
    {
        public CustomerDetail GetCustomerDetail(long customerId)
        {
            return new CustomerDetail();
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="customer">用户信息</param>
        public void AddCustomerInfo(CustomerDetail customer)
        {
            //添加到Redis中
            string customerId = customer.CustomerId.ToString();
            string userHashId = string.Format(RedisKeyConstants.CustomerHashId, customerId);
            RedisEntity.HashSetRange<string>(userHashId, JsonHelper.ObjectToDictionary(customer));
        }

        //public void AddWeixinUserInfo(UserInfo userInfo)
        //{
        //    string userId = userInfo.userid.ToString();
        //    string userHashId = string.Format(RedisKeyConstants.CustomerHashId, userId);
        //    RedisEntity.HashSetRange<string>(userHashId, JsonHelper.ObjectToDictionary(userInfo));

        //}
    }
}
