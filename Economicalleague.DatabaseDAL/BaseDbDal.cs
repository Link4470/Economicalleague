using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.EntityFramework;
using Economicalleague.Infrastructure.ProjectException;
using Economicalleague.Infrastructure.Response;

namespace Economicalleague.DatabaseDAL
{
    /// <summary>
    /// 数据访问基类
    /// </summary>
    public class BaseDbDal
    {
        /// <summary>
        /// 构造参数
        /// </summary>
        public BaseDbDal()
        {
            if (_economicalleagueEntity == null)
            {
                _economicalleagueEntity = new EconomicalleagueContainer();
            }
        }

        /// <summary>
        /// 构造参数
        /// </summary>
        public BaseDbDal(EconomicalleagueContainer entity)
        {
            if (entity != null)
            {
                _economicalleagueEntity = entity;
            }
            else
            {
                _economicalleagueEntity = new EconomicalleagueContainer();
            }
        }
        private EconomicalleagueContainer _economicalleagueEntity;
        /// <summary>
        /// sql数据库操作实体
        /// </summary>
        public EconomicalleagueContainer EconomicalleagueEntity
        {
            get { return _economicalleagueEntity; }
        }
        /// <summary>
        /// 重新初始化数据库操作实体
        /// </summary>
        public void ReInitBZoneEntity()
        {
            _economicalleagueEntity = new EconomicalleagueContainer();
        }

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
