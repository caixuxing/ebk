namespace YueJia.Ebk.Domain.Order;

/// <summary>
/// 订单房间每天价格明细
/// </summary>
[SugarTable("OrderDailyPrice", "订单房间每天价格明细")]
public partial record OrderDailyPriceDo : EntityBaseId
{
    public OrderDailyPriceDo() { }
    /// <summary>
    /// 预订号
    /// </summary>
    [SugarColumn(ColumnDescription = "预订号", Length = 50)]
    public string OrderNum { get; set; } = default!;
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


}

public partial record OrderDailyPriceDo
{
    private OrderDailyPriceDo(string orderNum, DateTime currentDate, decimal dayPrice)
    {
        OrderNum = orderNum;
        CurrentDate = currentDate;
        DayPrice = dayPrice;
    }

    public static OrderDailyPriceDo Create(string orderNum,  DateTime currentDate, decimal dayPrice)
    {
        return new OrderDailyPriceDo(orderNum,  currentDate, dayPrice);
    }
}