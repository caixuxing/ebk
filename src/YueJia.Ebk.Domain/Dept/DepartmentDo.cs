namespace YueJia.Ebk.Domain.Dept;


/// <summary>
/// 部门
/// </summary>
[SugarTable("Department", "部门")]
public partial record DepartmentDo : EntityBase
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public DepartmentDo() { }

    /// <summary>
    /// 部门
    /// </summary>
    [SugarColumn(ColumnDescription = "部门", Length = 15)]
    public string Name { get; private set; } = default!;

    /// <summary>
    /// 部门父级ID
    /// </summary>
    [SugarColumn(ColumnDescription = "部门父级ID")]
    public long ParentId { get; private set; }

    /// <summary>
    /// 公司ID
    /// </summary>
    [SugarColumn(ColumnDescription = "公司ID")]
    public long CompanyId { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public YesOrNoType Status { get; private set; }

}


public partial record DepartmentDo
{
    private DepartmentDo(string name, long parentId, long companyId, YesOrNoType status)
    {
        Name = name;
        ParentId = parentId;
        CompanyId = companyId;
        Status = status;
        this.Id = SnowFlakeSingle.instance.getID();
    }

    public static DepartmentDo Create(string name, long parentId, long companyId, YesOrNoType status)
    {
        return new DepartmentDo(name, parentId, companyId, status);
    }

    public DepartmentDo SetName(string name)
    {
        Name = name;
        return this;
    }

    public DepartmentDo SetParentId(long parentId)
    {
        ParentId = parentId;
        return this;
    }
    public DepartmentDo SetCompanyId(long companyId)
    {
        CompanyId = companyId;
        return this;
    }

    public DepartmentDo SetStatus(YesOrNoType status)
    {
        Status = status;
        return this;
    }
}
