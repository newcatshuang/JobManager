namespace Newcats.JobManager.Common.NetCore.DenpendencyInjection
{
    /// <summary>
    /// 依赖注入标记接口
    /// 在同一个Scope内只初始化一个实例 ，可以理解为（ 每一个request级别只创建一个实例，同一个http request会在一个 scope内）
    /// </summary>
    public interface IScopeDependency
    {
    }
}