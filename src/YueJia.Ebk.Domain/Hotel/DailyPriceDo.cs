namespace YueJia.Ebk.Domain.Hotel;

/// <summary>
/// 每日价格
/// </summary>
[SugarTable("DailyPrice", "每日价格")]
public partial record DailyPriceDo : EntityTenant
{
    public DailyPriceDo() { }
    /// <summary>
    /// 房间Id
    /// </summary>
    [SugarColumn(ColumnDescription = "房间Id")]
    public long RoomId { get; init; }

    /// <summary>
    /// 价格计划Id
    /// </summary>
    [SugarColumn(ColumnDescription = "价格计划Id")]
    public long PricePlanId { get; init; }

    /// <summary>
    /// 当前日期
    /// </summary>
    [SugarColumn(ColumnDescription = "当前日期", ColumnDataType = "date")]
    public DateTime CurrentDate { get; init; }

    /// <summary>
    /// 价格
    /// </summary>
    [SugarColumn(ColumnDescription = "价格", ColumnDataType = "decimal(8,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public YesOrNoType IsEnable { get; set; }
}
public partial record DailyPriceDo
{
    private DailyPriceDo(long roomId, long pricePlanId, DateTime currentDate, decimal price, YesOrNoType isEnable)
    {
        Id = SnowFlakeSingle.instance.getID();
        RoomId = roomId;
        PricePlanId = pricePlanId;
        CurrentDate = currentDate;
        Price = price;
        IsEnable = isEnable;
    }

    public static DailyPriceDo Create(long roomId, long pricePlanId, DateTime currentDate, decimal price, YesOrNoType isEnable)
    {
        return new DailyPriceDo(roomId, pricePlanId, currentDate, price, isEnable);
    }

    public DailyPriceDo SetPrice(decimal price)
    {
        Price = price;
        return this;
    }
    public DailyPriceDo SetIsEnable(YesOrNoType isEnable)
    {
        IsEnable = isEnable;
        return this;
    }


    public DailyPriceDo CreateByInfo(long tenantId, string createdbyId, string createdbyName)
    {
        this.TenantId = tenantId;
        this.CreatedbyId = createdbyId;
        this.CreatedbyName = createdbyName;
        this.CreateTime = DateTime.Now;
        return this;
    }

    public DailyPriceDo UpdateByInfo(string updatedbyId, string updatedbyName)
    {
        this.LastModifiedbyId = updatedbyId;
        this.LastModifiedbyName = updatedbyName;
        this.LastModifiedTime = DateTime.Now;
        return this;
    }
}