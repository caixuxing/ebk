namespace YueJia.Ebk.Application.Contracts.EbkApp;

/// <summary>
/// 查价结果Dto
/// </summary>
public record HotelPriceDto
{
    /// <summary>
    /// 酒店代码
    /// </summary>
    public string HotelCode { get; set; } = default!;

    /// <summary>
    /// 查价唯一标识
    /// </summary>
    public string SearchCode { get; set; } = default!;

    /// <summary>
    /// 房间代码
    /// </summary>
    public string RoomCode { get; set; } = default!;

    /// <summary>
    /// 房间类型
    /// </summary>
    public string RoomName { get; set; } = default!;

    /// <summary>
    /// 房间类型英文名
    /// </summary>
    public string RoomNameEn { get; set; } = default!;

    /// <summary>
    /// 餐食
    /// </summary>
    public string IsBreakfast { get; set; } = default!;

    /// <summary>
    /// 总价
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// 每日价格集合
    /// </summary>
    public Dictionary<string, decimal> DayPrice { get; set; } = new();
}
