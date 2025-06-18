using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;

namespace YueJia.Ebk.Infrastructure.DistributedLock;

public class DistributedLockService : IDistributedLockService, ISingletonDependency
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; } = default!;
    private IAbpDistributedLock _distributedLock => LazyServiceProvider.LazyGetRequiredService<IAbpDistributedLock>();


    public async Task LockAsync(string lockKey, Func<Task> method, TimeSpan timeout = default(TimeSpan), Func<Task> timeoutMethod = null)
    {
        await using (var handle = await _distributedLock.TryAcquireAsync(lockKey, timeout))
        {
            if (handle != null)
            {
                await method();
            }
            else if (timeoutMethod != null)
            {
                await timeoutMethod();
            }
        }
    }
}
