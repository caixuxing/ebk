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
    /// 销售平台Code
    /// </summary>
    [SugarColumn(ColumnDescription = "销售平台Code")]
    public string SalePlatCode { get; private set; } = default!;

    ///// <summary>
    ///// 是否启用
    ///// </summary>
    //[SugarColumn(ColumnDescription = "是否启用")]
    //public YesOrNoType Status { get; private set; } = default!;
}

public partial record CompanyChannelMapDo
{
    private CompanyChannelMapDo(long companyId, string salePlatCode, long tenantId)
    {
        CompanyId = companyId;
        SalePlatCode = salePlatCode;
        TenantId = tenantId;
    }

    public static CompanyChannelMapDo Create(long companyId, string salePlatCode, long tenantId)
    {
        return new CompanyChannelMapDo(companyId, salePlatCode,  tenantId);
    }

}
