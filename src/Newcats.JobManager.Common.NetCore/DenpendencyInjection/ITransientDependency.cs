namespace Newcats.JobManager.Common.NetCore.DenpendencyInjection
{
    /// <summary>
    /// 依赖注入标记接口
    /// 每一次GetService都会创建一个新的实例
    /// </summary>
    public interface ITransientDependency
    {
    }
}