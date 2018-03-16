using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Economicalleague.Common;

namespace Economicalleague.Infrastructure.Response
{
    [Serializable]
    [DataContract]
    public class ResponseHead
    {
        /// <summary>
        /// 默认成功
        /// </summary>
        public ResponseHead()
        {
            this.Ret = 0;
            this.Code = ErrCode.Sucess;
            this.Msg = EnumHelper.GetEnumDescription(ErrCode.Sucess);
        }

        /// <summary>
        /// 不自动计算Msg
        /// </summary>
        [IgnoreDataMember]
        public bool NotAutoCalcMsg { get; set; }

        /// <summary>
        /// 返回值
        /// 0   :成功
        /// -1  :失败
        /// </summary>
        [DataMember]
        public int Ret { get; set; }

        /// <summary>
        /// 错误码
        /// 1000,1001
        /// </summary>
        [DataMember]
        public ErrCode Code { get; set; }


        [DataMember]
        public string Token { get; set; }

        private string msg = string.Empty;
        /// <summary>
        /// 错误信息
        /// </summary>
        [DataMember]
        public string Msg
        {
            get
            {
                if (NotAutoCalcMsg)
                    return this.msg;
                if (Code != ErrCode.Sucess && this.msg == EnumHelper.GetEnumDescription(ErrCode.Sucess))
                {
                    return EnumHelper.GetEnumDescription(Code);
                }
                else
                {
                    return this.msg;
                }
            }
            set
            {
                this.msg = value;
            }
        }


        public ResponseHead(int ret, int code, string msg)
        {
            Ret = ret;
            Code = (ErrCode)code;
            Msg = msg;
        }
    }
}