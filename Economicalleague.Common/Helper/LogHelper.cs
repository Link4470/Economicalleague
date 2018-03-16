using System;
using System.Text;
using Economicalleague.Common.Models;

namespace Economicalleague.Common
{
    public class LogHelper
    {
        public static void Debug(string msg)
        {
            Log4NetHelper.Debug(msg);
        }

        public static void Performance(string msg)
        {
            Log4NetHelper.Performance(msg);
        }

        public static void Performance(string controllername, string actionname, string msg)
        {
            StringBuilder sbMsg = new StringBuilder();
            sbMsg.AppendFormat("ControllerName={0},ActionName={1},Msg={2}", controllername, actionname, msg);

            Performance(sbMsg.ToString());
        }

        public static void Info(string msg)
        {
            Log4NetHelper.Info(msg);
        }

        public static void Info(string controllername, string actionname, string msg)
        {
            StringBuilder sbMsg = new StringBuilder();
            sbMsg.AppendFormat("ControllerName={0},ActionName={1},Msg={2}", controllername, actionname, msg);

            Info(sbMsg.ToString());
        }

        public static void Exception(Exception ex)
        {
            Exception(ex.Message, ex);
        }

        public static void Exception(string msg, Exception ex)
        {
            Error(msg, ex);
        }

        public static void Exception(string controllername, string actionname, Exception ex)
        {
            StringBuilder sbMsg = new StringBuilder();
            sbMsg.AppendFormat("ControllerName={0},ActionName={1},Msg={2}", controllername, actionname, ex.Message);

            Log4NetHelper.Error(sbMsg.ToString(), ex);
            Log4DbHelper.Exception(controllername, actionname, ex);
        }

        public static void Exception(ExceptionLog exceptionLog, Exception ex)
        {
            StringBuilder sbMsg = new StringBuilder();
            sbMsg.AppendFormat("ControllerName={0},ActionName={1},Msg={2}", exceptionLog?.ControllerName, exceptionLog?.ActionName, ex?.Message);

            Log4NetHelper.Error(sbMsg.ToString(), ex);
            Log4DbHelper.Exception(exceptionLog);
        }

        public static void Error(string errorMsg, Exception ex = null)
        {
            Log4NetHelper.Error(errorMsg, ex);
            Log4DbHelper.Error(errorMsg, ex);
        }
    }
}
