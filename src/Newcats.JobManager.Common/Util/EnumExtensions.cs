using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Newcats.JobManager.Common.Util
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取描述,使用System.ComponentModel.Description特性设置描述
        /// </summary>
        /// <param name="value">当前枚举项</param>
        /// <returns>Description特性描述</returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string memberName = Enum.GetName(type, value);
            MemberInfo memberInfo = type.GetTypeInfo().GetMember(memberName).FirstOrDefault();
            return memberInfo.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute attribute ? attribute.Description : memberInfo.Name;
        }
    }
}
