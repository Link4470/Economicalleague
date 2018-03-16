using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static Economicalleague.EntityFramework.MultipleResultSets;

namespace Economicalleague.EntityFramework
{
    /// <summary>
    /// EF扩展类
    /// </summary>
    public partial class EconomicalleagueContainer
    {
        /// <summary>
        /// 搜索内容
        /// </summary>
        /// <typeparam name="TSource">需要搜索的对象类型</typeparam>
        /// <param name="source">对象</param>
        /// <param name="selector">查询器</param>
        /// <param name="keyWord">关键字</param>
        /// 例子:entity.SearchByKeyword(entity.Zone_GroupInfo, (x => x.GroupName), "朋友😔")
        /// <returns></returns>
        public IQueryable<TSource> SearchByKeyword<TSource>(IQueryable<TSource> source, Expression<Func<TSource, string>> selector, string keyWord)
        {
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@like", "%" + keyWord + "%"));
            param.Add(new SqlParameter("@keyword", keyWord));
            return Database.SqlQuery<TSource>(string.Format("SELECT * FROM {0} WHERE {1} COLLATE SQL_Latin1_General_Cp437_BIN LIKE @like ORDER BY CHARINDEX(@keyword,{1}),LEN({1}),{1}", typeof(TSource).Name, ((MemberExpression)selector.Body).Member.Name), param).AsQueryable();
        }


        /// <summary>
        /// 获取数量
        /// </summary>
        /// <typeparam name="sql">sql语句</typeparam>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public int CountBySql(string sql, params object[] parameters)
        {
            return Database.SqlQuery<int>(sql, parameters).FirstOrDefault();
        }

        /// <summary>
        /// 判断指定字段值是否存在
        /// </summary>
        /// <typeparam name="TSource">需要搜索的对象类型</typeparam>
        /// <param name="source">对象</param>
        /// <param name="selector">查询器</param>
        /// <param name="keyWord">关键字</param>
        /// 例子:entity.SearchByKeyword(entity.Zone_GroupInfo, (x => x.GroupName), "朋友😔")
        /// <returns></returns>
        public IQueryable<TSource> GetRecordEquals<TSource>(IQueryable<TSource> source, Expression<Func<TSource, string>> selector, string keyWord)
        {
            return Database.SqlQuery<TSource>(string.Format("SELECT * FROM {0} WHERE {1} COLLATE SQL_Latin1_General_Cp437_BIN = @keyword",
                typeof(TSource).Name, ((MemberExpression)selector.Body).Member.Name), new SqlParameter("@keyword", keyWord)).AsQueryable();
        }

