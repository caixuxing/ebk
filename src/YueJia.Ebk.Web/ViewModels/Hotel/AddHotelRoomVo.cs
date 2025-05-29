namespace YueJia.Ebk.Web.ViewModels.Hotel;

/// <summary>
/// 添加房型ViewModel
/// </summary>
public class AddHotelRoomVo
{
    public string Id { get; set; }
    /// <summary>
    /// 酒店代码
    /// </summary>
    public string HotelCode { get; set; } = default!;
    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;
    /// <summary>
    /// 房型
    /// </summary>
    public string? RoomType { get; set; }
    /// <summary>
    /// 床型
    /// </summary>
    public string? BedType { get; set; }

    /// <summary>
    /// 人数上限
    /// </summary>
    public int? MaximumNumberOfPeople { get; set; }
    /// <summary>
    /// 成人上限
    /// </summary>
    public int? AdultLimit { get; set; }

    /// <summary>
    /// 儿童上限
    /// </summary>
    public int? ChildLimit { get; set; }

    /// <summary>
    /// 库存
    /// </summary>
    public StockVo Stock { get; set; } = default!;
}
