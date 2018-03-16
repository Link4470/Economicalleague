using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Economicalleague.Api.Functions;
using Economicalleague.Infrastructure.Response;
using Economicalleague.Services.PlatformOrders;

namespace Economicalleague.Api.Controllers.Api
{
    public class OrdersController:BaseApiController
    {
        [HttpGet]
        [Route("api/goods/exceltodata")]
        [ApiAuthFilter(false)]
        public ResponseContext Exceltodata(string filepath)
        {
            throw new NullReferenceException();
        }

        [HttpGet]
        [Route("api/goods/createpid")]
        [ApiAuthFilter(false)]
        public ResponseContext CreatePid()
        {
            var ordersrv = new PlatformOrdersrv();
            result.Content = ordersrv.CreatePid();
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.CreatePidFailed);
            }
            return result;
        }

        [HttpGet]
        [ApiAuthFilter(true)]
        [Route("api/goods/getincominfo")]
        public ResponseContext GetIncomInfo()
        {
            var ordersrv = new PlatformOrdersrv();
            result.Content = ordersrv.GetIncomInfo(LoginUser.adzoneid);
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.GetIncomFailed);
            }
            return result;
        }

        [HttpGet]
        [Route("api/goods/getorderlist")]
        [ApiAuthFilter(true)]
        public ResponseContext GetOrdersInfo( int pageSize = 10, int pageIndex = 1)
        {
            var ordersrv = new PlatformOrdersrv();
            result.Content = ordersrv.GetBrebateOrderses(LoginUser.adzoneid, pageSize, pageIndex);
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.GetordersFailed);
            }
            return result;
        }

        [HttpGet]
        [Route("api/goods/getorderdetail")]
        [ApiAuthFilter(true)]
        public ResponseContext GetOrderDetail(long tradeId)
        {
            var ordersrv = new PlatformOrdersrv();
            result.Content = ordersrv.GetOrderDetail(tradeId);
            if (result.Content == null)
            {
                result.SetErrorCode(ErrCode.GetorderDetailFailed);
            }
            return result;
        }
    }
}