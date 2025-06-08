using System.Threading.Channels;


/// <summary>
/// 公司渠道关联映射表
/// </summary>
[SugarTable("DepartmentChannelMap", "部门渠道关联映射表")]
public partial record DepartmentChannelMapDo : EntityTenant
{
    public DepartmentChannelMapDo()
    {
    }
    /// <summary>
    /// 公司ID
    /// </summary>
    [SugarColumn(ColumnDescription = "部门ID")]
    public long DeptId { get; private set; } = default!;

    /// <summary>
    /// 渠道ID
    /// </summary>
    [SugarColumn(ColumnDescription = "渠道ID")]
    public string SalePlatCode { get; private set; } = default!;


}

public partial record DepartmentChannelMapDo
{
    private DepartmentChannelMapDo(long deptId, string salePlatCode,  long tenantId)
    {
        DeptId = deptId;
        SalePlatCode = salePlatCode;
        TenantId = tenantId;
    }

    public static DepartmentChannelMapDo Create(long deptId, string salePlatCode, long tenantId)
    {
        return new DepartmentChannelMapDo(deptId, salePlatCode,  tenantId);
    }

    public DepartmentChannelMapDo SetCompanyId(long deptId)
    {
        DeptId = deptId;
        return this;
    }

    public DepartmentChannelMapDo SetChannelId(string salePlatCode)
    {
        SalePlatCode = salePlatCode;
        return this;
    }
}