using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Domain
{
    public class LoginInfo
    {

        public string UserName { set; get; }

        public string Password { set; get; }
    }
    public class LoginResult
    {
        public bool IsOk { get; set; }
        public string Token { get; set; }
    }
}
