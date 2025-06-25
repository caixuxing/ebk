namespace YueJia.Ebk.Domain.Other;


/// <summary>
/// 日志
/// </summary>
[SugarTable("CustomerLogs", "审计日志")]
public partial record CustomerLogsDo : EntityBaseId, ITenantIdFilter
{
    /// <summary>
    /// 事件名称
    /// </summary>
    [SugarColumn(ColumnDescription = "事件名称", Length = 100, IsNullable = true)]
    public string EventName { get; set; } = default!;

    /// <summary>
    /// 事件简介
    /// </summary>
    [SugarColumn(ColumnDescription = "事件简介", IsNullable = true)]
    public string? EventDesc { get; set; }

    /// <summary>
    /// 更改前内容
    /// </summary>
    [SugarColumn(ColumnDescription = "更改前内容", ColumnDataType = "text", IsNullable = true)]
    public string? BeforeContent { get; set; }
    /// <summary>
    /// 更改后内容
    /// </summary>
    [SugarColumn(ColumnDescription = "更改后内容", ColumnDataType = "text", IsNullable = true)]
    public string? AfterContent { get; set; }

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
    /// 租户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者姓名", IsNullable = true)]
    public long? TenantId { get; set; }

}


public partial record CustomerLogsDo
{
    public CustomerLogsDo() { }

    private CustomerLogsDo(string eventName, string? eventDesc, string? beforeContent, string? afterContent)
    {
        EventName = eventName;
        EventDesc = eventDesc;
        BeforeContent = beforeContent;
        AfterContent = afterContent;
    }


    public static CustomerLogsDo Create(string eventName, string? eventDesc, string? beforeContent, string? afterContent)
    {
        return new CustomerLogsDo(eventName, eventDesc, beforeContent, afterContent);
    }

}