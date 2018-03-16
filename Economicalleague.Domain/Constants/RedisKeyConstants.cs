using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Domain.Constants
{
    public class RedisKeyConstants
    {
        #region 用户信息
        /// <summary>
        /// 登录用户Token列表
        /// </summary>
        public const string LoginCustomerTokenList = "ZuoTop:Login:LoginCustomerList:TokenList";
        /// <summary>
        /// 登录用户账号列表
        /// </summary>
        public const string LoginCustomerNameList = "ZuoTop:Login:LoginCustomerList:CustomerNameList";
        /// <summary>
        /// 被挤掉的登录用户token列表
        /// </summary>
        public const string CrowdedToken = "ZuoTop:Login:CrowdedToken";
        /// <summary>
        /// Redis用户信息HashsetId;
        /// </summary>
        public const string CustomerHashId = "ZuoTop:Customer:{0}";

        public const string WXLoginTokenList = "Economicalleague:Login:TokenList";
        #endregion
        #region 分页
        public const string PageingTime = "PageingTime:{0}:{1}";
        #endregion
    }
}
