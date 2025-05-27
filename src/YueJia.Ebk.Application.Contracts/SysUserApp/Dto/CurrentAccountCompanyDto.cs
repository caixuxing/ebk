namespace YueJia.Ebk.Application.Contracts.SysUserApp.Dto;

/// <summary>
/// 当前账户所属公司
/// </summary>
public class CurrentAccountCompanyDto
{
    /// <summary>
    /// 公司ID
    /// </summary>
    public string? CompanyId { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    ///租户ID
    /// </summary>
    public string? TenantId { get; set; }
}

