namespace YueJia.Ebk.Domain.AggRoot;


/// <summary>
/// 框架实体基类Id
/// </summary>
public abstract record EntityBaseId
{
    [SugarColumn(ColumnDescription = "主键ID", IsPrimaryKey = true, ColumnName = "Id")]
    public virtual long Id { get; set; }
}



/// <summary>
/// 框架实体基类
/// </summary>
[SugarIndex("index_{table}_CT", nameof(CreateTime), OrderByType.Asc)]
public abstract record EntityBase : EntityBaseId, IDeletedFilter
{
    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsNullable = true, IsOnlyIgnoreUpdate = true, InsertServerTime = true)]
    public DateTime CreateTime { get; set; }
    /// <summary>
    /// 创建者ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者ID", IsOnlyIgnoreUpdate = true, Length = 64, IsNullable = true)]
    public string? CreatedbyId { get; set; } = null!;
    /// <summary>
    /// 创建者姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者姓名", Length = 20, IsNullable = true)]
    public string? CreatedbyName { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后修改时间", IsNullable = true, IsOnlyIgnoreInsert = true, UpdateServerTime = true)]
    public DateTime LastModifiedTime { get; set; }

    /// <summary>
    /// 最后修改者ID
    /// </summary>
    [SugarColumn(ColumnDescription = "最后修改者ID", Length = 64, IsNullable = true)]
    public string? LastModifiedbyId { get; set; }
    /// <summary>
    /// 最后修改者姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "最后修改者姓名", Length = 20, IsNullable = true)]
    public string? LastModifiedbyName { get; set; }
    /// <summary>
    /// 软删除
    /// </summary>
    [SugarColumn(ColumnDescription = "软删除")]
    public virtual bool IsDelete { get; set; } = false;


    /// <summary>
    /// 版本号
    /// </summary>
    [SugarColumn(IsEnableUpdateVersionValidation = true)]
    public long Version { get; set; }
}



/// <summary>
/// 租户实体基类
/// </summary>
[SugarIndex("index_{table}_tenantid", nameof(TenantId), OrderByType.Asc)]
public abstract record EntityTenant : EntityBase, ITenantIdFilter
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsOnlyIgnoreUpdate = true)]
    public virtual long? TenantId { get; set; }
}

