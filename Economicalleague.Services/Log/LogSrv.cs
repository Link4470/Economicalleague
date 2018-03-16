using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common.Models;
using Economicalleague.DatabaseDAL.Log;
using Economicalleague.EntityFramework;

namespace Economicalleague.Services.Log
{
    public class LogSrv : BaseService
    {
        public void AddExceptionLog(Log_ExceptionLog exLog)
        {
            Task.Run(() => LogDbDAL.AddExceptionLog(exLog));
        }
        public void AddLoginLog(Log_LoginLog Log)
        {
            Task.Run(() => LogDbDAL.AddLoginLog(Log));
        }
    }
}
