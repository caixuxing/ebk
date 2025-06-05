namespace YueJia.Ebk.Domain.Hotel;



/// <summary>
/// 每日库存
/// </summary>
[SugarTable("DailyInventory", "每日库存")]
public partial record DailyInventoryDo : EntityTenant
{
    public DailyInventoryDo() { }

    /// <summary>
    /// 房间Id
    /// </summary>
    [SugarColumn(ColumnDescription = "房间Id")]
    public long RoomId { get; init; }

    /// <summary>
    /// 当前日期
    /// </summary>
    [SugarColumn(ColumnDescription = "当前日期", ColumnDataType = "date")]
    public DateTime CurrentDate { get; init; }

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

public partial record DailyInventoryDo
{

    private DailyInventoryDo(long roomId, DateTime currentDate, int inventoryNum, YesOrNoType isEnable)
    {
        RoomId = roomId;
        CurrentDate = currentDate;
        InventoryNum = inventoryNum;
        IsEnable = isEnable;
    }

    public static DailyInventoryDo Create(long roomId, DateTime currentDate, int inventoryNum, YesOrNoType isEnable)
    {
        return new DailyInventoryDo(roomId, currentDate, inventoryNum, isEnable);
    }

    public DailyInventoryDo SetInventoryNum(int inventoryNum)
    {
        InventoryNum = inventoryNum;
        return this;
    }
    public DailyInventoryDo SetIsEnable(YesOrNoType isEnable)
    {

        IsEnable = isEnable;
        return this;
    }
}
