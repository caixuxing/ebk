namespace YueJia.Ebk.Domain.Hotel;



/// <summary>
/// 每日库存
/// </summary>
[SugarTable("DailyInventory", "每日库存")]
public record DailyInventoryDo : EntityTenant
{

    /// <summary>
    /// 房间Id
    /// </summary>
    [SugarColumn(ColumnDescription = "房间Id")]
    public long RoomId { get; init; }

    /// <summary>
    /// 当前日期
    /// </summary>
    [SugarColumn(ColumnDescription = "当前日期", ColumnDataType = "date")]
    public DateTime CurrentDate { get; private set; }

    /// <summary>
    /// 当日库存数
    /// </summary>
    [SugarColumn(ColumnDescription = "当日库存数")]
    public int InventoryNum { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public YesOrNoType IsEnable { get; private set; }
}
