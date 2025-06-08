namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands;

/// <summary>
/// 保存加载库存和价格命令
/// </summary>
public class SaveLoadingInventoryAndPricesCmd
{
    /// <summary>
    /// 酒店ID
    /// </summary>

    public string HotelId { get; set; } = default!;

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 房间信息
    /// </summary>
    public List<RoomInfoCmd> Rooms { get; set; } = new();

}

/// <summary>
/// 房间信息命令
/// </summary>
public class RoomInfoCmd
{
    /// <summary>
    /// 房间ID
    /// </summary>
    public string RoomId { get; set; } = default!;

    /// <summary>
    /// 库存（周几，库存数）
    /// </summary>
    public Dictionary<DayOfWeek, int> Inventory { get; set; } = new();

    /// <summary>
    /// 价格明细
    /// </summary>
    public List<PriceCmd> Prices { get; set; } = new();
}

/// <summary>
/// 价格明细命令
/// </summary>
public class PriceCmd
{
    /// <summary>
    /// 价格计划ID
    /// </summary>
    public string PricePlanId { get; set; } = default!;

    /// <summary>
    /// （周几，价格）价格明细
    /// </summary>
    public Dictionary<DayOfWeek, decimal> DailyPrices { get; set; } = new();
}
