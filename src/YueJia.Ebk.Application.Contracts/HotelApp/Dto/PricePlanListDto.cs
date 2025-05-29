namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;


/// <summary>
/// 价格计划集合Dto
/// </summary>
public record PricePlanListDto
{
    /// <summary>
    /// 价格计划Id
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long Id { get; set; }


    /// <summary>
    /// 酒店房间ID
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long HotelRoomId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public BreakfastTypeEnum BreakfastType { get; set; }

    public string BreakfastTypeName
    {

        get
        {

            return BreakfastType.ToDescription();

        }

    }

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
    /// 是否保留房名称
    /// </summary>
    public string IsReservedRoomName
    {
        get
        {
            return IsReservedRoom.ToDescription();
        }

    }

    /// <summary>
    /// 是否启用
    /// </summary>
    public YesOrNoType IsEnable { get; set; }


    public string IsEnableName
    {
        get
        {


            return IsEnable.ToDescription();
        }
    }
}
