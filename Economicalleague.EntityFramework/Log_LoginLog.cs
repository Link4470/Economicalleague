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
    
    public partial class Log_LoginLog
    {
        public long Id { get; set; }
        public Nullable<long> LoginCustomer { get; set; }
        public string AppVersion { get; set; }
        public Nullable<int> AppType { get; set; }
        public string ApiVersion { get; set; }
        public Nullable<int> ApiType { get; set; }
        public string RemoteAddr { get; set; }
        public Nullable<System.DateTime> OccurTime { get; set; }
    }
}
