using System;

namespace Newcats.JobManager.Api.Infrastructure.Helper
{
    public class ConvertHelper
    {
        /// <summary>
        /// 通用泛型转换
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">输入值</param>
        public static T To<T>(object input)
        {
            if (input == null)
                return default(T);
            if (input is string && string.IsNullOrWhiteSpace(input.ToString()))
                return default(T);
            Type type = Nullable.GetUnderlyingType(typeof(T));//Common.GetType<T>();
            var typeName = type.Name.ToLower();
            try
            {
                if (typeName == "string")
                    return (T)(object)input.ToString();
                if (typeName == "guid")
                    return (T)(object)new Guid(input.ToString());
                if (type.IsEnum)
                    return (T)Enum.Parse(type, input.ToString());
                if (input is IConvertible)
                    return (T)Convert.ChangeType(input, type);
                return (T)input;
            }
            catch
            {
                return default(T);
            }
        }
    }
}