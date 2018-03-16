using System;
using Economicalleague.Common.Models;
using Economicalleague.EntityFramework;

namespace Economicalleague.Common
{
    public class Log4DbHelper
    {
        private static bool isSaveExceptionToDb = ConfigurationHelper.GetBool("IsSaveExceptionToDb", true);

        public static void Error(string errorMsg, Exception ex)
        {
            if (!isSaveExceptionToDb)
            {
                return;
            }

            var exLog = new ExceptionLog
            {
                Message = errorMsg,
                StackTrace = ex?.StackTrace,
                OccurTime = DateTime.Now,
            };

            AddExceptionLog(exLog);
        }

        public static void Exception(string controllername, string actionname, Exception ex)
        {
            if (!isSaveExceptionToDb)
            {
                return;
            }

            var exLog = new ExceptionLog
            {
                ControllerName = controllername,
                ActionName = actionname,
                Message = ex?.Message,
                StackTrace = ex?.StackTrace,
                OccurTime = DateTime.Now,
            };

            AddExceptionLog(exLog);
        }

        public static void Exception(ExceptionLog exLog)
        {
            if (!isSaveExceptionToDb)
            {
                return;
            }

            AddExceptionLog(exLog);
        }

        private static void AddExceptionLog(ExceptionLog exLog)
        {
            var exLogRecord = new Log_ExceptionLog
            {
                ControllerName = exLog.ControllerName,
                ActionName = exLog.ActionName,
                Message = exLog.Message,
                StackTrace = exLog.StackTrace,
                RemoteAddr = exLog.RemoteAddr,
                RequestID = exLog.RequestID,
                LoginUser = exLog.LoginUser,
                OccurTime = exLog.OccurTime,
                AppId = ProcessHelper.CurrentAppId,
            };

            ActionHelper.EatException(() =>
            {
                AddExceptionLog(exLogRecord);
            });
        }
        private static void AddExceptionLog(Log_ExceptionLog exLog)
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
    }
}