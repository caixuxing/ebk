namespace YueJia.Ebk.Domain.Hotel;



[SugarTable("RoomStock", "房间库存")]
public partial record RoomStockDo : EntityTenant
{
    public RoomStockDo() { }
    /// <summary>
    /// 酒店房间ID
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店房间ID", IsNullable = true)]
    public long HotelRoomId { get; private set; }

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

}

public partial record RoomStockDo
{
    private RoomStockDo(long hotelRoomId, DateTime currentDate, int stockNum)
    {
        HotelRoomId = hotelRoomId;
        CurrentDate = currentDate;
        StockNum = stockNum;
    }

    public static RoomStockDo Create(long hotelRoomId, DateTime currentDate, int stockNum)
    {
        return new RoomStockDo(hotelRoomId, currentDate, stockNum);
    }
}
