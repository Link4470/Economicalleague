using Economicalleague.Domain;
using Economicalleague.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.DatabaseDAL.Manager
{
    public class UserManagerDbDal : BaseDbDal
    {
        public UserManager UserManagerLogin(LoginInfo loginInfo)
        {
            return EconomicalleagueEntity.UserManager.FirstOrDefault(x => x.UserName == loginInfo.UserName && x.PassWord == loginInfo.Password);
        }

    }
}
