namespace YueJia.Ebk.Domain.Hotel;


/// <summary>
/// 酒店房间
/// </summary>
[SugarTable("HotelRoom", "酒店房间")]
public partial record HotelRoomDo : EntityTenant
{
    public HotelRoomDo() { }

    /// <summary>
    /// 酒店ID
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店ID", DefaultValue = "0")]
    public long HotelId { get; init; }

    /// <summary>
    /// 酒店代码
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店编码", Length = 30)]
    public string HotelCode { get; init; } = default!;

    /// <summary>
    /// 房型
    /// </summary>
    [SugarColumn(ColumnDescription = "房型", Length = 10)]
    public string RoomType { get; private set; } = default!;
    /// <summary>
    /// 床型
    /// </summary>
    [SugarColumn(ColumnDescription = "床型", Length = 10)]
    public BedTypeEnum BedType { get; private set; } = default!;

    /// <summary>
    /// 人数上限
    /// </summary>
    [SugarColumn(ColumnDescription = "人数上限")]
    public int MaximumNumberOfPeople { get; private set; }
    /// <summary>
    /// 成人上限
    /// </summary>
    [SugarColumn(ColumnDescription = "成人上限", IsNullable = true)]
    public int? AdultLimit { get; private set; }

    /// <summary>
    /// 儿童上限
    /// </summary>
    [SugarColumn(ColumnDescription = "儿童上限", IsNullable = true)]
    public int? ChildLimit { get; private set; }


    /// <summary>
    /// 开始日期
    /// </summary>
    [SugarColumn(ColumnDescription = "开始日期")]
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    [SugarColumn(ColumnDescription = "开始日期")]
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// 库存初始值（Json）
    /// </summary>
    [SugarColumn(ColumnDescription = "库存初始值（Json）", Length = 200, IsNullable = true)]
    public string? StockInitValJosn { get; private set; } = default!;

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public YesOrNoType IsEnabled { get; private set; }
}


public partial record HotelRoomDo
{


    private HotelRoomDo(long id, long hotelId, string hotelCode, string roomType, BedTypeEnum bedType, int maximumNumberOfPeople, int? adultLimit, int? childLimit, DateTime startDate, DateTime endDate, string? stockInitValJosn, YesOrNoType isEnabled)
    {
        this.Id = id;
        HotelId = hotelId;
        HotelCode = hotelCode;
        RoomType = roomType;
        BedType = bedType;
        MaximumNumberOfPeople = maximumNumberOfPeople;
        AdultLimit = adultLimit;
        ChildLimit = childLimit;
        StartDate = startDate;
        EndDate = endDate;
        StockInitValJosn = stockInitValJosn;
        IsEnabled = isEnabled;
    }

    public static HotelRoomDo Create(long hotelId, string hotelCode, string roomType, BedTypeEnum bedType, int maximumNumberOfPeople, int? adultLimit, int? childLimit, DateTime startDate, DateTime endDate, string? stockInitValJosn)
    {
        return new HotelRoomDo(SnowFlakeSingle.instance.getID(), hotelId, hotelCode, roomType, bedType, maximumNumberOfPeople, adultLimit, childLimit, startDate, endDate, stockInitValJosn, YesOrNoType.Yes);
    }

    public HotelRoomDo SetIsEnabled(YesOrNoType isEnabled)
    {
        this.IsEnabled = isEnabled;
        return this;
    }


}
