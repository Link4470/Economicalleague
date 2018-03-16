using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.EntityFramework;

namespace Economicalleague.Domain.Customer
{

    public class LoginResponseInfo : CustomerInfo
    {
        public string session_key { get; set; }
        public string openid { get; set; }
        public string unionid { get; set; }
    }
    public class WxLoginResponseInfo
    {
        public string thrdsession { get; set; }
    }
    public class WxLoginInfo
    {
        public string session_key { get; set; }
        public string openid { get; set; }
        public string unionid { get; set; }
    }
    public class RegisterInfo
    {
        public string CustomerName { get; set; }
        public string PassWord { get; set; }
        public string Email { get; set; }
    }
    public class LoginInfo
    {
        public string code { get; set; }
        //public string CustomerName { get; set; }
        //public string PassWord { get; set; }

        public WxRequestUserInfo UserInfo { get; set; } 
    }
    /// <summary>
    /// 用户信息详情
    /// </summary>
    public class WxRequestUserInfo
    {
        public string nickName { get; set; }
        public string avatarUrl { get; set; }
        public string gender { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string language { get; set; }
    }
}
