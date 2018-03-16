using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Economicalleague.EntityFramework
{
    /// <summary>
    /// EF生成Sql语句日志格式处理
    /// </summary>
    public class DbLogConfiguration : DbConfiguration
    {
        public DbLogConfiguration()
        {
            SetDatabaseLogFormatter(
                (context, writeAction) => new SingleLineLogFormatter(context, writeAction));
        }
    }

    public class SingleLineLogFormatter : DatabaseLogFormatter
    {
        public SingleLineLogFormatter(DbContext ctx, Action<string> action)
          : base(ctx, action)
        {
        }
        public override void LogCommand<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            //base.LogCommand<TResult>(command, interceptionContext);
        }
        public override void LogParameter<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext, DbParameter parameter)
        {
            // base.LogParameter<TResult>(command, interceptionContext, parameter);
        }

        public override void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            // base.Opened(connection, interceptionContext);
        }

        public override void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            // base.Closed(connection, interceptionContext);
        }

        public override void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            // base.Disposing(connection, interceptionContext);
        }

        public override void Committed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            // base.Committed(transaction, interceptionContext);
        }

        public override void BeganTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
        {
            // base.BeganTransaction(connection, interceptionContext);
        }

        public override void RolledBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            // base.RolledBack(connection, interceptionContext);
        }

        public override void EnlistedTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
        {
            //  base.EnlistedTransaction(connection, interceptionContext);
        }

        public override void LogResult<TResult>(System.Data.Common.DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (interceptionContext.Exception != null)
            {
                // Empty.
            }
            else if (interceptionContext.TaskStatus.HasFlag(TaskStatus.Canceled))
            {
                // Empty.
            }
            else
            {
                string singleLineLog = SingleLineLogHelper.Generate(command, Stopwatch.ElapsedMilliseconds);

                Write(singleLineLog);
            }
        }
    }

    public class SingleLineLogHelper
    {
        public static string Generate(DbCommand command, long elapsedMilliseconds)
        {
            string paraStr = string.Empty;
            if (command.Parameters.Count > 0)
            {
                paraStr = GetParameterStr(command.Parameters);
            }

            var singleLineLog = string.Format("{{Command:'{0}',Parameters:'{1}',ExecTime:{2}}}",
                                    Escape(command.CommandText).Replace("'", "\\'").Replace(Environment.NewLine, ""),
                                    Escape(paraStr).Replace("'", "\\'"),
                                    elapsedMilliseconds);

            return singleLineLog;
        }

        /// <summary>
        /// 获取参数字符串
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string GetParameterStr(DbParameterCollection parameters)
        {
            List<string> ParameterValuesList = new List<string>();

            foreach (DbParameter Parameter in parameters)
            {
                string ParameterName, ParameterValue;
                ParameterName = Parameter.ParameterName;

                if (Parameter.Direction == ParameterDirection.ReturnValue)
                    continue;

                if (Parameter.Value == null || Parameter.Value.Equals(DBNull.Value))
                    ParameterValue = "NULL";
                else
                {
                    switch (Parameter.DbType)
                    {
                        case DbType.AnsiString:
                        case DbType.String:
                        case DbType.Date:
                        case DbType.DateTime:
                        case DbType.Guid:
                        case DbType.Xml:
                            ParameterValue
                                = "'" + Parameter
                                        .Value
                                        .ToString()
                                        .Replace(Environment.NewLine, "") + "'";
                            break;

                        default:
                            ParameterValue = Parameter.Value.ToString();
                            break;
                    }

                    if (Parameter.Direction != ParameterDirection.Input)
                        ParameterValue += " " + Parameter.Direction.ToString();
                }

                ParameterValuesList.Add(string.Format("{0}={1}", ParameterName, ParameterValue));
            }

            return string.Join(", ", ParameterValuesList.ToArray());
        }

        /// <summary>
        /// 转义字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <returns></returns>
        private static string Escape(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            var result = new StringBuilder(str);
            Dictionary<string, string> toReplace = new Dictionary<string, string>() {
                { "\t","   "},{ "\v", Environment.NewLine},
                { "\f", string.Empty},{ "\b", string.Empty},
                { "\a", string.Empty},{ "\0", string.Empty},{ "\\", "\\\\" }
            };
            foreach (var item in toReplace)
            {
                result.Replace(item.Key, item.Value);
            }
            if (str.Contains("\r\n"))
            {
                result.Replace("\r\n", Environment.NewLine);
            }
            else if (str.Contains("\r"))
            {
                result.Replace("\r", Environment.NewLine);
            }
            else if (str.Contains("\n"))
            {
                result.Replace("\n", Environment.NewLine);
            }
            return result.ToString();
        }
    }
}
