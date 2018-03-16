using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Common.Models
{
    /// <summary>
    /// 异常日志内容
    /// </summary>
    [Serializable]
    public class ExceptionLog
    {
        public long Id { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string RemoteAddr { get; set; }
        public string RequestID { get; set; }
        public string LoginUser { get; set; }
        public DateTime? OccurTime { get; set; }
    }
}
