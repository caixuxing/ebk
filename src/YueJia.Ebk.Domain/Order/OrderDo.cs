namespace YueJia.Ebk.Domain.Order;


/// <summary>
/// 订单表
/// </summary>
[SugarTable("Order", "订单表")]
public partial record OrderDo : EntityTenant
{
    public OrderDo() { }
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
    [SugarColumn(ColumnDescription = "房型Code", Length = 20)]
    public string RoomCode { get; set; } = default!;

    /// <summary>
    /// 房型名称
    /// </summary>
    [SugarColumn(ColumnDescription = "床型名称", Length = 60)]
    public string RoomName { get; set; } = default!;

    /// <summary>
    /// 床型名称
    /// </summary>
    [SugarColumn(ColumnDescription = "床型名称", Length = 60)]
    public string BedName { get; set; } = default!;

    /// <summary>
    /// 床型Code
    /// </summary>
    [SugarColumn(ColumnDescription = "床型Code", Length = 20)]
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

public partial record OrderDo
{
    private OrderDo(string orderNum, DateTime bookingDate, BookingStateTypeEnum state, string hotelCode,
        long userHotelId, string roomCode, string roomName, string bedName, string bedCode, DateTime checkInDate,
        DateTime checkOutDate, decimal totalAmount, string hotelConfirmNum, int numberOfRoomsBooked,
        int howManyNights, string customerName, string remark, string createdbyId, string createdbyName, long tenantId)
    {
        OrderNum = orderNum;
        BookingDate = bookingDate;
        State = state;
        HotelCode = hotelCode;
        UserHotelId = userHotelId;
        RoomCode = roomCode;
        RoomName = roomName;
        BedName = bedName;
        BedCode = bedCode;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        TotalAmount = totalAmount;
        HotelConfirmNum = hotelConfirmNum;
        NumberOfRoomsBooked = numberOfRoomsBooked;
        HowManyNights = howManyNights;
        CustomerName = customerName;
        Remark = remark;
        this.CreatedbyId = createdbyId;
        this.CreatedbyName = createdbyName;
        this.TenantId = tenantId;
    }

    public static OrderDo Create(string orderNum, DateTime bookingDate, BookingStateTypeEnum state, string hotelCode, long userHotelId, string roomCode, string roomName, string bedName, string bedCode, DateTime checkInDate, DateTime checkOutDate, decimal totalAmount, string hotelConfirmNum, int numberOfRoomsBooked, int howManyNights, string customerName, string remark, string createdbyId, string createdbyName, long tenantId)
    {
        return new OrderDo(orderNum, bookingDate, state, hotelCode, userHotelId, roomCode, roomName, bedName, bedCode, checkInDate, checkOutDate, totalAmount, hotelConfirmNum, numberOfRoomsBooked, howManyNights, customerName, remark, createdbyId, createdbyName, tenantId);
    }
}