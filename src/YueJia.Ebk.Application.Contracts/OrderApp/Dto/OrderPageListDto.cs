namespace YueJia.Ebk.Application.Contracts.OrderApp.Dto;


/// <summary>
/// 订单分页列表Dto
/// </summary>
public record OrderPageListDto
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
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;
    /// <summary>
    /// 酒店名称(英文名)
    /// </summary>

    public string HotelNameEn { get; set; } = default!;

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
    /// 酒店确认号
    /// </summary>
    public string HotelConfirmNum { get; set; } = default!;

    /// <summary>
    /// 订房间数
    /// </summary>
    public int NumberOfRoomsBooked { get; set; }
    /// <summary>
    /// 几晚
    /// </summary>
    public int HowManyNights { get; set; }

    /// <summary>
    /// 客户姓名（第一房  第一入住人）
    /// </summary>
    public string CustomerName { get; set; } = default!;

    /// <summary>
    /// 房型名称
    /// </summary>
    public string RoomName { get; set; } = default!;

    /// <summary>
    /// 床型名称
    /// </summary>
    public string BedName { get; set; } = default!;

}
