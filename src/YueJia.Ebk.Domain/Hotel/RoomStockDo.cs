namespace YueJia.Ebk.Domain.Hotel;



[SugarTable("RoomStock", "房间库存")]
public partial record RoomStockDo : EntityTenant
{
    /// <summary>
    /// 酒店房间ID
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店房间ID", IsNullable = true)]
    public long HotelRoomId { get; set; }

    /// <summary>
    /// 当前日期
    /// </summary>
    [SugarColumn(ColumnDescription = "当前日期")]
    public DateTime CurrentDate { get; set; }

    /// <summary>
    /// 库存
    /// </summary>
    [SugarColumn(ColumnDescription = "库存")]
    public int StockNum { get; set; }

}
