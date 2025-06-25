namespace YueJia.Ebk.Domain.Other;


/// <summary>
/// 追踪
/// </summary>
[SugarTable("Tracing", "追踪")]
public record TracingDo 
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(ColumnDescription = "主键ID", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; } = default!;
    /// <summary>
    /// 当前业务表主键ID
    /// </summary>
    [SugarColumn(ColumnDescription = "Id")]
    public long TableId { get; init; }
    /// <summary>
    /// 表名
    /// </summary>
    [SugarColumn(ColumnDescription = "表名", Length = 50, IsNullable = true)]
    public string? TableName { get; set; }

    ///// <summary>
    ///// 动作类型
    ///// </summary>
    //[SugarColumn(ColumnDescription = "动作类型", Length = 5, IsNullable = true)]
    //public string? ActionType { get; set; }

    /// <summary>
    /// 状态（默认0：待处理,1:已完成,2:异常）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态（默认0：待处理,1:已完成,2:异常）", IsNullable = true, DefaultValue = "0")]
    public int State { get; set; } = 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", ColumnDataType = "datetime")]
    public DateTime CreateTime { get; set; }

    ///// <summary>
    ///// 是否删除
    ///// </summary>
    //[SugarColumn(ColumnDescription = "是否删除", DefaultValue = "0")]
    //public bool IsDelete { get; set; }
}
