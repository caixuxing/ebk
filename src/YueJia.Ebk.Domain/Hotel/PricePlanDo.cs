namespace YueJia.Ebk.Domain.Hotel;

[SugarTable("PricePlan", "价格计划")]
public partial record PricePlanDo : EntityTenant
{
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public PricePlanDo() { }


    /// <summary>
    /// 酒店房间ID
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店房间ID")]
    public long HotelRoomId { get; set; }

    /// <summary>
    /// 价格计划标题
    /// </summary>
    [SugarColumn(ColumnDescription = "价格计划标题", Length = 300, IsNullable = true)]
    public string? PricePlanTitle { get; set; }

    /// <summary>
    /// 早餐类型
    /// </summary>
    [SugarColumn(ColumnDescription = "早餐类型")]
    public BreakfastTypeEnum BreakfastType { get; set; }
    /// <summary>
    /// 提前天数
    /// </summary>
    [SugarColumn(ColumnDescription = "提前天数")]
    public int DaysInAdvance { get; set; }

    /// <summary>
    /// 连住天数
    /// </summary>
    [SugarColumn(ColumnDescription = "连住天数")]
    public int ContinuousStayDays { get; set; }

    /// <summary>
    /// 是否保留房
    /// </summary>
    [SugarColumn(ColumnDescription = "是否保留房")]
    public YesOrNoType IsReservedRoom { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public YesOrNoType IsEnabled { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 100, IsNullable = true)]
    public string? Remark { get; set; }
}


public partial record PricePlanDo
{


    public PricePlanDo(long hotelRoomId, string pricePlanTitle, BreakfastTypeEnum breakfastType, int daysInAdvance, int continuousStayDays, YesOrNoType isReservedRoom, YesOrNoType isEnable)
    {
        Id = SnowFlakeSingle.instance.getID();
        HotelRoomId = hotelRoomId;
        PricePlanTitle = pricePlanTitle;
        BreakfastType = breakfastType;
        DaysInAdvance = daysInAdvance;
        ContinuousStayDays = continuousStayDays;
        IsReservedRoom = isReservedRoom;
        IsEnabled = isEnable;
    }

    public static PricePlanDo Create(long hotelRoomId, string pricePlanTitle, BreakfastTypeEnum breakfastType, int daysInAdvance, int continuousStayDays, YesOrNoType isReservedRoom, YesOrNoType isEnable)
    {
        return new PricePlanDo(hotelRoomId, pricePlanTitle, breakfastType, daysInAdvance, continuousStayDays, isReservedRoom, isEnable);
    }



    public PricePlanDo SetBreakfastType(BreakfastTypeEnum breakfastType)
    {
        BreakfastType = breakfastType;
        return this;
    }

    public PricePlanDo SetDaysInAdvance(int daysInAdvance)
    {
        DaysInAdvance = daysInAdvance;
        return this;
    }

    public PricePlanDo SetContinuousStayDays(int continuousStayDays)
    {
        ContinuousStayDays = continuousStayDays;
        return this;
    }

    public PricePlanDo SetIsReservedRoom(YesOrNoType isReservedRoom)
    {
        IsReservedRoom = isReservedRoom;
        return this;
    }

    public PricePlanDo SetIsEnable(YesOrNoType isEnable)
    {
        IsEnabled = isEnable;
        return this;
    }
}
