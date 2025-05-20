namespace YueJia.Ebk.Domain.AggRoot;

public interface ITenantIdFilter
{
    /// <summary>
    /// 租户Id
    /// </summary>
    long? TenantId { get; set; }
}
