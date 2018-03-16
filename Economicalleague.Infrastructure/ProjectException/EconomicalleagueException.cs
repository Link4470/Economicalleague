using System;

namespace Economicalleague.Infrastructure.ProjectException
{
    /// <summary>
    /// 自定义异常
    /// </summary>
    public class EconomicalleagueException : Exception
    {
        public EconomicalleagueException()
        {
        }

        public EconomicalleagueException(string message) : base(message)
        {
        }
    }
}
