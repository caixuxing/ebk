namespace YueJia.Ebk.Application.Contracts.OrderApp.Dto;

/// <summary>
/// 订单详情DTO
/// </summary>
public record OrderDetailDto
{

    /// <summary>
    /// 订单ID
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }
    /// <summary>
    /// 预订号
    /// </summary>
    public string OrderNum { get; set; } = default!;

    /// <summary>
    /// 预订日期
    /// </summary>
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime BookingDate { get; set; } = default!;

    /// <summary>
    /// 预订单状态
    /// </summary>
    public BookingStateTypeEnum State { get; set; } = default!;
    /// <summary>
    /// 预订单状态描述
    /// </summary>

    public string StateName
    {
        get
        {
            return State.ToDescription();
        }
    }
    /// <summary>
    /// 房型名称
    /// </summary>
    public string RoomName { get; set; } = default!;


    /// <summary>
    /// 床型
    /// </summary>
    public string BedTypeName { get; set; } = default!;

    /// <summary>
    /// 入店日期
    /// </summary>
    [JsonConverter(typeof(DateConverter))]
    public DateTime CheckInDate { get; set; }
    /// <summary>
    /// 离店日期
    /// </summary>
    [JsonConverter(typeof(DateConverter))]
    public DateTime CheckOutDate { get; set; }

    /// <summary>
    /// 餐食类型
    /// </summary>
    public BreakfastTypeEnum BreakfastType { get; set; }

    /// <summary>
    /// 餐食类型描述
    /// </summary>
    public string BreakfastTypeName
    {
        get { return BreakfastType.ToDescription(); }

    }

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal TotalAmount { get; set; } = default!;

    /// <summary>
    /// 客户备注
    /// </summary>
    public string CustRemark { get; set; } = default!;

    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;
    /// <summary>
    /// 酒店名称(英文名)
    /// </summary>

    public string HotelNameEn { get; set; } = default!;

    /// <summary>
    /// 区域
    /// </summary>
    public string Area { get; set; } = default!;

    /// <summary>
    ///酒店地址
    /// </summary>
    public string Address { get; set; } = default!;

    /// <summary>
    /// 酒店联系方式
    /// </summary>
    public string Contact { get; set; } = default!;

    /// <summary>
    /// 酒店确认号
    /// </summary>
    public string HotelConfirmNum { get; set; } = default!;

    /// <summary>
    /// 房间入住明细
    /// </summary>
    public List<HotelRoomInfoOB> HotelRoomInfo { get; set; } = new();

}


/// <summary>
/// 房间信息
/// </summary>
public record HotelRoomInfoOB
{


    /// <summary>
    /// 价格计划名称
    /// </summary>
    public string PricePlanTitle { get; set; } = default!;

    /// <summary>
    /// 成人集合
    /// </summary>
    public List<string> Adult { get; set; } = new();
    /// <summary>
    /// 儿童集合
    /// </summary>
    public List<string> Child { get; set; } = new();

    /// <summary>
    /// 每日价格（日期YYYY-MM-DD）,价格
    /// </summary>
    public Dictionary<string, decimal> DailyPrice { get; set; } = new();
}
