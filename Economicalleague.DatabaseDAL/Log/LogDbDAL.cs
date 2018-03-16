using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common.Models;
using Economicalleague.EntityFramework;

namespace Economicalleague.DatabaseDAL.Log
{
    public class LogDbDAL : BaseDbDal
    {
        public static void AddExceptionLog(Log_ExceptionLog exLog)
        {
            if (exLog != null)
            {
                EconomicalleagueContainer entity = new EconomicalleagueContainer();
                //var exceptionLogInfo = new Log_ExceptionLog()
                //{
                //    ControllerName = exLog.ControllerName,
                //    ActionName = exLog.ActionName,
                //    Message = exLog.Message,
                //    StackTrace = exLog.StackTrace,
                //    RemoteAddr = exLog.RemoteAddr,
                //    RequestID = exLog.RequestID,
                //    LoginUser = exLog.LoginUser,
                //    OccurTime = exLog.OccurTime,
                //    AppId = exLog.AppId,
                //};
                entity.Log_ExceptionLog.Add(exLog);
                entity.SaveChanges();
            }
        }
        public static void AddLoginLog(Log_LoginLog Log)
        {
            if (Log != null)
            {
                EconomicalleagueContainer entity = new EconomicalleagueContainer();
                entity.Log_LoginLog.Add(Log);
                entity.SaveChanges();
            }
        }
        public DateTime? CustomerLastLoginTime(out string lastLoginIp, long customerId)
        {
            lastLoginIp = "127.0.0.1";
            DateTime? lastLoginTime = DateTime.Now;
            EconomicalleagueContainer entity = new EconomicalleagueContainer();
            Log_LoginLog log = entity.Log_LoginLog.Where(x => x.LoginCustomer == customerId).OrderByDescending(t => t.OccurTime).FirstOrDefault();
            if (log != null)
            {
                lastLoginIp = log.RemoteAddr;
                lastLoginTime = log.OccurTime.Value;
            }
            return lastLoginTime;
        }
    }
}
