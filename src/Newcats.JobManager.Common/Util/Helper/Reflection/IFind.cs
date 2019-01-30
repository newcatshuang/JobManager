using System;
using System.Collections.Generic;
using System.Reflection;

namespace Newcats.JobManager.Common.Util.Helper.Reflection
{
    /// <summary>
    /// 类型查找器
    /// 引用自dotnet core开源项目Util
    /// 作者:何镇汐
    /// https://github.com/dotnetcore/Util/blob/master/src/Util/Reflections/IFind.cs
    /// </summary>
    public interface IFind
    {
        /// <summary>
        /// 获取程序集列表
        /// </summary>
        List<Assembly> GetAssemblies();
        /// <summary>
        /// 查找类型列表
        /// </summary>
        /// <typeparam name="T">查找类型</typeparam>
        /// <param name="assemblies">在指定的程序集列表中查找</param>
        List<Type> Find<T>(List<Assembly> assemblies = null);
        /// <summary>
        /// 查找类型列表
        /// </summary>
        /// <param name="findType">查找类型</param>
        /// <param name="assemblies">在指定的程序集列表中查找</param>
        List<Type> Find(Type findType, List<Assembly> assemblies = null);
    }
}