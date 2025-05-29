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
    public HotelSaleTypeMnum Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName
    {
        get
        {
            return Status.ToDescription();
        }
        private set { }
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
    /// 房间价格计划
    /// </summary>
    public List<PricePlanListDto> PricePlans { get; set; } = new();
}
