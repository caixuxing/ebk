namespace YueJia.Ebk.Domain.Hotel;



[SugarTable("RoomInventory", "房间库存")]
public partial record RoomInventoryDo : EntityTenant
{
    public RoomInventoryDo() { }
    /// <summary>
    /// 酒店房间ID
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店房间ID")]
    public long HotelRoomId { get; private set; }

    /// <summary>
    /// 价格计划ID
    /// </summary>
    [SugarColumn(ColumnDescription = "价格计划ID")]
    public long PricePlanId { get; private set; }

    /// <summary>
    /// 当前日期
    /// </summary>
    [SugarColumn(ColumnDescription = "当前日期")]
    public DateTime CurrentDate { get; private set; }

    /// <summary>
    /// 库存
    /// </summary>
    [SugarColumn(ColumnDescription = "库存")]
    public int StockNum { get; private set; }


    /// <summary>
    /// 价格
    /// </summary>
    [SugarColumn(ColumnDescription = "库存", ColumnDataType = "decimal(8,2)")]
    public decimal Price { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public YesOrNoType IsEnabled { get; set; }

}

public partial record RoomInventoryDo
{
    private RoomInventoryDo(long hotelRoomId, DateTime currentDate, int stockNum)
    {
        HotelRoomId = hotelRoomId;
        CurrentDate = currentDate;
        StockNum = stockNum;
    }

    public static RoomInventoryDo Create(long hotelRoomId, DateTime currentDate, int stockNum)
    {
        return new RoomInventoryDo(hotelRoomId, currentDate, stockNum);
    }
}
