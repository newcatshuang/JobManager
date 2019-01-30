using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;

namespace Newcats.JobManager.Common.NetCore.Util.Helper.Reflection
{
    /// <summary>
    /// Web类型查找器
    /// </summary>
    public class WebFinder : Finder
    {
        /// <summary>
        /// 获取程序集列表
        /// </summary>
        public override List<Assembly> GetAssemblies()
        {
            LoadAssemblies(PlatformServices.Default.Application.ApplicationBasePath);
            return base.GetAssemblies();
        }
    }
}