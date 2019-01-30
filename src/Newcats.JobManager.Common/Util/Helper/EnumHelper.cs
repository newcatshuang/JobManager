using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Newcats.JobManager.Common.NetCore.Util.Helper
{
    public class EnumHelper
    {
        /// <summary>
        /// 把枚举对象的每一项转换成对应的类
        /// </summary>
        /// <typeparam name="T">要转换的枚举对象</typeparam>
        /// <returns></returns>
        public static List<EnumberEntity> ConvertToList<T>()
        {
            List<EnumberEntity> list = new List<EnumberEntity>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                EnumberEntity m = new EnumberEntity();

                Type type = e.GetType();
                string memberName = Enum.GetName(type, e);
                MemberInfo memberInfo = type.GetTypeInfo().GetMember(memberName).FirstOrDefault();
                m.Description = memberInfo.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute attribute ? attribute.Description : memberInfo.Name;

                m.EnumValue = Convert.ToInt32(e);
                m.EnumName = e.ToString();
                list.Add(m);
            }
            return list;
        }
    }

    /// <summary>
    /// 枚举项转换的类
    /// </summary>
    public class EnumberEntity
    {
        /// <summary>  
        /// 枚举项的值  
        /// </summary>  
        public int EnumValue { get; set; }

        /// <summary>  
        /// 枚举项的名称  
        /// </summary>  
        public string EnumName { get; set; }

        /// <summary>  
        /// 枚举项的描述  
        /// </summary>  
        public string Description { get; set; }
    }
}