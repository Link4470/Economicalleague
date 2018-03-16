using Economicalleague.Common;
using Economicalleague.Domain.PlatformOrders;
using Economicalleague.Services.PlatformOrders;
using Economicalleague.Web.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Economicalleague.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        [AuthFilter(true)]
        public JsonResult GetOrderList(long tradeid = 0, long pid = 0, int pageIndex = 1, int pageSize = 10)
        {
            PlatformOrdersrv platformOrdersrv = new PlatformOrdersrv();
            var result = platformOrdersrv.GetOrderList(tradeid, pid, pageIndex, pageSize);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool Upload()
        {
            var files = HttpContext.Request.Files;
            if (files.Count <= 0)
            {
                return false;
            }
            PlatformOrdersrv platformOrdersrv = new PlatformOrdersrv();
            return platformOrdersrv.UploadExcelOrder(files[0]);
        }
    }
}