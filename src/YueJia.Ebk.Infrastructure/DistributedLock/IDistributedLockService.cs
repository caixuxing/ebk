namespace YueJia.Ebk.Infrastructure.DistributedLock;

public interface IDistributedLockService
{
    /// <summary>
    /// 分布式事务锁封装
    /// </summary>
    /// <param name="lockKey">锁的key</param>
    /// <param name="method">方法</param>
    /// <param name="timeout">等待时间，如果默认则不等待</param>
    /// <param name="timeoutMethod">超时调用的方法</param>
    /// <returns></returns>
    Task LockAsync(string lockKey, Func<Task> method, TimeSpan timeout = default(TimeSpan), Func<Task> timeoutMethod = null);
}
