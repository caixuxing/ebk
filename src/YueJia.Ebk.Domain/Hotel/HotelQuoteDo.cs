namespace YueJia.Ebk.Domain.Hotel;
using OBT = SqlSugar.OrderByType;


/// <summary>
/// 酒店报价
/// </summary>
[SugarTable("HotelQuote", "酒店报价")]

[SugarIndex("index_{table}_UHUUC",
    [nameof(UserHotelId), nameof(HotelCode), nameof(UserRoomId), nameof(UserPricePlanId), nameof(CurrentDate)],
    [OBT.Asc, OBT.Asc, OBT.Asc, OBT.Asc, OBT.Asc], isUnique: true)]
public record HotelQuoteDo : EntityBaseId 
{

    [SugarColumn(ColumnDescription = "主键ID", IsPrimaryKey = true, ColumnName = "Id", IsIdentity = true)]
    public new virtual long Id { get; init; }

    public HotelQuoteDo() { }

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID")]
    public string UserId { get; set; } = default!;

    [SugarColumn(ColumnDescription = "公司Id")]
    public long CompanyId { get; set; }

    [SugarColumn(ColumnDescription = "部门Id")]
    public long DeptId { get; set; }

    /// <summary>
    /// 酒店编码
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店编码", Length = 50)]
    public string HotelCode { get; set; }

    /// <summary>
    /// 房型
    /// </summary>
    [SugarColumn(ColumnDescription = "房型", Length = 50)]
    public string RoomCode { get; set; }


    /// <summary>
    /// 用户酒店发布ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户酒店发布ID")]
    public long UserHotelId { get; set; }

    /// <summary>
    /// 用户房间ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户房间ID")]
    public long UserRoomId { get; set; }

    /// <summary>
    /// 用户价格计划ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户价格计划ID")]
    public long UserPricePlanId { get; set; }

    /// <summary>
    /// 每日库存ID
    /// </summary>
    [SugarColumn(ColumnDescription = "每日库存ID")]
    public long DailyInventoryId { get; set; }

    /// <summary>
    /// 每日报价ID
    /// </summary>
    [SugarColumn(ColumnDescription = "每日报价ID")]
    public long DailyPriceId { get; set; }



    /// <summary>
    /// 当日库存数
    /// </summary>
    [SugarColumn(ColumnDescription = "当日库存数")]
    public int InventoryNum { get; set; }

    /// <summary>
    /// 当日价格
    /// </summary>
    [SugarColumn(ColumnDescription = "价格", ColumnDataType = "decimal(8,2)")]
    public decimal Price { get; set; }



    /// <summary>
    /// 人数上限
    /// </summary>
    [SugarColumn(ColumnDescription = "人数上限")]
    public int MaximumNumberOfPeople { get; set; }
    /// <summary>
    /// 成人上限
    /// </summary>
    [SugarColumn(ColumnDescription = "成人上限")]
    public int AdultLimit { get; set; }

    /// <summary>
    /// 儿童上限
    /// </summary>
    [SugarColumn(ColumnDescription = "儿童上限")]
    public int ChildLimit { get; set; }

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
    /// 当前日期
    /// </summary>
    [SugarColumn(ColumnDescription = "当前日期", ColumnDataType = "date")]
    public DateTime CurrentDate { get; set; }


    /// <summary>
    /// 最后修改时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后修改时间")]
    public DateTime LastModifiedTime { get; set; }

    
    /// <summary>
    /// 调价类型
    /// </summary>
    [SugarColumn(ColumnDescription = "调价类型")]
    public AdjustmentPriceTypeEnum AdjustmentPriceType { get; set; }

    /// <summary>
    /// 调价值
    /// </summary>
    [SugarColumn(ColumnDescription = "调价值")]
    public int AdjustmentPriceValue { get; set; }


    /// <summary>
    /// 公司状态
    /// </summary>
    [SugarColumn(ColumnDescription = "公司状态")]
    public bool CompanyStatus { get; set; }

    /// <summary>
    /// 用户状态
    /// </summary>
    [SugarColumn(ColumnDescription = "用户状态")]
    public bool SysUserStatus { get; set; }

    /// <summary>
    /// 用户酒店状态
    /// </summary>
    [SugarColumn(ColumnDescription = "用户酒店状态")]
    public bool UserHotelStatus { get; set; }

    /// <summary>
    /// 用户房间状态
    /// </summary>
    [SugarColumn(ColumnDescription = "用户房间状态")]
    public bool UserRoomStatus { get; set; }

    /// <summary>
    /// 用户价格计划状态
    /// </summary>
    [SugarColumn(ColumnDescription = "用户价格计划状态")]
    public bool UserPlanStatus { get; set; }

    /// <summary>
    /// 报价状态
    /// </summary>
    [SugarColumn(ColumnDescription = "报价状态")]
    public bool DailyPriceStatus { get; set; }

    /// <summary>
    /// 库存状态
    /// </summary>
    [SugarColumn(ColumnDescription = "库存状态")]
    public bool DailyInventoryStatus { get; set; }

}
