//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Economicalleague.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class Log_ExceptionLog
    {
        public long Id { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string RemoteAddr { get; set; }
        public string RequestID { get; set; }
        public string LoginUser { get; set; }
        public Nullable<System.DateTime> OccurTime { get; set; }
        public string AppId { get; set; }
    }
}
