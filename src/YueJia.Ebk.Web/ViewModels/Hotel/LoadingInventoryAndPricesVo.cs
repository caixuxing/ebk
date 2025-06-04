namespace YueJia.Ebk.Web.ViewModels.Hotel;


/// <summary>
/// 加载库存和价格的视图模型
/// </summary>
public class LoadingInventoryAndPricesVo
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
    /// 酒店房间数据Json
    /// </summary>
    public string HotelRoomDataJson { get; set; } = default!;

    /// <summary>
    /// 房间Id
    /// </summary>
    public string RoomId { get; set; } = default!;
}
