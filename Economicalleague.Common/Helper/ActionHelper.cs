using System;
using System.Threading.Tasks;

namespace Economicalleague.Common
{
    public static class ActionHelper
    {
        public static void EatException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Log4NetHelper.Exception("发生在ActionHelper.EatException中的异常。", ex);
            }
        }

        public static void EatExceptionAsync(Action action)
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Log4NetHelper.Exception("发生在ActionHelper.EatExceptionAsync中的异常。", ex);
                }
            });
        }

        public static T EatException<T>(Func<T> action, T defaultValue = default(T))
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Log4NetHelper.Exception("发生在ActionHelper.EatException中的异常。", ex);
                return defaultValue;
            }
        }
    }
}
