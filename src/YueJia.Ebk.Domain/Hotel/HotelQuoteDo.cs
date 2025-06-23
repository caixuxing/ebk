namespace YueJia.Ebk.Domain.Hotel;
using OBT = SqlSugar.OrderByType;


/// <summary>
/// 酒店报价
/// </summary>
[SugarTable("HotelQuote", "酒店报价")]

[SugarIndex("index_{table}_UHUUC",
    [nameof(UserHotelId), nameof(HotelCode), nameof(UserRoomId), nameof(UserPricePlanId), nameof(CurrentDate)],
    [OBT.Asc, OBT.Asc, OBT.Asc, OBT.Asc, OBT.Asc], isUnique: true)]
public record HotelQuoteDo : EntityBaseId, IDeletedFilter, ITenantIdFilter
{



    [SugarColumn(ColumnDescription = "主键ID", IsPrimaryKey = true, ColumnName = "Id", IsIdentity = true)]
    public new virtual long Id { get; init; }

    public HotelQuoteDo() { }
    /// <summary>
    /// 用户酒店发布ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户酒店发布ID")]
    public long UserHotelId { get; set; }
    /// <summary>
    /// 酒店编码
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店编码", Length = 30)]
    public string HotelCode { get; set; } = default!;

    /// <summary>
    /// 用户房间ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户房间ID")]
    public long UserRoomId { get; set; }

    /// <summary>
    /// 房型
    /// </summary>
    [SugarColumn(ColumnDescription = "房型", Length = 10)]
    public string RoomCode { get; set; } = default!;

    /// <summary>
    /// 酒店房间标题
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店房间标题", IsNullable = true, Length = 300)]
    public string? HotelRoomTitle { get; set; }

    /// <summary>
    /// 床型
    /// </summary>
    [SugarColumn(ColumnDescription = "床型", Length = 10)]
    public BedTypeEnum BedType { get; set; } = default!;

    /// <summary>
    /// 人数上限
    /// </summary>
    [SugarColumn(ColumnDescription = "人数上限")]
    public int MaximumNumberOfPeople { get; set; }
    /// <summary>
    /// 成人上限
    /// </summary>
    [SugarColumn(ColumnDescription = "成人上限", IsNullable = true)]
    public int? AdultLimit { get; set; }

    /// <summary>
    /// 儿童上限
    /// </summary>
    [SugarColumn(ColumnDescription = "儿童上限", IsNullable = true)]
    public int? ChildLimit { get; set; }

    /// <summary>
    /// 用户价格计划ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户价格计划ID")]
    public long UserPricePlanId { get; set; }


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
    /// 每日库存ID
    /// </summary>
    [SugarColumn(ColumnDescription = "每日库存ID")]
    public long InventoryId { get; set; }

    /// <summary>
    /// 当前日期
    /// </summary>
    [SugarColumn(ColumnDescription = "当前日期", ColumnDataType = "date")]
    public DateTime CurrentDate { get; set; }

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
    /// 租户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "租户ID", IsNullable = true)]
    public long? TenantId { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public YesOrNoType IsEnable { get; set; }


    /// <summary>
    /// 创建者ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建者ID")]
    public string CreatedbyId { get; set; } = default!;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后修改时间", IsNullable = true)]
    public DateTime LastModifiedTime { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [SugarColumn(ColumnDescription = "是否删除")]
    public bool IsDelete { get; set; }
}
