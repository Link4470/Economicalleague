using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Infrastructure.ProjectException;
using Economicalleague.Infrastructure.Response;

namespace Economicalleague.Services
{
    public class BaseService
    {
        /// <summary>
        /// 抛出自定义异常
        /// </summary>
        /// <param name="errCode">错误代码</param>
        /// <param name="msg">自定义错误消息</param>
        protected void ThrowResponseContextException(ErrCode errCode, string msg = "")
        {
            throw new ResponseContextException(errCode, msg);
        }
    }
}
