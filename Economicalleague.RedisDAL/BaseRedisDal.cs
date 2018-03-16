using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Economicalleague.Common;
using Economicalleague.EntityFramework;
using Economicalleague.Infrastructure.ProjectException;
using Economicalleague.Infrastructure.Response;

namespace Economicalleague.RedisDAL
{
    /// <summary>
    /// Redis数据访问基类
    /// </summary>
    public class BaseRedisDal
    {
        /// <summary>
        /// 构造参数
        /// </summary>
        public BaseRedisDal()
        {
            if (_zuoTopProjectEntity == null)
            {
                _zuoTopProjectEntity = new EconomicalleagueContainer();
            }
        }

        /// <summary>
        /// 构造参数
        /// </summary>
        public BaseRedisDal(EconomicalleagueContainer entity)
        {
            if (entity != null)
            {
                _zuoTopProjectEntity = entity;
            }
            else
            {
                _zuoTopProjectEntity = new EconomicalleagueContainer();
            }
        }


        private EconomicalleagueContainer _zuoTopProjectEntity;
        /// <summary>
        /// sql数据库操作实体
        /// </summary>
        public EconomicalleagueContainer ZuoTopProjectEntity
        {
            get { return _zuoTopProjectEntity; }
        }
        /// <summary>
        /// 重新初始化数据库操作实体
        /// </summary>
        public void ReInitZuoTopProjectEntity()
        {
            _zuoTopProjectEntity = new EconomicalleagueContainer();
        }

        private IRedisBase _redisEntity;
        /// <summary>
        /// Redis操作实体
        /// </summary>
        protected IRedisBase RedisEntity
        {
            get
            {
                if (_redisEntity == null)
                {
                    _redisEntity = CacheFactory.Redis;
                }
                return _redisEntity;
            }
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
