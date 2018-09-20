using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace Newcats.JobManager.Host.Domain.Repository
{
    /// <summary>
    /// DbParameter帮助类
    /// </summary>
    internal class DbHelper
    {
        /// <summary>
        /// 把DbWhere参数数组转换成DynamicParameters参数，并且输出sql where语句（不包含【WHERE】关键字）
        /// </summary>
        /// <typeparam name="TEntity">数据库实体类</typeparam>
        /// <param name="dbWheres">DbWhere参数数组</param>
        /// <param name="sqlWhere">不包含【WHERE】和【WHERE 1=1】的sql where语句(例  AND Id=@pw_1 AND Age=@pw_2 )</param>
        /// <returns>DynamicParameters参数</returns>
        public static DynamicParameters GetWhereDynamicParameter<TEntity>(IEnumerable<DbWhere<TEntity>> dbWheres, ref string sqlWhere) where TEntity : class
        {
            if (dbWheres == null || !dbWheres.Any())
            {
                sqlWhere = "";
                return null;
            }
            DynamicParameters dp = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (DbWhere<TEntity> item in dbWheres)
            {
                if (item == null)
                    continue;
                switch (item.OperateType)
                {
                    case OperateType.Equal:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NULL ", item.LogicType.ToString(), item.PropertyName);
                        else
                        {
                            sb.AppendFormat(" {0} {1} = {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                            dp.Add("@pw_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.NotEqual:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NOT NULL ", item.LogicType.ToString(), item.PropertyName);
                        else
                        {
                            sb.AppendFormat(" {0} {1} != {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                            dp.Add("@pw_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.Greater:
                        sb.AppendFormat(" {0} {1} > {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), item.Value);
                        break;
                    case OperateType.GreaterEqual:
                        sb.AppendFormat(" {0} {1} >= {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), item.Value);
                        break;
                    case OperateType.Less:
                        sb.AppendFormat(" {0} {1} < {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), item.Value);
                        break;
                    case OperateType.LessEqual:
                        sb.AppendFormat(" {0} {1} <= {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), item.Value);
                        break;
                    case OperateType.Like:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), "%" + item.Value + "%");
                        break;
                    case OperateType.LeftLike:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), item.Value + "%");
                        break;
                    case OperateType.RightLike:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), "%" + item.Value);
                        break;
                    case OperateType.NotLike:
                        sb.AppendFormat(" {0} {1} NOT LIKE {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), "%" + item.Value + "%");
                        break;
                    case OperateType.In:
                        Array arr = item.Value as Array;
                        if (arr != null)
                            sb.AppendFormat(" {0} {1} IN ({2}) ", item.LogicType.ToString(), item.PropertyName, ArrayToString(arr));
                        else
                            sb.AppendFormat(" {0} 1<>1 ", item.LogicType.ToString());
                        break;
                    case OperateType.NotIn:
                        Array arr1 = item.Value as Array;
                        if (arr1 != null)
                            sb.AppendFormat(" {0} {1} NOT IN ({2}) ", item.LogicType.ToString(), item.PropertyName, ArrayToString(arr1));
                        break;
                    //case OperateType.SqlFormat:
                    //    object[] arr2 = item.Value as object[];
                    //    if (arr2 != null)
                    //        sb.AppendFormat(item.PropertyName, ArrayToString(arr2));
                    //    else
                    //        sb.AppendFormat(item.PropertyName, item.Value);
                    //    break;
                    //case OperateType.SqlFormatPar:
                    //    object[] arr3 = item.Value as object[];
                    //    if (arr3 != null)
                    //    {
                    //        string[] ps = new string[arr3.Length];
                    //        for (int i = 0; i < arr3.Length; i++)
                    //        {
                    //            ps[i] = "@pw_" + index.ToString();
                    //            dp.Add("@pw_" + index.ToString(), arr3[i]);
                    //            index++;
                    //        }
                    //        sb.AppendFormat(item.PropertyName, ps);
                    //    }
                    //    else
                    //    {
                    //        sb.AppendFormat(item.PropertyName, "@pw_" + index.ToString());
                    //        dp.Add("@pw_" + index.ToString(), item.Value);
                    //    }
                    //    break;
                    case OperateType.Between:
                        sb.AppendFormat(" {0} {1} BETWEEN {2} ", item.LogicType.ToString(), item.PropertyName, "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), item.Value);
                        break;
                    case OperateType.End:
                        sb.AppendFormat(" {0} {1} ", item.LogicType.ToString(), "@pw_" + index.ToString());
                        dp.Add("@pw_" + index.ToString(), item.Value);
                        break;
                    case OperateType.SqlText:
                        sb.AppendFormat(" {0} {1} ", item.LogicType.ToString(), item.Value);
                        break;
                    default:
                        break;
                }
                index++;
            }
            sqlWhere = sb.ToString();
            return dp;
        }

        /// <summary>
        /// 把DbUpdate参数数组转换成DynamicParameters参数，并且输出sql update语句（不包含【SET】关键字）
        /// </summary>
        /// <typeparam name="TEntity">数据库实体类</typeparam>
        /// <param name="dbUpdates">DbUpdate参数数组</param>
        /// <param name="sqlUpdate">不包含【SET】的sql update语句（例 Id=@pu,Age=@pu）</param>
        /// <returns>DynamicParameters参数</returns>
        public static DynamicParameters GetUpdateDynamicParameter<TEntity>(IEnumerable<DbUpdate<TEntity>> dbUpdates, ref string sqlUpdate) where TEntity : class
        {
            if (dbUpdates == null || !dbUpdates.Any())
                throw new ArgumentNullException(nameof(dbUpdates));
            DynamicParameters db = new DynamicParameters();
            StringBuilder builder = new StringBuilder();
            int index = 0;
            foreach (var item in dbUpdates)
            {
                builder.AppendFormat("{0}=@pu_{1},", item.PropertyName, index);
                db.Add("@pu_" + index.ToString(), item.Value);
                index++;
            }
            sqlUpdate = builder.ToString().TrimEnd(',');
            return db;
        }

        /// <summary>
        /// 解析DbOrderBy参数，输出sql order by语句（不包含【ORDER BY】关键字）
        /// </summary>
        /// <typeparam name="TEntity">数据库实体类</typeparam>
        /// <param name="dbOrderBy">DbOrderBy参数</param>
        /// <returns>sql order by语句（不包含【ORDER BY】关键字）</returns>
        public static string GetOrderBySql<TEntity>(params DbOrderBy<TEntity>[] dbOrderBy) where TEntity : class
        {
            if (dbOrderBy == null || dbOrderBy.Length == 0)
                return "";
            StringBuilder builder = new StringBuilder();
            foreach (var item in dbOrderBy)
            {
                builder.AppendFormat(" {0} {1},", item.PropertyName, item.OrderByType);
            }
            return builder.ToString().TrimEnd(',');
        }

        private static string ArrayToString(Array arr)
        {
            if (arr.GetLength(0) == 0)
                return string.Empty;

            object o = arr.GetValue(0);
            string[] str = new string[arr.GetLength(0)];
            if (o is int || o is decimal || o is double || o is float || o is long)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    str[i] = arr.GetValue(i).ToString();
                }
                return string.Join(",", str);
            }
            else
            {
                for (int i = 0; i < str.Length; i++)
                {
                    str[i] = arr.GetValue(i).ToString().Replace("'", "''");
                }
                return "'" + string.Join("','", str) + "'";
            }
        }
    }
}
