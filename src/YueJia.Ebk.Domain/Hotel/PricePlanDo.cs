namespace YueJia.Ebk.Domain.Hotel;

[SugarTable("PricePlan", "价格计划")]
public partial record PricePlanDo : EntityTenant
{
    public PricePlanDo() { }


    /// <summary>
    /// 酒店房间ID
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店房间ID")]
    public long HotelRoomId { get; set; }

    /// <summary>
    /// 早餐类型
    /// </summary>
    [SugarColumn(ColumnDescription = "早餐类型")]
    public BreakfastTypeEnum BreakfastType { get; private set; }
    /// <summary>
    /// 提前天数
    /// </summary>
    [SugarColumn(ColumnDescription = "提前天数")]
    public int DaysInAdvance { get; private set; }

    /// <summary>
    /// 连住天数
    /// </summary>
    [SugarColumn(ColumnDescription = "提前天数")]
    public int ContinuousStayDays { get; private set; }

    /// <summary>
    /// 是否保留房
    /// </summary>
    [SugarColumn(ColumnDescription = "提前天数")]
    public YesOrNoType IsReservedRoom { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "提前天数")]
    public YesOrNoType IsEnable { get; private set; }
}


public partial record PricePlanDo
{
    private PricePlanDo(long hotelRoomId, BreakfastTypeEnum breakfastType, int daysInAdvance, int continuousStayDays, YesOrNoType isReservedRoom, YesOrNoType isEnable)
    {
        HotelRoomId = hotelRoomId;
        BreakfastType = breakfastType;
        DaysInAdvance = daysInAdvance;
        ContinuousStayDays = continuousStayDays;
        IsReservedRoom = isReservedRoom;
        IsEnable = isEnable;
    }
    public static PricePlanDo Create(long hotelRoomId, BreakfastTypeEnum breakfastType, int daysInAdvance, int continuousStayDays, YesOrNoType isReservedRoom, YesOrNoType isEnable)
    {
        return new PricePlanDo(hotelRoomId, breakfastType, daysInAdvance, continuousStayDays, isReservedRoom, isEnable);
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
        IsEnable = isEnable;
        return this;
    }
}
