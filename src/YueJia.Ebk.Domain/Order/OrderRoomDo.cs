namespace YueJia.Ebk.Domain.Order;


/// <summary>
/// 订单表
/// </summary>
[SugarTable("OrderRoom", "订单房间表")]
public partial record OrderRoomDo : EntityBase
{
    public OrderRoomDo() { }
    /// <summary>
    /// 预订号
    /// </summary>
    [SugarColumn(ColumnDescription = "预订号", Length = 50)]
    public string OrderNum { get; set; } = default!;

    /// <summary>
    /// 房间名称
    /// </summary>
    [SugarColumn(ColumnDescription = "房间名称", Length = 200)]
    public string RoomName { get; set; } = default!;

    /// <summary>
    /// 房间代码
    /// </summary>
    [SugarColumn(ColumnDescription = "房间代码", Length = 20)]
    public string RoomCode { get; set; } = default!;

    /// <summary>
    /// 床型名称
    /// </summary>
    [SugarColumn(ColumnDescription = "床型名称", Length = 200)]
    public string BedName { get; set; } = default!;

    /// <summary>
    /// 床型代码
    /// </summary>
    [SugarColumn(ColumnDescription = "床型代码", Length = 20)]
    public string BedCode { get; set; } = default!;


    /// <summary>
    /// 价格计划名称
    /// </summary>
    [SugarColumn(ColumnDescription = "价格计划名称", Length = 200)]
    public string PricePlanName { get; set; } = default!;


    /// <summary>
    /// 价格计划ID
    /// </summary>
    [SugarColumn(ColumnDescription = "价格计划ID")]
    public long PricePlanId { get; set; } = default!;

    /// <summary>
    /// 早餐类型
    /// </summary>
    [SugarColumn(ColumnDescription = "早餐类型")]
    public BreakfastTypeEnum BreakfastType { get; set; } = default!;

}

public partial record OrderRoomDo
{


    private OrderRoomDo(string orderNum, string roomName, string roomCode, string bedName, string bedCode, string pricePlanName, long pricePlanId, BreakfastTypeEnum breakfastType)
    {
        OrderNum = orderNum;
        RoomName = roomName;
        RoomCode = roomCode;
        BedName = bedName;
        BedCode = bedCode;
        PricePlanName = pricePlanName;
        PricePlanId = pricePlanId;
        BreakfastType = breakfastType;
    }

    public static OrderRoomDo Create(string orderNum, string roomName, string roomCode, string bedName, string bedCode, string pricePlanName, long pricePlanId, BreakfastTypeEnum breakfastType)
    {
        return new OrderRoomDo(orderNum, roomName, roomCode, bedName, bedCode, pricePlanName, pricePlanId, breakfastType);
    }
}