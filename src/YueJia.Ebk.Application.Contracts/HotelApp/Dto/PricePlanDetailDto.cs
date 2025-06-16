namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;

/// <summary>
/// 价格计划明细详情
/// </summary>
public record PricePlanDetailDto
{
    /// <summary>
    /// 价格计划ID
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// 酒店ID
    /// </summary>
    public string HotelId { get; set; }

    /// <summary>
    /// 酒店代码
    /// </summary>
    public string HotelCode { get; set; }
    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; }
    /// <summary>
    /// 酒店名称(英文)
    /// </summary>
    public string HotelNameEn { get; set; }
    /// <summary>
    /// 房型
    /// </summary>
    public string RoomType { get; set; }

    /// <summary>
    /// 房型名称
    /// </summary>
    public string RoomTypeName { get; set; }

    public BedTypeEnum BedType { get; set; }
    public string BedTypeName
    {
        get
        {
            return BedType.ToDescription();
        }
    }
    /// <summary>
    /// 餐食类型
    /// </summary>
    public BreakfastTypeEnum BreakfastType { get; set; }
    public string BreakfastTypeName
    {
        get

        {
            return BreakfastType.ToDescription();
        }
    }

    public int DaysInAdvance { get; set; }

    public int ContinuousStayDays { get; set; }

    public YesOrNoType IsReservedRoom { get; set; }

    public YesOrNoType IsEnable { get; set; }

    public string PricePlanTitle { get; set; }
}