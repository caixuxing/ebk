namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;


/// <summary>
/// 价格计划集合Dto
/// </summary>
public record PricePlanListDto
{
    /// <summary>
    /// 价格计划Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 价格计划名称
    /// </summary>
    public string PricePlanTitle { get; set; }

    /// <summary>
    /// 酒店房间ID
    /// </summary>
    public string HotelRoomId { get; set; }

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

    /// <summary>
    /// 是否启用
    /// </summary>
    public string IsEnableName
    {
        get
        {
            return IsEnable == YesOrNoType.Yes ? "启用" : "禁用";
        }
    }
}
