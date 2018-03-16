using Economicalleague.Common.Cache;
using Economicalleague.DatabaseDAL.Manager;
using Economicalleague.Domain;
using Economicalleague.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Services.Customer
{
    public class UserManagerSrv : BaseService
    {
        public LoginResult Login(LoginInfo info)
        {
            UserManagerDbDal _userManagerDbDal = new UserManagerDbDal();
            LoginResult result = new LoginResult();
            var user = _userManagerDbDal.UserManagerLogin(info);
            result.IsOk = user != null;
            if (result.IsOk)
            {
                result.Token = Guid.NewGuid().ToString();
                CacheManager.Instance.Set(IdentityCacheKeys.UserToken.AppendSuffix(result.Token), user.Id.ToString());
            }
            return result;
        }
    }
}
