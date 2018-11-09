using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newcats.JobManager.Api.Infrastructure.Text.Encrypt;

namespace Newcats.JobManager.Api.Infrastructure.DataAccess
{
    /// <summary>
    /// 1.仓储实现类,提供数据库访问能力,封装了基本的CRUD方法。
    /// 2.若要使用非默认的数据库连接，请重新给Connection属性赋值。
    /// 3.默认在Newcats.DenpendencyInjection里注册了作用域泛型仓储类
    /// _builder.RegisterGeneric(typeof(DataAccess.Dapper.Repository<,>)).As(typeof(DataAccess.Dapper.IRepository<,>)).InstancePerLifetimeScope();
    /// </summary>
    /// <typeparam name="TEntity">数据库实体类</typeparam>
    /// <typeparam name="TPrimaryKey">此数据库实体类的主键类型</typeparam>
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity
    {
        #region 数据库连接/配置
        /// <summary>
        /// 构造函数，初始化Connection/EntityType属性并赋值
        /// </summary>
        public Repository()
        {
            Connection = CreateDbConnection();
            EntityType = typeof(TEntity);
        }

        /// <summary>
        /// 1.数据库连接,在构造函数初始化(默认连接为"DefaultConnection")。
        /// 2.若要使用非默认的数据库连接，请重新赋值。
        /// 3.一般在Service类的构造函数赋值_repository.Connection=_repository.CreateDbConnection("OtherDB")。
        /// </summary>
        public IDbConnection Connection { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        private Type EntityType { get; set; }

        /// <summary>
        /// 1.根据应用程序执行目录下的appsettings.json配置文件(默认ConnectionStrings:DefaultConnection)的连接字符串创建数据库连接
        /// 2.会在构造函数自动调用并赋值，不需要手动调用，除非需要使用非默认的数据库连接
        /// </summary>
        /// <param name="key">连接字符串名，默认为"DefaultConnection"</param>
        /// <returns>数据库连接</returns>
        public IDbConnection CreateDbConnection(string key = "DefaultConnection")
        {
            if (!key.Equals("DefaultConnection", StringComparison.OrdinalIgnoreCase) && Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
            }
            //return new SqlConnection("Data Source = .; Initial Catalog =NewcatsDB; User ID = sa; Password = 123456;");
            return new SqlConnection(GetConnectionString(key));
        }

        /// <summary>
        /// 1.获取应用程序执行目录下的appsettings.json配置文件(默认ConnectionStrings:DefaultConnection)里的连接字符串
        /// 2.此处有缓存，如果更改了配置文件，请重新启动应用程序
        /// </summary>
        /// <param name="key">连接字符串名称</param>
        /// <returns>解密之后的连接字符串</returns>
        private string GetConnectionString(string key)
        {
            string dicKey = $"connStr_{key}";
            string connStr = "";
            if (_connStrDic.TryGetValue(dicKey, out connStr))
            {
                if (!string.IsNullOrWhiteSpace(connStr))
                    return connStr;
            }
            string connStrConfig = Helper.ConfigHelper.AppSettings.GetConnectionString(key);
            if (string.IsNullOrWhiteSpace(connStrConfig))
            {
                throw new KeyNotFoundException($"The config item ConnectionStrings:{key} do not exists on file appsettings.json");
            }
            connStr = Encrypt.DESDecrypt(connStrConfig);
            _connStrDic[dicKey] = connStr;
            return connStr;
        }
        #endregion

        #region 同步方法
        /// <summary>
        /// 插入一条数据，成功时返回当前主键的值，否则返回主键类型的默认值
        /// </summary>
        /// <param name="entity">要插入的数据实体</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功时返回当前主键的值，否则返回主键类型的默认值</returns>
        public TPrimaryKey Insert(TEntity entity, int? commandTimeout = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            string key = $"{EntityType.FullName}_Insert";
            string sqlText = "";
            if (_sqlInsertDic.TryGetValue(key, out sqlText))
            {
                if (!string.IsNullOrWhiteSpace(sqlText))
                {
                    return Connection.ExecuteScalar<TPrimaryKey>(sqlText, entity, null, commandTimeout, CommandType.Text);
                }
            }

            string fields = GetTableFieldsInsert(EntityType);
            string tableName = GetTableName(EntityType);
            string[] fieldArray = fields.Split(',');
            for (int i = 0; i < fieldArray.Length; i++)
            {
                fieldArray[i] = $"@{fieldArray[i]}";
            }
            sqlText = $" INSERT INTO {tableName} ({fields}) VALUES ({string.Join(",", fieldArray)});SELECT SCOPE_IDENTITY(); ";
            _sqlInsertDic[key] = sqlText;
            return Connection.ExecuteScalar<TPrimaryKey>(sqlText, entity, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 批量插入数据，返回成功的条数
        /// </summary>
        /// <param name="list">要插入的数据实体集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public int InsertBulk(IEnumerable<TEntity> list, int? commandTimeout = null)
        {
            if (list == null || !list.Any())
                throw new ArgumentNullException(nameof(list));

            string key = $"{EntityType.FullName}_InsertBulk";
            string sqlText = "";
            if (_sqlInsertDic.TryGetValue(key, out sqlText))
            {
                if (!string.IsNullOrWhiteSpace(sqlText))
                {
                    return Connection.Execute(sqlText, list, null, commandTimeout, CommandType.Text);
                }
            }

            string fields = GetTableFieldsInsert(EntityType);
            string tableName = GetTableName(EntityType);
            string[] fieldArray = fields.Split(',');
            for (int i = 0; i < fieldArray.Length; i++)
            {
                fieldArray[i] = $"@{fieldArray[i]}";
            }
            sqlText = $" INSERT INTO {tableName} ({fields}) VALUES ({string.Join(",", fieldArray)}); ";
            _sqlInsertDic[key] = sqlText;
            return Connection.Execute(sqlText, list, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据主键，删除一条记录
        /// </summary>
        /// <param name="primaryKeyValue">主键的值</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public int Delete(TPrimaryKey primaryKeyValue, int? commandTimeout = null)
        {
            if (primaryKeyValue == null)
                throw new ArgumentNullException(nameof(primaryKeyValue));

            string tableName = GetTableName(EntityType);
            string pkName = GetTablePrimaryKey(EntityType);
            string sqlText = $" DELETE FROM {tableName} WHERE {pkName}=@p_1 ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_1", primaryKeyValue);
            return Connection.Execute(sqlText, parameters, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据给定的条件，删除记录
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public int Delete(IEnumerable<DbWhere<TEntity>> dbWheres, int? commandTimeout = null)
        {
            string tableName = GetTableName(EntityType);
            string sqlWhere = "";
            DynamicParameters pars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sqlWhere = $" WHERE 1=1 {sqlWhere} ";
            string sqlText = $" DELETE FROM {tableName} {sqlWhere} ";
            return Connection.Execute(sqlText, pars, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据主键，更新一条记录
        /// </summary>
        /// <param name="primaryKeyValue">主键的值</param>
        /// <param name="dbUpdates">要更新的字段集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public int Update(TPrimaryKey primaryKeyValue, IEnumerable<DbUpdate<TEntity>> dbUpdates, int? commandTimeout = null)
        {
            if (primaryKeyValue == null)
                throw new ArgumentNullException(nameof(primaryKeyValue));
            if (dbUpdates == null || !dbUpdates.Any())
                throw new ArgumentNullException(nameof(dbUpdates));

            string tableName = GetTableName(EntityType);
            string pkName = GetTablePrimaryKey(EntityType);
            string sqlUpdate = "";
            DynamicParameters parameters = DbHelper.GetUpdateDynamicParameter(dbUpdates, ref sqlUpdate);
            parameters.Add("@" + pkName, primaryKeyValue);
            string sqlText = $" UPDATE {tableName} SET {sqlUpdate} WHERE {pkName}=@{pkName} ";
            return Connection.Execute(sqlText, parameters, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据给定的条件，更新记录
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="dbUpdates">要更新的字段集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public int Update(IEnumerable<DbWhere<TEntity>> dbWheres, IEnumerable<DbUpdate<TEntity>> dbUpdates, int? commandTimeout = null)
        {
            if (dbUpdates == null || !dbUpdates.Any())
                throw new ArgumentNullException(nameof(dbUpdates));
            string tableName = GetTableName(EntityType);
            string sqlWhere = "";
            DynamicParameters wherePars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sqlWhere = $" WHERE 1=1 {sqlWhere} ";
            string sqlUpdate = "";
            DynamicParameters updatePars = DbHelper.GetUpdateDynamicParameter(dbUpdates, ref sqlUpdate);
            wherePars.AddDynamicParams(updatePars);
            string sqlText = $" UPDATE {tableName} SET {sqlUpdate} {sqlWhere} ";
            return Connection.Execute(sqlText, wherePars, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据主键，获取一条记录
        /// </summary>
        /// <param name="primaryKeyValue">主键的值</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>数据库实体或null</returns>
        public TEntity Get(TPrimaryKey primaryKeyValue, int? commandTimeout = null)
        {
            if (primaryKeyValue == null)
                throw new ArgumentNullException(nameof(primaryKeyValue));

            string tableName = GetTableName(EntityType);
            string pkName = GetTablePrimaryKey(EntityType);
            string fields = GetTableFieldsQuery(EntityType);
            string sqlText = $" SELECT TOP 1 {fields} FROM {tableName} WHERE {pkName}=@p_1 ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_1", primaryKeyValue);
            return Connection.QueryFirstOrDefault<TEntity>(sqlText, parameters, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据给定的条件，获取一条记录
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="dbOrderBy">排序集合</param>
        /// <returns>数据库实体或null</returns>
        public TEntity Get(IEnumerable<DbWhere<TEntity>> dbWheres, int? commandTimeout = null, params DbOrderBy<TEntity>[] dbOrderBy)
        {
            string tableName = GetTableName(EntityType);
            string fields = GetTableFieldsQuery(EntityType);
            string sqlWhere = "";
            DynamicParameters parameters = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sqlWhere = $" WHERE 1=1 {sqlWhere} ";
            string sqlOrderBy = DbHelper.GetOrderBySql(dbOrderBy);
            if (!string.IsNullOrWhiteSpace(sqlOrderBy))
                sqlOrderBy = $" ORDER BY {sqlOrderBy} ";
            string sqlText = $" SELECT TOP 1 {fields} FROM {tableName} {sqlWhere} {sqlOrderBy} ";
            return Connection.QueryFirstOrDefault<TEntity>(sqlText, parameters, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据给定的条件及排序，分页获取数据
        /// </summary>
        /// <param name="pageIndex">页码索引（从0开始）（pageIndex小于等于0，返回第0页数据）</param>
        /// <param name="pageSize">页大小(pageSize小于等于0，返回所有数据)</param>
        /// <param name="totalCount">当前条件下的总记录数</param>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="dbOrderBy">排序</param>
        /// <returns>分页数据集合</returns>
        public IEnumerable<TEntity> GetPage(int pageIndex, int pageSize, ref int totalCount, IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<TEntity>[] dbOrderBy)
        {
            string tableName = GetTableName(EntityType);
            string fields = GetTableFieldsQuery(EntityType);
            string sqlText = "", sqlWhere = "", sqlOrderBy = "";
            DynamicParameters pars = new DynamicParameters();
            if (dbWheres != null && dbWheres.Any())
                pars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            sqlOrderBy = DbHelper.GetOrderBySql(dbOrderBy);
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sqlWhere = $" WHERE 1=1 {sqlWhere} ";
            if (!string.IsNullOrWhiteSpace(sqlOrderBy))
                sqlOrderBy = $" ORDER BY {sqlOrderBy} ";
            if (pars == null)
                pars = new DynamicParameters();
            pars.Add("@Row_Count", totalCount, DbType.Int32, ParameterDirection.Output);
            if (pageSize <= 0)
            {
                sqlText = $" SELECT {fields} FROM {tableName} {sqlWhere} {sqlOrderBy} ; SELECT @Row_Count=COUNT(1) FROM {tableName} {sqlWhere};";
            }
            else
            {
                if (pageIndex <= 0)
                {
                    sqlText = $" SELECT TOP {pageSize} {fields} FROM {tableName} {sqlWhere} {sqlOrderBy} ; SELECT @Row_Count=COUNT(1) FROM {tableName} {sqlWhere} ";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(sqlOrderBy))
                    {
                        sqlOrderBy = GetTablePrimaryKey(EntityType);
                        sqlOrderBy = $" ORDER BY {sqlOrderBy} ";
                    }
                    sqlText = $" SELECT * FROM(SELECT TOP {((pageIndex + 1) * pageSize)} ROW_NUMBER() OVER({sqlOrderBy}) RowNumber_Index,{fields} FROM {tableName} {sqlWhere}) temTab1 WHERE RowNumber_Index > {(pageIndex * pageSize)} ORDER BY RowNumber_Index ; SELECT @Row_Count=COUNT(1) FROM {tableName} {sqlWhere}; ";
                }
            }
            IEnumerable<TEntity> list = Connection.Query<TEntity>(sqlText, pars, null, true, commandTimeout, CommandType.Text);
            totalCount = pars.Get<int?>("@Row_Count") ?? 0;
            return list;
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns>数据集合</returns>
        public IEnumerable<TEntity> GetAll()
        {
            int totalCount = 0;
            return GetPage(0, 0, ref totalCount, null, null, null);
        }

        /// <summary>
        /// 根据给定的条件及排序，获取所有数据
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="dbOrderBy">排序</param>
        /// <returns>分页数据集合</returns>
        public IEnumerable<TEntity> GetAll(IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<TEntity>[] dbOrderBy)
        {
            int totalCount = 0;
            return GetPage(0, 0, ref totalCount, dbWheres, commandTimeout, dbOrderBy);
        }

        /// <summary>
        /// 根据默认排序，获取指定数量的数据
        /// </summary>
        /// <param name="top">指定数量</param>
        /// <returns>指定数量的数据集合</returns>
        public IEnumerable<TEntity> GetTop(int top)
        {
            int totalCount = 0;
            return GetPage(0, top, ref totalCount, null, null, null);
        }

        /// <summary>
        /// 根据给定的条件及排序，获取指定数量的数据
        /// </summary>
        /// <param name="top">指定数量</param>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="dbOrderBy">排序</param>
        /// <returns>分页数据集合</returns>
        public IEnumerable<TEntity> GetTop(int top, IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<TEntity>[] dbOrderBy)
        {
            int totalCount = 0;
            return GetPage(0, top, ref totalCount, dbWheres, commandTimeout, dbOrderBy);
        }

        /// <summary>
        /// 获取记录总数量
        /// </summary>
        /// <returns>记录总数量</returns>
        public int Count()
        {
            return Count(null, null);
        }

        /// <summary>
        /// 根据给定的条件，获取记录数量
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>记录数量</returns>
        public int Count(IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null)
        {
            string tableName = GetTableName(EntityType);
            if (dbWheres != null && dbWheres.Any())
            {
                string sqlWhere = "";
                DynamicParameters pars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
                string sqlText = $" SELECT COUNT(1) FROM {tableName} WHERE 1=1 {sqlWhere} ";
                return Connection.ExecuteScalar<int>(sqlText, pars, null, commandTimeout, CommandType.Text);
            }
            else
            {
                string sqlText = $" SELECT COUNT(1) FROM {tableName} ";
                return Connection.ExecuteScalar<int>(sqlText, null, null, commandTimeout, CommandType.Text);
            }
        }

        /// <summary>
        /// 根据主键，判断数据是否存在
        /// </summary>
        /// <param name="primaryKeyValue">主键值</param>
        /// <returns>是否存在</returns>
        public bool Exists(TPrimaryKey primaryKeyValue)
        {
            if (primaryKeyValue == null)
                throw new ArgumentNullException(nameof(primaryKeyValue));
            string tableName = GetTableName(EntityType);
            string pkName = GetTablePrimaryKey(EntityType);
            string sqlText = $" SELECT TOP 1 1 FROM {tableName} WHERE {pkName}=@p_1 ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_1", primaryKeyValue);
            object o = Connection.ExecuteScalar(sqlText, parameters, null, null, CommandType.Text);
            if (o != null && o != DBNull.Value && Convert.ToInt32(o) == 1)
                return true;
            return false;
        }

        /// <summary>
        /// 根据给定的条件，判断数据是否存在
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>是否存在</returns>
        public bool Exists(IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null)
        {
            string tableName = GetTableName(EntityType);
            string sqlWhere = "";
            DynamicParameters pars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            string sqlText = $" SELECT TOP 1 1 FROM {tableName} WHERE 1=1 {sqlWhere} ";
            object o = Connection.ExecuteScalar(sqlText, pars, null, commandTimeout, CommandType.Text);
            if (o != null && o != DBNull.Value && Convert.ToInt32(o) == 1)
                return true;
            return false;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteStoredProcedure(string storedProcedureName, DynamicParameters pars, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(storedProcedureName))
                throw new ArgumentNullException(nameof(storedProcedureName));
            return Connection.Execute(storedProcedureName, pars, null, commandTimeout, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行sql语句，返回受影响的行数
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>受影响的行数</returns>
        public int Execute(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return Connection.Execute(sqlText, pars, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行查询，并返回由查询返回的结果集中的第一行的第一列，其他行或列将被忽略
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>查询结果</returns>
        public T ExecuteScalar<T>(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return Connection.ExecuteScalar<T>(sqlText, pars, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行查询，并返回由查询返回的结果集中的第一行的第一列，其他行或列将被忽略
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>查询结果</returns>
        public object ExecuteScalar(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return Connection.ExecuteScalar(sqlText, pars, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行查询，返回结果集
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>查询结果集</returns>
        public IEnumerable<T> Query<T>(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return Connection.Query<T>(sqlText, pars, null, true, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行单行查询，返回结果
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>查询结果</returns>
        public T QueryFirstOrDefault<T>(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return Connection.QueryFirstOrDefault<T>(sqlText, pars, null, commandTimeout, commandType);
        }
        #endregion

        #region 异步方法
        /// <summary>
        /// 插入一条数据，成功时返回当前主键的值，否则返回主键类型的默认值
        /// </summary>
        /// <param name="entity">要插入的数据实体</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功时返回当前主键的值，否则返回主键类型的默认值</returns>
        public async Task<TPrimaryKey> InsertAsync(TEntity entity, int? commandTimeout = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            string key = $"{EntityType.FullName}_Insert";
            string sqlText = "";
            if (_sqlInsertDic.TryGetValue(key, out sqlText))
            {
                if (!string.IsNullOrWhiteSpace(sqlText))
                {
                    return await Connection.ExecuteScalarAsync<TPrimaryKey>(sqlText, entity, null, commandTimeout, CommandType.Text);
                }
            }

            string fields = GetTableFieldsInsert(EntityType);
            string tableName = GetTableName(EntityType);
            string[] fieldArray = fields.Split(',');
            for (int i = 0; i < fieldArray.Length; i++)
            {
                fieldArray[i] = $"@{fieldArray[i]}";
            }
            sqlText = $" INSERT INTO {tableName} ({fields}) VALUES ({string.Join(",", fieldArray)});SELECT SCOPE_IDENTITY(); ";
            _sqlInsertDic[key] = sqlText;
            return await Connection.ExecuteScalarAsync<TPrimaryKey>(sqlText, entity, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 批量插入数据，返回成功的条数
        /// </summary>
        /// <param name="list">要插入的数据实体集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public async Task<int> InsertBulkAsync(IEnumerable<TEntity> list, int? commandTimeout = null)
        {
            if (list == null || !list.Any())
                throw new ArgumentNullException(nameof(list));

            string key = $"{EntityType.FullName}_InsertBulk";
            string sqlText = "";
            if (_sqlInsertDic.TryGetValue(key, out sqlText))
            {
                if (!string.IsNullOrWhiteSpace(sqlText))
                {
                    return await Connection.ExecuteAsync(sqlText, list, null, commandTimeout, CommandType.Text);
                }
            }

            string fields = GetTableFieldsInsert(EntityType);
            string tableName = GetTableName(EntityType);
            string[] fieldArray = fields.Split(',');
            for (int i = 0; i < fieldArray.Length; i++)
            {
                fieldArray[i] = $"@{fieldArray[i]}";
            }
            sqlText = $" INSERT INTO {tableName} ({fields}) VALUES ({string.Join(",", fieldArray)}); ";
            _sqlInsertDic[key] = sqlText;
            return await Connection.ExecuteAsync(sqlText, list, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据主键，删除一条记录
        /// </summary>
        /// <param name="primaryKeyValue">主键的值</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public async Task<int> DeleteAsync(TPrimaryKey primaryKeyValue, int? commandTimeout = null)
        {
            if (primaryKeyValue == null)
                throw new ArgumentNullException(nameof(primaryKeyValue));

            string tableName = GetTableName(EntityType);
            string pkName = GetTablePrimaryKey(EntityType);
            string sqlText = $" DELETE FROM {tableName} WHERE {pkName}=@p_1 ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_1", primaryKeyValue);
            return await Connection.ExecuteAsync(sqlText, parameters, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据给定的条件，删除记录
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public async Task<int> DeleteAsync(IEnumerable<DbWhere<TEntity>> dbWheres, int? commandTimeout = null)
        {
            string tableName = GetTableName(EntityType);
            string sqlWhere = "";
            DynamicParameters pars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sqlWhere = $" WHERE 1=1 {sqlWhere} ";
            string sqlText = $" DELETE FROM {tableName} {sqlWhere} ";
            return await Connection.ExecuteAsync(sqlText, pars, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据主键，更新一条记录
        /// </summary>
        /// <param name="primaryKeyValue">主键的值</param>
        /// <param name="dbUpdates">要更新的字段集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public async Task<int> UpdateAsync(TPrimaryKey primaryKeyValue, IEnumerable<DbUpdate<TEntity>> dbUpdates, int? commandTimeout = null)
        {
            if (primaryKeyValue == null)
                throw new ArgumentNullException(nameof(primaryKeyValue));
            if (dbUpdates == null || !dbUpdates.Any())
                throw new ArgumentNullException(nameof(dbUpdates));

            string tableName = GetTableName(EntityType);
            string pkName = GetTablePrimaryKey(EntityType);
            string sqlUpdate = "";
            DynamicParameters parameters = DbHelper.GetUpdateDynamicParameter(dbUpdates, ref sqlUpdate);
            parameters.Add("@" + pkName, primaryKeyValue);
            string sqlText = $" UPDATE {tableName} SET {sqlUpdate} WHERE {pkName}=@{pkName} ";
            return await Connection.ExecuteAsync(sqlText, parameters, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据给定的条件，更新记录
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="dbUpdates">要更新的字段集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>成功的条数</returns>
        public async Task<int> UpdateAsync(IEnumerable<DbWhere<TEntity>> dbWheres, IEnumerable<DbUpdate<TEntity>> dbUpdates, int? commandTimeout = null)
        {
            if (dbUpdates == null || !dbUpdates.Any())
                throw new ArgumentNullException(nameof(dbUpdates));
            string tableName = GetTableName(EntityType);
            string sqlWhere = "";
            DynamicParameters wherePars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sqlWhere = $" WHERE 1=1 {sqlWhere} ";
            string sqlUpdate = "";
            DynamicParameters updatePars = DbHelper.GetUpdateDynamicParameter(dbUpdates, ref sqlUpdate);
            wherePars.AddDynamicParams(updatePars);
            string sqlText = $" UPDATE {tableName} SET {sqlUpdate} {sqlWhere} ";
            return await Connection.ExecuteAsync(sqlText, wherePars, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据主键，获取一条记录
        /// </summary>
        /// <param name="primaryKeyValue">主键的值</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>数据库实体或null</returns>
        public async Task<TEntity> GetAsync(TPrimaryKey primaryKeyValue, int? commandTimeout = null)
        {
            if (primaryKeyValue == null)
                throw new ArgumentNullException(nameof(primaryKeyValue));

            string tableName = GetTableName(EntityType);
            string pkName = GetTablePrimaryKey(EntityType);
            string fields = GetTableFieldsQuery(EntityType);
            string sqlText = $" SELECT TOP 1 {fields} FROM {tableName} WHERE {pkName}=@p_1 ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_1", primaryKeyValue);
            return await Connection.QueryFirstOrDefaultAsync<TEntity>(sqlText, parameters, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据给定的条件，获取一条记录
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="dbOrderBy">排序集合</param>
        /// <returns>数据库实体或null</returns>
        public async Task<TEntity> GetAsync(IEnumerable<DbWhere<TEntity>> dbWheres, int? commandTimeout = null, params DbOrderBy<TEntity>[] dbOrderBy)
        {
            string tableName = GetTableName(EntityType);
            string fields = GetTableFieldsQuery(EntityType);
            string sqlWhere = "";
            DynamicParameters parameters = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sqlWhere = $" WHERE 1=1 {sqlWhere} ";
            string sqlOrderBy = DbHelper.GetOrderBySql(dbOrderBy);
            if (!string.IsNullOrWhiteSpace(sqlOrderBy))
                sqlOrderBy = $" ORDER BY {sqlOrderBy} ";
            string sqlText = $" SELECT TOP 1 {fields} FROM {tableName} {sqlWhere} {sqlOrderBy} ";
            return await Connection.QueryFirstOrDefaultAsync<TEntity>(sqlText, parameters, null, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据给定的条件及排序，分页获取数据
        /// </summary>
        /// <param name="pageIndex">页码索引（从0开始）（pageIndex小于等于0，返回第0页数据）</param>
        /// <param name="pageSize">页大小(pageSize小于等于0，返回所有数据)</param>
        /// <param name="totalCount">当前条件下的总记录数</param>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="dbOrderBy">排序</param>
        /// <returns>分页数据集合</returns>
        public async Task<(IEnumerable<TEntity> list, int totalCount)> GetPageAsync(int pageIndex, int pageSize, IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<TEntity>[] dbOrderBy)
        {
            string tableName = GetTableName(EntityType);
            string fields = GetTableFieldsQuery(EntityType);
            string sqlText = "", sqlWhere = "", sqlOrderBy = "";
            DynamicParameters pars = new DynamicParameters();
            if (dbWheres != null && dbWheres.Any())
                pars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            sqlOrderBy = DbHelper.GetOrderBySql(dbOrderBy);
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sqlWhere = $" WHERE 1=1 {sqlWhere} ";
            if (!string.IsNullOrWhiteSpace(sqlOrderBy))
                sqlOrderBy = $" ORDER BY {sqlOrderBy} ";
            if (pars == null)
                pars = new DynamicParameters();
            int totalCount = 0;
            pars.Add("@Row_Count", totalCount, DbType.Int32, ParameterDirection.Output);
            if (pageSize <= 0)
            {
                sqlText = $" SELECT {fields} FROM {tableName} {sqlWhere} {sqlOrderBy} ; SELECT @Row_Count=COUNT(1) FROM {tableName} {sqlWhere};";
            }
            else
            {
                if (pageIndex <= 0)
                {
                    sqlText = $" SELECT TOP {pageSize} {fields} FROM {tableName} {sqlWhere} {sqlOrderBy} ; SELECT @Row_Count=COUNT(1) FROM {tableName} {sqlWhere} ";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(sqlOrderBy))
                    {
                        sqlOrderBy = GetTablePrimaryKey(EntityType);
                        sqlOrderBy = $" ORDER BY {sqlOrderBy} ";
                    }
                    sqlText = $" SELECT * FROM(SELECT TOP {((pageIndex + 1) * pageSize)} ROW_NUMBER() OVER({sqlOrderBy}) RowNumber_Index,{fields} FROM {tableName} {sqlWhere}) temTab1 WHERE RowNumber_Index > {(pageIndex * pageSize)} ORDER BY RowNumber_Index ; SELECT @Row_Count=COUNT(1) FROM {tableName} {sqlWhere}; ";
                }
            }
            IEnumerable<TEntity> list = await Connection.QueryAsync<TEntity>(sqlText, pars, null, commandTimeout, CommandType.Text);
            totalCount = pars.Get<int?>("@Row_Count") ?? 0;
            return (list, totalCount);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns>数据集合</returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var (list, totals) = await GetPageAsync(0, 0, null, null, null);
            return list;
        }

        /// <summary>
        /// 根据给定的条件及排序，获取所有数据
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="dbOrderBy">排序</param>
        /// <returns>数据集合</returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<TEntity>[] dbOrderBy)
        {
            var (list, totals) = await GetPageAsync(0, 0, dbWheres, commandTimeout, dbOrderBy);
            return list;
        }

        /// <summary>
        /// 根据默认排序，获取指定数量的数据
        /// </summary>
        /// <param name="top">指定数量</param>
        /// <returns>指定数量的数据集合</returns>
        public async Task<IEnumerable<TEntity>> GetTopAsync(int top)
        {
            var (list, totals) = await GetPageAsync(0, top, null, null, null);
            return list;
        }

        /// <summary>
        /// 根据给定的条件及排序，获取指定数量的数据
        /// </summary>
        /// <param name="top">指定数量</param>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="dbOrderBy">排序</param>
        /// <returns>指定数量的数据集合</returns>
        public async Task<IEnumerable<TEntity>> GetTopAsync(int top, IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<TEntity>[] dbOrderBy)
        {
            var (list, totals) = await GetPageAsync(0, top, dbWheres, commandTimeout, dbOrderBy);
            return list;
        }

        /// <summary>
        /// 获取记录总数量
        /// </summary>
        /// <returns>记录总数量</returns>
        public async Task<int> CountAsync()
        {
            return await CountAsync(null, null);
        }

        /// <summary>
        /// 根据给定的条件，获取记录数量
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>记录数量</returns>
        public async Task<int> CountAsync(IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null)
        {
            string tableName = GetTableName(EntityType);
            if (dbWheres != null && dbWheres.Any())
            {
                string sqlWhere = "";
                DynamicParameters pars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
                string sqlText = $" SELECT COUNT(1) FROM {tableName} WHERE 1=1 {sqlWhere} ";
                return await Connection.ExecuteScalarAsync<int>(sqlText, pars, null, commandTimeout, CommandType.Text);
            }
            else
            {
                string sqlText = $" SELECT COUNT(1) FROM {tableName} ";
                return await Connection.ExecuteScalarAsync<int>(sqlText, null, null, commandTimeout, CommandType.Text);
            }
        }

        /// <summary>
        /// 根据主键，判断数据是否存在
        /// </summary>
        /// <param name="primaryKeyValue">主键值</param>
        /// <returns>是否存在</returns>
        public async Task<bool> ExistsAsync(TPrimaryKey primaryKeyValue)
        {
            if (primaryKeyValue == null)
                throw new ArgumentNullException(nameof(primaryKeyValue));
            string tableName = GetTableName(EntityType);
            string pkName = GetTablePrimaryKey(EntityType);
            string sqlText = $" SELECT TOP 1 1 FROM {tableName} WHERE {pkName}=@p_1 ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_1", primaryKeyValue);
            object o = await Connection.ExecuteScalarAsync(sqlText, parameters, null, null, CommandType.Text);
            if (o != null && o != DBNull.Value && Convert.ToInt32(o) == 1)
                return true;
            return false;
        }

        /// <summary>
        /// 根据给定的条件，判断数据是否存在
        /// </summary>
        /// <param name="dbWheres">条件集合</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>是否存在</returns>
        public async Task<bool> ExistsAsync(IEnumerable<DbWhere<TEntity>> dbWheres = null, int? commandTimeout = null)
        {
            string tableName = GetTableName(EntityType);
            string sqlWhere = "";
            DynamicParameters pars = DbHelper.GetWhereDynamicParameter(dbWheres, ref sqlWhere);
            string sqlText = $" SELECT TOP 1 1 FROM {tableName} WHERE 1=1 {sqlWhere} ";
            object o = await Connection.ExecuteScalarAsync(sqlText, pars, null, commandTimeout, CommandType.Text);
            if (o != null && o != DBNull.Value && Convert.ToInt32(o) == 1)
                return true;
            return false;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcedureName">存储过程名称</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, DynamicParameters pars, int? commandTimeout = null)
        {
            if (string.IsNullOrWhiteSpace(storedProcedureName))
                throw new ArgumentNullException(nameof(storedProcedureName));
            return await Connection.ExecuteAsync(storedProcedureName, pars, null, commandTimeout, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行sql语句，返回受影响的行数
        /// </summary>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> ExecuteAsync(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return await Connection.ExecuteAsync(sqlText, pars, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行查询，并返回由查询返回的结果集中的第一行的第一列，其他行或列将被忽略
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>查询结果</returns>
        public async Task<T> ExecuteScalarAsync<T>(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return await Connection.ExecuteScalarAsync<T>(sqlText, pars, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行查询，并返回由查询返回的结果集中的第一行的第一列，其他行或列将被忽略
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>查询结果</returns>
        public async Task<object> ExecuteScalarAsync(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return await Connection.ExecuteScalarAsync(sqlText, pars, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行查询，返回结果集
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>查询结果集</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return await Connection.QueryAsync<T>(sqlText, pars, null, commandTimeout, commandType);
        }

        /// <summary>
        /// 执行单行查询，返回结果
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sqlText">sql语句</param>
        /// <param name="pars">参数</param>
        /// <param name="commandTimeout">超时时间(单位：秒)</param>
        /// <param name="commandType">执行类型，默认为CommandType.Text</param>
        /// <returns>查询结果</returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sqlText, DynamicParameters pars = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrWhiteSpace(sqlText))
                throw new ArgumentNullException(nameof(sqlText));
            return await Connection.QueryFirstOrDefaultAsync<T>(sqlText, pars, null, commandTimeout, commandType);
        }
        #endregion

        #region 静态字典缓存及私有方法
        /// <summary>
        /// 解密后的数据库连接字符串字典
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _connStrDic = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 实体类对应的insert语句字典，键为实体类全名（命名空间+类名+插入？批量插入？）
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _sqlInsertDic = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 实体类对应的数据库表名字典，键为实体类全名（命名空间+类名）
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _tableNameDic = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 实体类包含的所有有效字段的字典，键为实体类全名（命名空间+类名+插入?查询?）
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _tableFieldsDic = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 实体类对应的数据库表的主键名，键为实体类全名（命名空间+类名）
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _tablePrimaryKeyDic = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 获取实体类的表名
        /// 获取优先级为：
        /// 1.获取TableName特性的表名
        /// 2.若类名以Entity结尾，则获取类名（去除Entity名称）
        /// 3.前面都不满足，则直接获取类名
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>数据库表名</returns>
        private static string GetTableName(Type type)
        {
            string key = type.FullName;
            string tableName = "";
            if (_tableNameDic.TryGetValue(key, out tableName))
            {
                if (!string.IsNullOrWhiteSpace(tableName))
                    return tableName;
            }

            var attrs = type.GetCustomAttributes(typeof(TableNameAttribute), false);
            if (attrs != null && attrs.Length > 0)
            {
                foreach (var item in attrs)
                {
                    if (item is TableNameAttribute tabAttr)
                    {
                        tableName = tabAttr.Name;
                        break;
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(tableName))//1.判断TableNameAttribute特性
            {
                _tableNameDic[key] = tableName;
                return tableName;
            }
            tableName = type.Name;
            if (tableName.EndsWith("Entity", StringComparison.OrdinalIgnoreCase))//2.以Entity结尾的实体类型
            {
                tableName = tableName.Substring(0, tableName.Length - 6);
                _tableNameDic[key] = tableName;
                return tableName;
            }
            _tableNameDic[key] = tableName;
            return tableName;//3.直接返回类名
        }

        /// <summary>
        /// 获取实体表的所有字段（insert语句时使用）(实体类的属性应与表的字段完全一致)（表连接查询时的查询字段才会判断RealColumn特性）
        /// 1.排除NotTable特性的字段
        /// 2.排除AutoIncrement特性的字段（自增字段）
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>数据库表字段(逗号(,)分割)</returns>
        private static string GetTableFieldsInsert(Type type)
        {
            string key = $"{type.FullName}_InsertFields";
            string fields = "";
            if (_tableFieldsDic.TryGetValue(key, out fields))
            {
                if (!string.IsNullOrWhiteSpace(fields))
                    return fields;
            }
            var pros = type.GetProperties();
            if (pros == null || pros.Length == 0)
                throw new ArgumentException("No Fields found in this Entity", nameof(type));
            StringBuilder sb = new StringBuilder();
            foreach (var pro in pros)
            {
                //1.排除NotTableFieldAttribute特性的字段
                var attrsNot = pro.GetCustomAttributes(typeof(NotTableFieldAttribute), false);
                if (attrsNot != null && attrsNot.Length > 0)
                {
                    continue;
                }

                //2.插入时，排除自增类型字段
                var attrAuto = pro.GetCustomAttributes(typeof(AutoIncrementAttribute), false);
                if (attrAuto != null && attrAuto.Length > 0)
                {
                    continue;
                }
                sb.AppendFormat("{0},", pro.Name);
            }
            fields = sb.ToString().TrimEnd(',');
            _tableFieldsDic[key] = fields;
            return fields;
        }

        /// <summary>
        /// 获取实体表的所有字段（select语句时使用）(单表实体类的属性应与表的字段完全一致)（表连接查询时的查询字段会判断RealColumn特性）
        /// 1.排除NotTable特性的字段
        /// 2.表连接查询时请为每个属性使用RealColumn特性，否则会直接使用属性名
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>数据库表字段(逗号(,)分割)</returns>
        private static string GetTableFieldsQuery(Type type)
        {
            string key = $"{type.FullName}_QueryFields";
            string fields = "";
            if (_tableFieldsDic.TryGetValue(key, out fields))
            {
                if (!string.IsNullOrWhiteSpace(fields))
                    return fields;
            }
            var pros = type.GetProperties();
            if (pros == null || pros.Length == 0)
                throw new ArgumentException("No Fields found in this Entity", nameof(type));
            StringBuilder sb = new StringBuilder();
            foreach (var pro in pros)
            {
                //1.排除NotTableFieldAttribute特性的字段
                var attrsNot = pro.GetCustomAttributes(typeof(NotTableFieldAttribute), false);
                if (attrsNot != null && attrsNot.Length > 0)
                {
                    continue;
                }
                //3.判断表连接时候的实际字段
                var attrReal = pro.GetCustomAttributes(typeof(RealColumnAttribute), false);
                if (attrReal != null && attrReal.Length > 0)
                {
                    sb.AppendFormat("{0} as {1},", ((RealColumnAttribute)attrReal[0]).ColumnName, pro.Name);//表连接查询时候的字段
                }
                else
                {
                    sb.AppendFormat("{0},", pro.Name);
                }
            }
            fields = sb.ToString().TrimEnd(',');
            _tableFieldsDic[key] = fields;
            return fields;
        }

        /// <summary>
        /// 获取实体表的主键
        /// 获取优先级为：
        /// 1.获取PrimaryKey特性的字段
        /// 2.获取字段名为id的字段（忽略大小写）
        /// 3.获取字段为以id结尾的字段（忽略大小写）
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns>数据库表的主键</returns>
        private static string GetTablePrimaryKey(Type type)
        {
            string key = type.FullName;
            string pkName = "";
            if (_tablePrimaryKeyDic.TryGetValue(key, out pkName))
            {
                if (!string.IsNullOrWhiteSpace(pkName))
                    return pkName;
            }

            var pros = type.GetProperties();
            if (pros == null || pros.Length == 0)
                throw new ArgumentException("No Fields found in this Entity", nameof(type));
            //1.获取PrimaryKey特性的字段
            foreach (var p in pros)
            {
                var pkAttrs = p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                if (pkAttrs != null && pkAttrs.Length > 0)
                {
                    var realAttr = p.GetCustomAttributes(typeof(RealColumnAttribute), false);
                    if (realAttr != null && realAttr.Length > 0)
                        pkName = ((RealColumnAttribute)realAttr[0]).ColumnName;
                    else
                        pkName = p.Name;
                    _tablePrimaryKeyDic[key] = pkName;
                    return pkName;
                }
            }
            //2.获取字段名为id的字段（忽略大小写）
            foreach (var p in pros)
            {
                if (p.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    var realAttr = p.GetCustomAttributes(typeof(RealColumnAttribute), false);
                    if (realAttr != null && realAttr.Length > 0)
                        pkName = ((RealColumnAttribute)realAttr[0]).ColumnName;
                    else
                        pkName = p.Name;
                    _tablePrimaryKeyDic[key] = pkName;
                    return pkName;
                }
            }
            //3.获取字段为以id结尾的字段（忽略大小写）
            foreach (var p in pros)
            {
                if (p.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase))
                {
                    var realAttr = p.GetCustomAttributes(typeof(RealColumnAttribute), false);
                    if (realAttr != null && realAttr.Length > 0)
                        pkName = ((RealColumnAttribute)realAttr[0]).ColumnName;
                    else
                        pkName = p.Name;
                    _tablePrimaryKeyDic[key] = pkName;
                    return pkName;
                }
            }
            if (string.IsNullOrWhiteSpace(pkName))
                throw new ArgumentException("No PrimaryKey found in this Entity", nameof(type));
            return pkName;
        }
        #endregion
    }
}