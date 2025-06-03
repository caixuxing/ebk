namespace YueJia.Ebk.Web.ViewModels.Hotel;


/// <summary>
/// 库存和价格
/// </summary>
public class InventoryAndPriceVo
{

    /// <summary>
    /// 酒店Id
    /// </summary>
    public string HotelId { get; set; } = default!;

    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;
    /// <summary>
    /// 酒店名称（英文）
    /// </summary>
    public string HotelNameEn { get; set; } = default!;


    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 显示天数
    /// </summary>
    public int ShowDays { get; set; }

    /// <summary>
    /// 房间Json
    /// </summary>
    public string RoomJson { get; set; } = default!;

    /// <summary>
    /// 价格计划Json
    /// </summary>
    public string PricePlanJson { get; set; } = default!;

    /// <summary>
    /// 房型下拉框Json
    /// </summary>
    public string RoomDropdownJson { get; set; } = default!;
}
