namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;



/// <summary>
/// 价格计划明细
/// </summary>
public class PricePlanListDetailDto
{
    /// <summary>
    /// 价格计划ID
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long PricePlanId { get; set; }

    /// <summary>
    /// 酒店ID
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long HotelId { get; set; }

    /// <summary>
    /// 酒店代码
    /// </summary>
    public string HotelCode { get; set; } = default!;
    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;
    /// <summary>
    /// 酒店名称（英文）
    /// </summary>
    public string HotelNameEn { get; set; } = default!;

    /// <summary>
    /// 房型
    /// </summary>
    public string RoomType { get; set; } = default!;

    /// <summary>
    /// 房型名称
    /// </summary>
    public string RoomTypeName { get; set; } = default!;

    /// <summary>
    /// 床型
    /// </summary>
    public BedTypeEnum BedType { get; set; }
    /// <summary>
    /// 床型名称
    /// </summary>
    public string BedTypeName
    {
        get
        {
            return BedType.ToDescription();
        }
    }
    /// <summary>
    /// 状态
    /// </summary>
    public YesOrNoType Status { get; set; }
    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName
    {
        get
        {
            return Status == YesOrNoType.Yes ? "启用" : "停用";
        }
    }
    /// <summary>
    /// 人数上限
    /// </summary>
    public int MaximumNumberOfPeople { get; set; }
    /// <summary>
    /// 成人上限
    /// </summary>
    public int? AdultLimit { get; set; }

    /// <summary>
    /// 儿童上限
    /// </summary>
    public int? ChildLimit { get; set; }



    /// <summary>
    /// 酒店房间ID
    /// </summary>
    public long HotelRoomId { get; set; }
    /// <summary>
    /// 早餐类型
    /// </summary>
    public BreakfastTypeEnum BreakfastType { get; private set; }
    /// <summary>
    /// 提前天数
    /// </summary>
    public int DaysInAdvance { get; private set; }

    /// <summary>
    /// 连住天数
    /// </summary>
    public int ContinuousStayDays { get; private set; }

    /// <summary>
    /// 是否保留房
    /// </summary>
    public YesOrNoType IsReservedRoom { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public YesOrNoType IsEnable { get; private set; }
}
