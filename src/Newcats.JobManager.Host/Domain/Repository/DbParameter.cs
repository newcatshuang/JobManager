using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Newcats.JobManager.Host.Domain.Repository
{
    #region 参数
    /// <summary>
    /// 连接逻辑
    /// </summary>
    public enum LogicType
    {
        /// <summary>
        /// where语句之间用and连接
        /// </summary>
        And = 0,

        /// <summary>
        /// where语句之间用or连接
        /// </summary>
        Or = 1
    }

    /// <summary>
    /// 操作逻辑
    /// </summary>
    public enum OperateType
    {
        /// <summary>
        /// 相等
        /// </summary>
        Equal = 0,

        /// <summary>
        /// 不相等
        /// </summary>
        NotEqual = 1,

        /// <summary>
        /// 大于
        /// </summary>
        Greater = 2,

        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterEqual = 3,

        /// <summary>
        /// 小于
        /// </summary>
        Less = 4,

        /// <summary>
        /// 小于等于
        /// </summary>
        LessEqual = 5,

        /// <summary>
        /// like
        /// </summary>
        Like = 6,

        /// <summary>
        /// LeftLike
        /// </summary>
        LeftLike = 7,

        /// <summary>
        /// RightLike
        /// </summary>
        RightLike = 8,

        /// <summary>
        /// NotLike
        /// </summary>
        NotLike = 9,

        /// <summary>
        /// In
        /// </summary>
        In = 10,

        /// <summary>
        /// NotIn
        /// </summary>
        NotIn = 11,

        /// <summary>
        /// 不带参数的sql语句
        /// </summary>
        //SqlFormat = 12,

        /// <summary>
        /// 带参数的sql语句
        /// </summary>
        //SqlFormatPar = 13,

        /// <summary>
        /// Between
        /// </summary>
        Between = 14,

        /// <summary>
        /// End
        /// </summary>
        End = 15,

        /// <summary>
        /// 直接拼接sql语句.
        /// sql写在第二个参数value处.
        /// </summary>
        SqlText = 16
    }

    /// <summary>
    /// 排序类型
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// 正序
        /// </summary>
        ASC = 0,

        /// <summary>
        /// 倒序
        /// </summary>
        DESC = 1
    }

    /// <summary>
    /// 数据库sql where参数封装类
    /// 封装了sql逻辑，便于生成sql语句
    /// </summary>
    /// <typeparam name="TEntity">数据库实体类</typeparam>
    public class DbWhere<TEntity> where TEntity : class
    {
        /// <summary>
        /// 属性名（即数据库表的字段名）
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// 对应字段的值
        /// </summary>
        public object Value { get; private set; } = null;

        /// <summary>
        /// 操作逻辑
        /// </summary>
        public OperateType OperateType { get; private set; }

        /// <summary>
        /// 连接逻辑（and/or）
        /// </summary>
        public LogicType LogicType { get; private set; }

        /// <summary>
        /// 数据库sql where参数封装类
        /// </summary>
        /// <param name="expression">表达式，例u=>u.Id</param>
        /// <param name="value">表达式的值</param>
        /// <param name="operateType">操作逻辑</param>
        /// <param name="logicType">连接逻辑</param>
        public DbWhere(Expression<Func<TEntity, object>> expression, object value, OperateType operateType = OperateType.Equal, LogicType logicType = LogicType.And)
        {
            PropertyInfo property = GetProperty(expression) as PropertyInfo;
            var real = property.GetCustomAttribute(typeof(RealColumnAttribute), false);
            if (real != null && real is RealColumnAttribute)
            {
                PropertyName = ((RealColumnAttribute)real).ColumnName;
            }
            else
            {
                PropertyName = property.Name;
            }
            Value = value;
            OperateType = operateType;
            LogicType = logicType;
        }

        private MemberInfo GetProperty(LambdaExpression lambda)
        {
            Expression expr = lambda;
            for (; ; )
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        MemberExpression memberExpression = (MemberExpression)expr;
                        MemberInfo mi = memberExpression.Member;
                        return mi;
                    default:
                        return null;
                }
            }
        }
    }

    /// <summary>
    /// 数据库sql update参数的封装类
    /// 封装了sql逻辑，便于生成sql语句
    /// </summary>
    /// <typeparam name="TEntity">数据库实体类</typeparam>
    public class DbUpdate<TEntity> where TEntity : class
    {
        /// <summary>
        /// 属性名（即数据库表的字段名）
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// 对应字段的值
        /// </summary>
        public object Value { get; private set; } = null;

        /// <summary>
        /// 数据库sql update参数的封装类
        /// </summary>
        /// <param name="expression">表达式，例u=>u.Id</param>
        /// <param name="value">表达式的值</param>
        public DbUpdate(Expression<Func<TEntity, object>> expression, object value)
        {
            PropertyInfo property = GetProperty(expression) as PropertyInfo;
            PropertyName = property.Name;
            Value = value;
        }

        private MemberInfo GetProperty(LambdaExpression lambda)
        {
            Expression expr = lambda;
            for (; ; )
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        MemberExpression memberExpression = (MemberExpression)expr;
                        MemberInfo mi = memberExpression.Member;
                        return mi;
                    default:
                        return null;
                }
            }
        }
    }

    /// <summary>
    /// 数据库sql order by参数的封装类
    /// 封装了sql逻辑，便于生成sql语句
    /// </summary>
    /// <typeparam name="TEntity">数据库实体类</typeparam>
    public class DbOrderBy<TEntity> where TEntity : class
    {
        /// <summary>
        /// 属性名（即数据库表的字段名）
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// 排序类型
        /// </summary>
        public string OrderByType { get; private set; }

        /// <summary>
        /// 数据库sql order by参数的封装类
        /// </summary>
        /// <param name="expression">表达式，例u=>u.Id</param>
        /// <param name="orderByType">SortType.ASC/SortType.DESC</param>
        public DbOrderBy(Expression<Func<TEntity, object>> expression, SortType orderByType = SortType.ASC)
        {
            PropertyInfo property = GetProperty(expression) as PropertyInfo;
            var real = property.GetCustomAttribute(typeof(RealColumnAttribute), false);
            if (real != null && real is RealColumnAttribute)
            {
                PropertyName = ((RealColumnAttribute)real).ColumnName;
            }
            else
            {
                PropertyName = property.Name;
            }
            OrderByType = orderByType.ToString();
        }

        /// <summary>
        /// 数据库sql order by参数的封装类
        /// </summary>
        /// <param name="propertyName">属性名（即数据库表的字段名）</param>
        /// <param name="isAsc">是否升序</param>
        public DbOrderBy(string propertyName, bool isAsc = true)
        {
            PropertyName = propertyName;
            OrderByType = isAsc ? "ASC" : "DESC";
        }

        private MemberInfo GetProperty(LambdaExpression lambda)
        {
            Expression expr = lambda;
            for (; ; )
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        MemberExpression memberExpression = (MemberExpression)expr;
                        MemberInfo mi = memberExpression.Member;
                        return mi;
                    default:
                        return null;
                }
            }
        }
    }
    #endregion

    #region 特性
    /* 1.特性的优先级大于默认约定
     * 2.推荐使用特性
     * 3.如果一个实体类使用了特性，请全部属性都使用（如果有必要使用）
     * 4.程序里使用了缓存技术，不用太担心反射的性能问题
     * 
     * 默认约定：
     * 1.实体类名必须与数据库表名完全一致（或者数据库表名+Entity结尾）
     * 2.多表连接的实体类请使用数据库视图
     * 3.数据库主键默认为Id，或者Id结尾的字段
     * 4.自增主键请使用特性
     * 5.实体类中不应该包括数据库表中不存在的字段，否自请使用特性
     */
    /// <summary>
    /// 数据库表名(例User)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 表名(例User)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 对应的数据库表(例User)
        /// </summary>
        /// <param name="name">表名(例User)</param>
        public TableNameAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 如果当前实体对应的数据库表为表连接，则通过此特性标明真实的数据库字段（例User.Id）
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RealColumnAttribute : Attribute
    {
        /// <summary>
        /// 真实的数据库字段（例User.Id）
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 如果当前实体对应的数据库表为表连接，则通过此特性标明真实的数据库字段（例User.Id）
        /// </summary>
        /// <param name="columnName">真实的数据库字段（例User.Id）</param>
        public RealColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    /// <summary>
    /// 数据库表主键标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {

    }

    /// <summary>
    /// 数据库表主键自动增长标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoIncrementAttribute : Attribute
    {

    }

    /// <summary>
    /// 类的属性不属于数据库表
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotTableFieldAttribute : Attribute
    {

    }
    #endregion
}
