namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;

/// <summary>
/// 加载库存和价格DTO
/// </summary>
public record LoadingInventoryAndPricesDto
{
    /// <summary>
    /// 酒店ID
    /// </summary>
    public string HotelId { get; set; } = default!;
    /// <summary>
    /// 酒店代码
    /// </summary>
    public string HotelCode { get; set; } = default!;
    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;
    /// <summary>
    /// 酒店名称(英文)
    /// </summary>
    public string HotelNameEn { get; set; } = default!;

    /// <summary>
    /// 房型集合
    /// </summary>
    public List<TreeSelectDataDto<string>> RoomTypes { get; set; } = new();

    /// <summary>
    /// 房型默认选择中
    /// </summary>
    public string RoomTypeDefaultValue
    {
        get
        {
            return RoomTypes.FirstOrDefault()?.Children.FirstOrDefault()?.Value ?? string.Empty;
        }
    }
    /// <summary>
    /// 酒店所有房型价格计划
    /// </summary>
    public List<PricePlanItemDto> HotelRoomPricePlanAll { get; set; } = new();

    /// <summary>
    /// 当前房型价格计划
    /// </summary>
    public List<PricePlanItemDto> CruuentRoomPricePlan
    {
        get
        {
            return HotelRoomPricePlanAll.Where(x => x.RoomId == RoomTypeDefaultValue).ToList();
        }
    }
}
/// <summary>
/// 房间价格计划
/// </summary>
public record PricePlanItemDto
{
    /// <summary>
    /// 价格计划ID
    /// </summary>
    public string PricePlanId { get; set; } = default!;

    /// <summary>
    /// 房型ID
    /// </summary>
    public string RoomId { get; set; } = default!;

    /// <summary>
    /// 价格计划名称
    /// </summary>

    public string PricePlanName { get; set; } = default!;
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
            return Status.ToDescription();
        }
    }
    /// <summary>
    /// 每日价格
    /// </summary>
    public List<DailyPriceDto> DailyPrices { get; set; } = new();
}
