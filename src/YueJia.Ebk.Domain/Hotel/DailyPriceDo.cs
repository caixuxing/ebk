namespace YueJia.Ebk.Domain.Hotel;

/// <summary>
/// 每日价格
/// </summary>
[SugarTable("DailyPrice", "每日价格")]
public record DailyPriceDo : EntityTenant
{

    /// <summary>
    /// 房间Id
    /// </summary>
    [SugarColumn(ColumnDescription = "房间Id")]
    public long RoomId { get; init; }

    /// <summary>
    /// 价格计划Id
    /// </summary>
    [SugarColumn(ColumnDescription = "价格计划Id")]
    public long PricePlanId { get; init; }

    /// <summary>
    /// 当前日期
    /// </summary>
    [SugarColumn(ColumnDescription = "当前日期", ColumnDataType = "date")]
    public DateTime CurrentDate { get;  set; }

    /// <summary>
    /// 价格
    /// </summary>
    [SugarColumn(ColumnDescription = "价格", ColumnDataType = "decimal(8,2)")]
    public decimal Price { get;  set; }
}