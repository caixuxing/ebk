namespace YueJia.Ebk.Infrastructure.Uilts;
/// <summary>
/// 异步订单号生成器
/// </summary>
public static class OrderNumberGenerator
{
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private static int _sequence = 0; // 自增序列
    private static readonly int MAX_SEQUENCE = 9999; // 序列最大值
    private static string _lastTimestamp = string.Empty;

    /// <summary>
    /// 异步生成订单号
    /// </summary>
    /// <param name="prefix">业务前缀(可选)</param>
    /// <param name="cancellationToken">取消令牌(可选)</param>
    /// <returns>唯一订单号</returns>
    public static async Task<string> GenerateAsync(string prefix = "", CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // 获取当前时间戳(精确到毫秒)
            var now = DateTime.Now;
            var timestamp = now.ToString("yyyyMMddHHmmssfff");

            // 如果时间戳与上次相同，则增加序列号
            if (timestamp == _lastTimestamp)
            {
                _sequence++;
                if (_sequence > MAX_SEQUENCE)
                {
                    // 如果序列号超过最大值，异步等待到下一毫秒
                    await Task.Delay(1, cancellationToken).ConfigureAwait(false);
                    now = DateTime.Now;
                    timestamp = now.ToString("yyyyMMddHHmmssfff");
                    _sequence = 0;
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            // 生成随机部分(3位)
            var random = new Random(Guid.NewGuid().GetHashCode()).Next(100, 999);

            // 组合订单号: 前缀 + 时间戳 + 序列号(4位) + 随机数(3位)
            return $"{prefix}{timestamp}{_sequence:D4}{random}";
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
