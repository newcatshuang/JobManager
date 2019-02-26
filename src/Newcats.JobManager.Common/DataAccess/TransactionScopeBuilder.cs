using System.Transactions;

namespace Newcats.JobManager.Common.DataAccess
{
    /// <summary>
    /// TransactionScope构建类
    /// </summary>
    public static class TransactionScopeBuilder
    {
        /// <summary>
        /// 设置事务隔离级别为IsolationLevel.ReadCommitted
        /// </summary>
        /// <param name="enabledAsync">是否启用异步支持</param>
        /// <returns></returns>
        public static TransactionScope CreateReadCommitted(bool enabledAsync = true)
        {
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,//默认为Serializable，锁的级别太高，死锁严重
                Timeout = TransactionManager.DefaultTimeout
            };
            return new TransactionScope(TransactionScopeOption.Required, options, enabledAsync ? TransactionScopeAsyncFlowOption.Enabled : TransactionScopeAsyncFlowOption.Suppress);
        }
    }
}