using Economicalleague.Domain;
using Economicalleague.Services.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Economicalleague.Web.Controllers
{
    public class LoginController: Controller
    {
        public static readonly String TokenKey = "LoginToken";
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginInfo info)
        {
            UserManagerSrv _userManagerSrv = new UserManagerSrv();
            var result = _userManagerSrv.Login(info);
            if (result.IsOk)
            {
                this.Response.Cookies.Add(new HttpCookie(TokenKey, result.Token));
                return RedirectToAction("Index", "Home");
            }

            TempData["errmsg"] = "用户名或密码错误，请重新输入";
            return View();
        }
    }
}