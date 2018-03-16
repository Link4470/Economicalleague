using System;
using System.Text;

namespace Economicalleague.Common
{
    public class Log4NetHelper
    {
        static readonly log4net.ILog logdebug = log4net.LogManager.GetLogger("logdebug");
        static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");
        static readonly log4net.ILog logmonitor = log4net.LogManager.GetLogger("logmonitor");
        static readonly log4net.ILog logperformance = log4net.LogManager.GetLogger("logperformance");
        static readonly log4net.ILog logsms = log4net.LogManager.GetLogger("logsms");
        static readonly log4net.ILog logpush = log4net.LogManager.GetLogger("logpush");

        public static void Debug(string msg)
        {
            logdebug.Debug(msg);
        }

        public static void Performance(string msg)
        {
            logperformance.Info(msg);
        }

        public static void Performance(string controllername, string actionname, string msg)
        {
            StringBuilder sbMsg = new StringBuilder();
            sbMsg.AppendFormat("ControllerName={0},ActionName={1},Msg={2}", controllername, actionname, msg);

            Performance(sbMsg.ToString());
        }

        public static void Info(string msg)
        {
            loginfo.Info(msg);
        }

        public static void Info(string controllername, string actionname, string msg)
        {
            StringBuilder sbMsg = new StringBuilder();
            sbMsg.AppendFormat("ControllerName={0},ActionName={1},Msg={2}", controllername, actionname, msg);

            loginfo.Info(sbMsg.ToString());
        }

        public static void Error(string errorMsg, Exception ex = null)
        {
            if (ex != null)
            {
                logerror.Error(errorMsg, ex);
            }
            else
            {
                logerror.Error(errorMsg);
            }
        }

        public static void Exception(string msg)
        {
            Exception(msg, null);
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

            Error(sbMsg.ToString(), ex);
        }
    }
}
