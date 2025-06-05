namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;

/// <summary>
/// 库存和价格DTO
/// </summary>
public class InventoryAndPriceDto
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
    /// 酒店代码
    /// </summary>
    public string HotelCode { get; set; } = default!;

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// 显示天数
    /// </summary>
    public int ShowDays { get; set; }



    /// <summary>
    /// 房型默认选中值
    /// </summary>
    public string RoomTypeValue { get; set; } = default!;

    /// <summary>
    /// 房型信息
    /// </summary>
    public RoomTypeInfoDto RoomTypeInfo { get; set; } = default!;

    /// <summary>
    ///房型下拉集合
    /// </summary>
    public List<SelectDataDto<string>> RoomDropDownList { get; set; } = new();
}

/// <summary>
/// 房型信息
/// </summary>
public class RoomTypeInfoDto
{
    /// <summary>
    /// 房间Id
    /// </summary>
    public string RoomId { get; set; } = default!;

    /// <summary>
    /// 房型标题（多个房间信息组合值）
    /// </summary>
    public string? HotelRoomTitle { get; set; }

    /// <summary>
    /// 房间状态
    /// </summary>
    public YesOrNoType? Status { get; set; }

    /// <summary>
    /// 房间状态描述
    /// </summary>
    public string StatusName
    {
        get
        {
            return Status?.ToDescription() ?? string.Empty;
        }
    }
    /// <summary>
    /// 每日库存集合
    /// </summary>
    public List<DailyInventoryDto> DailyInventory { get; set; } = new();

    /// <summary>
    /// 价格计划集合
    /// </summary>
    public List<PricePlanItemDto> PricePlan { get; set; } = new();
}

/// <summary>
/// 每日库存Dto
/// </summary>
public class DailyInventoryDto
{
    /// <summary>
    /// 库存Id
    /// </summary>
    public string InventoryId { get; set; } = default!;

    public DateTime CurrentDate { get; set; }
    /// <summary>
    /// 月-日（01-01）
    /// </summary>
    public string MonthDay
    {
        get
        {
            return CurrentDate.ToString("MM-dd");
        }
    }

    /// <summary>
    /// 月-日（01-01）
    /// </summary>
    public string CurrentDateString
    {
        get
        {
            return CurrentDate.ToString("yyyy-MM-dd");
        }
    }
    /// <summary>
    /// 星期
    /// </summary>
    public string DayOfWeek
    {
        get
        {
            return "周" + "日一二三四五六"[(int)CurrentDate.DayOfWeek];
        }
    }
    /// <summary>
    /// 库存数量
    /// </summary>
    public int InventoryNum { get; set; }
    /// <summary>
    /// 库存状态
    /// </summary>
    public YesOrNoType Status { get; set; } = default!;

    /// <summary>
    /// 库存状态描述
    /// </summary>

    public string StatusName
    {
        get
        {
            return Status.ToDescription();
        }
    }
}
/// <summary>
/// 每日价格
/// </summary>
public class DailyPriceDto
{
    /// <summary>
    /// 价格Id
    /// </summary>
    public string PriceId { get; set; } = default!;

    /// <summary>
    /// 房间Id
    /// </summary>
    public string RoomId { get; set; } = default!;

    /// <summary>
    /// 价格计划Id
    /// </summary>
    public string PricePlanId { get; set; } = default!;

    /// <summary>
    /// 月-日（01-01）
    /// </summary>
    public string MonthDay { get; set; } = default!;

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 价格状态
    /// </summary>
    public YesOrNoType Status { get; set; } = default!;

    /// <summary>
    /// 价格状态描述
    /// </summary>
    public string StatusName
    {
        get
        {
            return Status.ToDescription();
        }
    }
}
