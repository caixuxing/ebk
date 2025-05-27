namespace YueJia.Ebk.Domain.Hotel;


/// <summary>
/// 酒店房间
/// </summary>
[SugarTable("HotelRoom", "酒店房间")]
public partial record HotelRoomDo : EntityTenant
{
    /// <summary>
    /// 酒店代码
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店编码", Length = 30)]
    public string HotelCode { get; set; } = default!;

    /// <summary>
    /// 房型
    /// </summary>
    [SugarColumn(ColumnDescription = "房型", Length = 10)]
    public string RoomType { get; set; } = default!;
    /// <summary>
    /// 床型
    /// </summary>
    [SugarColumn(ColumnDescription = "床型", Length = 10)]
    public string BedType { get; set; } = default!;

    /// <summary>
    /// 人数上限
    /// </summary>
    [SugarColumn(ColumnDescription = "人数上限")]
    public int MaximumNumberOfPeople { get; set; }
    /// <summary>
    /// 成人上限
    /// </summary>
    [SugarColumn(ColumnDescription = "成人上限", IsNullable = true)]
    public int? AdultLimit { get; set; }

    /// <summary>
    /// 儿童上限
    /// </summary>
    [SugarColumn(ColumnDescription = "儿童上限", IsNullable = true)]
    public int? ChildLimit { get; set; }


    /// <summary>
    /// 开始日期
    /// </summary>
    [SugarColumn(ColumnDescription = "开始日期")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    [SugarColumn(ColumnDescription = "开始日期")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 库存初始值（Json）
    /// </summary>
    [SugarColumn(ColumnDescription = "库存初始值（Json）", Length = 200, IsNullable = true)]
    public string? StockInitValJosn { get; set; } = default!;

    /// <summary>
    /// 是否启用
    /// </summary>
    [SugarColumn(ColumnDescription = "是否启用")]
    public YesOrNoType IsEnabled { get; set; }
}
