namespace YueJia.Ebk.Application.Contracts.SysUserApp.Query;

/// <summary>
/// 系统用户分页筛选条件模型
/// </summary>
public record SysUserPageFilterQry : BasePageQry
{
    public string? AccountName { get; set; }
    /// <summary>
    /// 真实姓名（昵称）
    /// </summary>

    public string? RealName { get; set; }
    /// <summary>
    /// 是否启用
    /// </summary>
    public YesOrNoType? IsEnabled { get; set; }
}