        /// <summary>
        /// 调用sql语句或存储过程返回多个结果集处理
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        public MultipleResultSetWrapper MultipleResults(string query, IEnumerable<SqlParameter> parameters = null, CommandType queryType = CommandType.StoredProcedure)
        {
            EconomicalleagueContainer db = new EconomicalleagueContainer();
            return new MultipleResultSetWrapper(db, query, queryType, parameters);
        }
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T">泛型集合的类型</typeparam>
        /// <param name="list">要插入大泛型集合</param>
        public void BulkInsert<T>(IList<T> list)
        {
            if (list.Count == 0)
                return;
            using (var db = new EconomicalleagueContainer())
            {
                if (db.Database.Connection.State != ConnectionState.Open)
                {
                    db.Database.Connection.Open(); //打开Connection连接  
                }
                using (var bulkCopy = new SqlBulkCopy((SqlConnection)db.Database.Connection))
                {
                    bulkCopy.BatchSize = list.Count;
                    bulkCopy.DestinationTableName = typeof(T).Name;

                    var table = new DataTable();
                    var props = TypeDescriptor.GetProperties(typeof(T))

                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                        .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];
                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }

                    bulkCopy.WriteToServer(table);
                }
                if (db.Database.Connection.State != ConnectionState.Closed)
                {
                    db.Database.Connection.Close(); //关闭Connection连接  
                }
            }
        }
        /// <summary>  
        /// 批量插入  
        /// </summary>  
        /// <typeparam name="T">泛型集合的类型</typeparam>  
        /// <param name="conn">连接对象</param>  
        /// <param name="tableName">将泛型集合插入到本地数据库表的表名</param>  
        /// <param name="list">要插入大泛型集合</param>  
        private void BulkInsert<T>(SqlConnection conn, string tableName, IList<T> list)
        {

        }

        /// <summary>
        /// 生成Insert语句信息
        /// </summary>
        /// <typeparam name="TEntity">实体对象类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="primaryKeyName">主键字段名称</param>
        /// <returns></returns>
        public Tuple<string, IList<SqlParameter>> GetInsertSql<TEntity>(TEntity entity, string primaryKeyName) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentException("数据不能为空");

            Type entityType = entity.GetType();
            var props = entityType.GetProperties().Where(i => i.PropertyType != typeof(EntityKey)
               && i.PropertyType != typeof(EntityState) && primaryKeyName != i.Name)
               .ToList();

            StringBuilder insertSqlSb = new StringBuilder();
            StringBuilder paraSb = new StringBuilder();
            IList<SqlParameter> paramList = new List<SqlParameter>();
            List<string> filedList = props.Select(x => x.Name).ToList();
            insertSqlSb.Append(" INSERT INTO " + string.Format("[{0}]", entityType.Name) + " (" + string.Join(",", filedList.ToArray()));
            insertSqlSb.Append(" ) Values(" + string.Join(",", filedList.Select(x => "@" + x).ToArray()) + ");");
            foreach (var member in props)
            {
                object value = member.GetValue(entity, null);
                paramList.Add(new SqlParameter("@" + member.Name, value != null ? value : DBNull.Value));
            }
            return new Tuple<string, IList<SqlParameter>>(insertSqlSb.ToString(), paramList);
        }

        /// <summary>
        /// 生成Insert语句信息
        /// </summary>
        /// <typeparam name="TEntity">实体对象类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <param name="primaryKeyName">主键字段名称</param>
        /// <returns></returns>
        public Tuple<string, IList<SqlParameter>> GetInsertSql<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentException("数据不能为空");

            Type entityType = entity.GetType();
            var props = entityType.GetProperties().Where(i => i.PropertyType != typeof(EntityKey)
               && i.PropertyType != typeof(EntityState))
               .ToList();

            StringBuilder insertSqlSb = new StringBuilder();
            StringBuilder paraSb = new StringBuilder();
            IList<SqlParameter> paramList = new List<SqlParameter>();
            List<string> filedList = props.Select(x => x.Name).ToList();
            insertSqlSb.Append(" INSERT INTO " + string.Format("[{0}]", entityType.Name) + " (" + string.Join(",", filedList.ToArray()));
            insertSqlSb.Append(" ) Values(" + string.Join(",", filedList.Select(x => "@" + x).ToArray()) + ");");
            foreach (var member in props)
            {
                object value = member.GetValue(entity, null);
                paramList.Add(new SqlParameter("@" + member.Name, value != null ? value : DBNull.Value));
            }
            return new Tuple<string, IList<SqlParameter>>(insertSqlSb.ToString(), paramList);
        }

        //public object MultipleResults(string v, KeyValuePair<string, object>[] keyValuePair)
        //{
        //    throw new NotImplementedException();
        //}
    }

    /// <summary>
    /// DbContext扩展类
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// 获取主键字段名称
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetKeyNames<TEntity>(this DbContext context)
            where TEntity : class
        {
            return context.GetKeyName(typeof(TEntity));
        }

        /// <summary>
        /// 获取主键字段名称
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static string GetKeyName(this DbContext context, Type entityType)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;
            var typeItems = metadata.GetItems<EntityType>(DataSpace.OSpace);
            if (typeItems.Count == 0)
                return string.Empty;
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));
            var entityMetadata = typeItems.FirstOrDefault(e => objectItemCollection.GetClrType(e) == entityType);
            if (entityMetadata == null)
            {
                return string.Empty;
            }
            return entityMetadata.KeyProperties.Select(p => p.Name).FirstOrDefault();
        }
    }

    /// <summary>
    /// 日志批量插入方法
    /// </summary>
    public partial class EconomicalleagueLogEntities
    {
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T">泛型集合的类型</typeparam>
        /// <param name="list">要插入大泛型集合</param>
        public void BulkInsert<T>(IList<T> list)
        {
            if (list.Count == 0)
                return;
            using (var db = new EconomicalleagueContainer())
            {
                if (db.Database.Connection.State != ConnectionState.Open)
                {
                    db.Database.Connection.Open(); //打开Connection连接  
                }
                using (var bulkCopy = new SqlBulkCopy((SqlConnection)db.Database.Connection))
                {
                    bulkCopy.BatchSize = list.Count;
                    bulkCopy.DestinationTableName = typeof(T).Name;

                    var table = new DataTable();
                    var props = TypeDescriptor.GetProperties(typeof(T))

                        .Cast<PropertyDescriptor>()
                        .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                        .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];
                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }

                    bulkCopy.WriteToServer(table);
                }
                if (db.Database.Connection.State != ConnectionState.Closed)
                {
                    db.Database.Connection.Close(); //关闭Connection连接  
                }
            }
        }
    }

    /// <summary>
    /// 调用存储过程返回多个结果集处理
    /// </summary>
    public static class MultipleResultSets
    {
        #region Public Methods
        // public static MultipleResultSetWrapper MultipleResults(string query, IEnumerable<SqlParameter> parameters = null, CommandType queryType = CommandType.StoredProcedure) => new MultipleResultSetWrapper(query: query, parameters: parameters, queryType: queryType);
        #endregion Public Methods

        #region Public Classes
        public class MultipleResultSetWrapper
        {
            #region Private Fields
            private readonly IObjectContextAdapter _Adapter;
            private readonly string _CommandText;
            private readonly CommandType _CommandType;
            private readonly DbContext _db;
            private readonly IEnumerable<SqlParameter> _parameters;
            #endregion Private Fields

            #region Public Constructors
            public MultipleResultSetWrapper(DbContext db, string query, CommandType queryType, IEnumerable<SqlParameter> parameters = null)
            {
                _db = db;
                _Adapter = db;
                _CommandText = query;
                _CommandType = queryType;
                _parameters = parameters;
                _resultSets = new Dictionary<string, Func<DbDataReader, IEnumerable>>();
            }
            #endregion Public Constructors

            #region Public Fields
            public Dictionary<string, Func<DbDataReader, IEnumerable>> _resultSets;
            #endregion Public Fields

            #region Public Methods
            public MultipleResultSetWrapper AddResult<TResult>(string key = "")
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = typeof(TResult).Name;
                }

                _resultSets.Add(key, OneResult<TResult>);
                return this;
            }
            public Dictionary<string, object> Execute()
            {
                var results = new Dictionary<string, object>();
                using (var connection = _db.Database.Connection)
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    var command = connection.CreateCommand();
                    command.CommandText = _CommandText;
                    command.CommandType = _CommandType;

                    if (_parameters?.Any() ?? false)
                    {
                        command.Parameters.AddRange(_parameters.ToArray());
                    }

                    Stopwatch stopwatch = Stopwatch.StartNew();

                    using (var reader = command.ExecuteReader())
                    {
                        foreach (var resultSet in _resultSets)
                        {
                            results.Add(resultSet.Key, resultSet.Value(reader));
                        }
                    }

                    stopwatch.Stop();
                    string singleLineLog = SingleLineLogHelper.Generate(command, stopwatch.ElapsedMilliseconds);
                    _db.Database.Log?.Invoke(singleLineLog);
                    return results;
                }
            }
            #endregion Public Methods

            #region Private Methods
            private IEnumerable OneResult<TResult>(DbDataReader reader)
            {
                var result = _Adapter
                    .ObjectContext
                    .Translate<TResult>(reader)
                    .ToArray();
                reader.NextResult();
                return result;
            }
            #endregion Private Methods
        }
        #endregion Public Classes

        #region Extensions


        #endregion
    }
}
