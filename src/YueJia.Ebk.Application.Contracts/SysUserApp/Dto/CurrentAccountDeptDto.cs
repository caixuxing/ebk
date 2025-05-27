namespace YueJia.Ebk.Application.Contracts.SysUserApp.Dto;


/// <summary>
/// 当前登录用户所在部门信息
/// </summary>
public record CurrentAccountDeptDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long? DeptId { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public string? DeptName { get; set; }
    /// <summary>
    /// 父级ID
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long? ParentDeptId { get; set; }

    /// <summary>
    /// 子级部门ID集合
    /// </summary>
    public List<long>? ChildDeptIds { get; set; }
}
