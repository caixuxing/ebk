namespace YueJia.Ebk.Application.Contracts.OrderApp.Qry;

/// <summary>
/// 订单分页过滤条件模型参数
/// </summary>
public record OrderPageListFilterQry : BasePageQry
{
    /// <summary>
    /// 日期类型
    /// </summary>
    public int DateType { get; set; }

    /// <summary>
    /// 日期范围
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public record DateRange(DateTime start, DateTime end);

    /// <summary>
    /// 预订号
    /// </summary>
    public string? OrderNum { get; set; }

    /// <summary>
    /// 酒店Code
    /// </summary>
    public string? HotelCode { get; set; }
    /// <summary>
    /// 酒店名称
    /// </summary>

    public string? HotelName { get; set; }

    public string UserId { get; set; }

}
