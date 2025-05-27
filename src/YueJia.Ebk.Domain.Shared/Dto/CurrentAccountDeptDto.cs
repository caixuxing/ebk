namespace YueJia.Ebk.Domain.Shared.Dto;


/// <summary>
/// 当前登录用户所在部门信息
/// </summary>
public record CurrentAccountDeptDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public string? DeptId { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public string? DeptName { get; set; }
    /// <summary>
    /// 父级ID
    /// </summary>
    public string? ParentDeptId { get; set; }

    /// <summary>
    /// 子级部门ID集合
    /// </summary>
    public List<string>? ChildDeptIds { get; set; }
}
