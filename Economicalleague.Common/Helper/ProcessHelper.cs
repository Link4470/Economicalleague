using System.Diagnostics;

namespace Economicalleague.Common
{
    public class ProcessHelper
    {
        public static string CurrentAppId
        {
            get
            {
                string currentAppId = ConfigurationHelper.GetString("AppId");

                if (string.IsNullOrEmpty(currentAppId))
                {
                    currentAppId = Process.GetCurrentProcess().ProcessName;
                }

                return currentAppId;
            }
        }
    }
}
