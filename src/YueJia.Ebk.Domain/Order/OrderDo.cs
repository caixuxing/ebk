using Volo.Abp.Domain.Entities;

namespace YueJia.Ebk.Domain.Order;


/// <summary>
/// 订单表
/// </summary>
[SugarTable("Order", "订单表")]
[SugarIndex("index_{table}_OrderNum", nameof(OrderNum), OrderByType.Asc, true)]
public partial record OrderDo : EntityTenant, IEntity<long>
{
    public OrderDo() { }
    /// <summary>
    /// 预订号
    /// </summary>
    [SugarColumn(ColumnDescription = "预订号", Length = 50)]
    public string OrderNum { get; set; } = default!;


    /// <summary>
    /// 预订单状态
    /// </summary>
    [SugarColumn(ColumnDescription = "预订单状态")]
    public BookingStateTypeEnum State { get; set; } = default!;

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
    /// 餐食类型
    /// </summary>
    [SugarColumn(ColumnDescription = "餐食类型")]
    public BreakfastTypeEnum BreakfastType { get; set; }

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
    public decimal SaleAmount { get; set; } 

    /// <summary>
    /// 成本金额
    /// </summary>
    [SugarColumn(ColumnDescription = "成本金额", ColumnDataType = "decimal(10,2)")]
    public decimal CostAmount { get; set; } 

    /// <summary>
    /// 酒店确认号
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店确认号", IsNullable = true, Length = 50)]
    public string HotelConfirmNum { get; set; } 

    /// <summary>
    /// 订房间数
    /// </summary>
    [SugarColumn(ColumnDescription = "订房间数")]
    public int RoomNumber { get; set; }

    /// <summary>
    /// 几晚
    /// </summary>
    [SugarColumn(ColumnDescription = "几晚")]
    public int HowManyNights { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 300)]

    public string Remark { get; set; } = default!;

}

public partial record OrderDo
{
    private OrderDo(string orderNum,  BookingStateTypeEnum state, 
        long userHotelId, string roomCode,  BreakfastTypeEnum breakfastType, DateTime checkInDate,
        DateTime checkOutDate, decimal saleAmount,decimal costAmount, string hotelConfirmNum, int roomNumber,
        int howManyNights,  string remark, string createdbyId,  long tenantId)
    {
        OrderNum = orderNum;
        State = state;
        UserHotelId = userHotelId;
        RoomCode = roomCode;
        BreakfastType = breakfastType;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        SaleAmount = saleAmount;
        CostAmount = costAmount;
        HotelConfirmNum = hotelConfirmNum;
        RoomNumber = roomNumber;
        HowManyNights = howManyNights;
        Remark = remark;
        this.CreatedbyId = createdbyId;
        this.CreatedbyName = "sys";
        this.TenantId = tenantId;
    }

    public static OrderDo Create(string orderNum, BookingStateTypeEnum state,  long userHotelId, string roomCode,  BreakfastTypeEnum breakfastType, DateTime checkInDate, DateTime checkOutDate, decimal saleAmount, decimal costAmount, string hotelConfirmNum, int roomNumber, int howManyNights,  string remark, string createdbyId,  long tenantId)
    {
        return new OrderDo(orderNum, state,  userHotelId, roomCode,  breakfastType, checkInDate, checkOutDate, saleAmount, costAmount, hotelConfirmNum, roomNumber, howManyNights,  remark, createdbyId,  tenantId);
    }
}