namespace YueJia.Ebk.Application.Contracts.SysUserApp.Dto;

public record SysUserDetailsDto
{
    public long Id { get; set; }

    /// <summary>
    /// 登录账户
    /// </summary>
    public string AccountName { get; set; } = default!;
    /// <summary>
    /// 真实姓名（昵称）
    /// </summary>

    public string RealName { get; set; } = default!;
    /// <summary>
    /// 是否启用
    /// </summary>

    public YesOrNoType IsEnabled { get; set; } = default!;
    /// <summary>
    /// 部门ID
    /// </summary>

    public long? DeptId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? ContactPhone { get; set; }
}
