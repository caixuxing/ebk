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

    public string CountryName { get; set; }
    public string CityName { get; set; }

    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;
    /// <summary>
    /// 酒店名称(英文名)
    /// </summary>

    public string HotelNameEn { get; set; } = default!;

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
    public DateTime CheckInDate { get; set; }
    /// <summary>
    /// 离店日期
    /// </summary>
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
    public decimal CostAmount { get; set; } = default!;

    /// <summary>
    /// 酒店确认号
    /// </summary>
    public string HotelConfirmNum { get; set; } = default!;

    /// <summary>
    /// 订房间数
    /// </summary>
    public int RoomNumber { get; set; }
    /// <summary>
    /// 几晚
    /// </summary>
    public int HowManyNights { get; set; }
    public DateTime CreateTime { get; set; }


    public string CreateTimeString
    {
        get {
            return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    public string CheckInDateString
    {
        get
        {
            return CheckInDate.ToString("yyyy-MM-dd");
        }
    }
    public string CheckOutDateString
    {
        get
        {
            return CheckOutDate.ToString("yyyy-MM-dd");
        }
    }

}
