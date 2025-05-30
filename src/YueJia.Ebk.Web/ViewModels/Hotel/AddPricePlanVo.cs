namespace YueJia.Ebk.Web.ViewModels.Hotel;


/// <summary>
/// 新增/编辑价格计划视图模型
/// </summary>
public class AddPricePlanVo
{

    /// <summary>
    /// 价格计划ID
    /// </summary>
    public string PricePlanId { get; set; } = default!;

    /// <summary>
    /// 酒店ID
    /// </summary>
    public string HotelId { get; set; } = default!;

    /// <summary>
    /// 酒店编码
    /// </summary>
    public string HotelCode { get; set; } = default!;

    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;

    /// <summary>
    /// 房型名称
    /// </summary>
    public string RoomTypeName { get; set; } = default!;

    /// <summary>
    /// 床型名称
    /// </summary>
    public string BedTypeName { get; set; } = default!;



    /// <summary>
    /// 酒店房间ID
    /// </summary>
    public string HotelRoomId { get; set; }
    /// <summary>
    /// 早餐类型
    /// </summary>
    public BreakfastTypeEnum? BreakfastType { get; set; }
    /// <summary>
    /// 提前天数
    /// </summary>
    public int DaysInAdvance { get; set; }

    /// <summary>
    /// 连住天数
    /// </summary>
    public int ContinuousStayDays { get; set; }

    /// <summary>
    /// 是否保留房
    /// </summary>
    public YesOrNoType IsReservedRoom { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public YesOrNoType IsEnable { get; set; }
}
