using Economicalleague.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.Infrastructure.ProjectException
{
    /// <summary>
    /// 自定义响应异常
    /// </summary>
    public class ResponseContextException : EconomicalleagueException
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="responseContext"></param>
        public ResponseContextException(ResponseContext responseContext)
        {
            ResponseContext = responseContext;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errCode"></param>
        /// <param name="msg"></param>
        public ResponseContextException(ErrCode errCode, string msg = "")
        {
            ResponseHead responseHead;
            if (string.IsNullOrEmpty(msg))
            {
                responseHead = new ResponseHead
                {
                    Ret = -1,
                    Code = errCode
                };
            }
            else
            {
                responseHead = new ResponseHead
                {
                    Ret = -1,
                    Code = errCode,
                    Msg = msg
                };
            }
            ResponseContext = new ResponseContext()
            {
                Head = responseHead
            };
        }

        public ResponseContext ResponseContext { get; private set; }
    }
}
