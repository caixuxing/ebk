namespace YueJia.Ebk.Domain.Company;


/// <summary>
/// 公司渠道关联映射表
/// </summary>
[SugarTable("CompanyChannelMap", "公司渠道关联映射表")]
public partial record CompanyChannelMapDo : EntityTenant
{
    public CompanyChannelMapDo()
    {
    }
    /// <summary>
    /// 公司ID
    /// </summary>
    [SugarColumn(ColumnDescription = "公司ID")]
    public long CompanyId { get; private set; } = default!;

    /// <summary>
    /// 渠道ID
    /// </summary>
    [SugarColumn(ColumnDescription = "公司ID")]
    public long ChannelId { get; private set; } = default!;

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public YesOrNoType Status { get; private set; } = default!;
}

public partial record CompanyChannelMapDo
{
    private CompanyChannelMapDo(long companyId, long channelId, YesOrNoType status, long tenantId)
    {
        CompanyId = companyId;
        ChannelId = channelId;
        Status = status;
        TenantId = tenantId;
    }

    public static CompanyChannelMapDo Create(long companyId, long channelId, long tenantId)
    {
        return new CompanyChannelMapDo(companyId, channelId, YesOrNoType.Yes, tenantId);
    }

    public CompanyChannelMapDo SetCompanyId(long companyId)
    {
        CompanyId = companyId;
        return this;
    }

    public CompanyChannelMapDo SetChannelId(long channelId)
    {
        ChannelId = channelId;
        return this;
    }

    public CompanyChannelMapDo SetStatus(YesOrNoType status)
    {
        Status = status;
        return this;
    }
}
