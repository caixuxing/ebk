namespace YueJia.Ebk.Domain.Order;

/// <summary>
/// 订单房间每天价格明细
/// </summary>
[SugarTable("OrderRoomDailyPriceDetail", "订单房间每天价格明细")]
public record OrderRoomDailyPriceDetailDo : EntityBaseId, IDeletedFilter
{
    /// <summary>
    /// 预订号
    /// </summary>
    [SugarColumn(ColumnDescription = "预订号", Length = 50)]
    public string OrderNum { get; set; } = default!;

    /// <summary>
    /// 订单房间Id
    /// </summary>
    [SugarColumn(ColumnDescription = "订单房间Id")]
    public long OrderRoomId { get; set; }

    /// <summary>
    /// 当前日期
    /// </summary>
    [SugarColumn(ColumnDescription = "预订号", ColumnDataType = "date")]
    public DateTime CurrentDate { get; set; }

    /// <summary>
    /// 当日价格
    /// </summary>
    [SugarColumn(ColumnDescription = "当日价格", ColumnDataType = "decimal(10,2)")]
    public decimal DayPrice { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [SugarColumn(ColumnDescription = "是否删除")]
    public bool IsDelete { get; set; }
}