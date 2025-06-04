namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;


/// <summary>
/// 酒店房间集合Dto
/// </summary>
public record HotelRoomListDto
{
    /// <summary>
    /// 房间ID
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long Id { get; set; }

    /// <summary>
    /// 酒店代码
    /// </summary>
    public string HotelCode { get; set; } = default!;


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
    public BedTypeEnum BedType { get; set; } = default!;

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
    public YesOrNoType IsEnabled { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string IsEnabledName
    {
        get
        {
            return IsEnabled == YesOrNoType.Yes ? "启用" : "停用";
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
    /// 开始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 房间价格计划
    /// </summary>
    public List<PricePlanListDto> PricePlans { get; set; } = new();

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StartDateString
    {
        get
        {
            return StartDate.ToString("yyyy-MM-dd");
        }
    }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string EndDateString
    {
        get
        {
            return EndDate.ToString("yyyy-MM-dd");
        }
    }
}
