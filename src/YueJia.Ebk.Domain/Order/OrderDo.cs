namespace YueJia.Ebk.Domain.Order;


/// <summary>
/// 订单表
/// </summary>
[SugarTable("Order", "订单表")]
public partial record OrderDo : EntityTenant
{
    /// <summary>
    /// 预订号
    /// </summary>
    [SugarColumn(ColumnDescription = "预订号", Length = 50)]
    public string OrderNum { get; set; } = default!;

    /// <summary>
    /// 预订日期
    /// </summary>
    [SugarColumn(ColumnDescription = "预订日期")]
    public DateTime BookingDate { get; set; } = default!;

    /// <summary>
    /// 预订单状态
    /// </summary>
    [SugarColumn(ColumnDescription = "预订单状态")]
    public BookingStateTypeEnum State { get; set; } = default!;

    /// <summary>
    /// 酒店Code
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店Code", Length = 50)]
    public string HotelCode { get; set; } = default!;

    /// <summary>
    /// 用户酒店ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户酒店ID")]
    public long UserHotelId { get; set; }

    /// <summary>
    /// 房型Code
    /// </summary>
    [SugarColumn(ColumnDescription = "房型Code")]
    public string RoomCode { get; set; } = default!;

    /// <summary>
    /// 房型名称
    /// </summary>
    [SugarColumn(ColumnDescription = "床型名称")]
    public string RoomName { get; set; } = default!;

    /// <summary>
    /// 床型名称
    /// </summary>
    [SugarColumn(ColumnDescription = "床型名称")]
    public string BedName { get; set; } = default!;

    /// <summary>
    /// 床型Code
    /// </summary>
    [SugarColumn(ColumnDescription = "床型名称")]
    public string BedCode { get; set; } = default!;

    /// <summary>
    /// 入店日期
    /// </summary>
    [SugarColumn(ColumnDescription = "入店日期", ColumnDataType = "date")]
    public DateTime CheckInDate { get; set; }
    /// <summary>
    /// 离店日期
    /// </summary>
    [SugarColumn(ColumnDescription = "离店日期", ColumnDataType = "date")]
    public DateTime CheckOutDate { get; set; }

    /// <summary>
    /// 订单总金额
    /// </summary>
    [SugarColumn(ColumnDescription = "订单总金额", ColumnDataType = "decimal(10,2)")]
    public decimal TotalAmount { get; set; } = default!;

    /// <summary>
    /// 酒店确认号
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店确认号", IsNullable = true, Length = 50)]
    public string HotelConfirmNum { get; set; } = default!;

    /// <summary>
    /// 订房间数
    /// </summary>
    [SugarColumn(ColumnDescription = "订房间数")]
    public int NumberOfRoomsBooked { get; set; }

    /// <summary>
    /// 几晚
    /// </summary>
    [SugarColumn(ColumnDescription = "几晚")]
    public int HowManyNights { get; set; }

    /// <summary>
    /// 客户姓名（第一房  第一入住人）
    /// </summary>
    [SugarColumn(ColumnDescription = "客户姓名", Length = 30)]
    public string CustomerName { get; set; } = default!;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 100)]

    public string Remark { get; set; } = default!;

}
