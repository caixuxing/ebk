namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;

/// <summary>
/// 查价结果Dto
/// </summary>
public record HotelPriceDto
{

    public HotelPriceDto(int _RoomNumber) {
        RoomNumber = _RoomNumber;
    }
    /// <summary>
    /// 酒店代码
    /// </summary>
    public string HotelCode { get; set; } = default!;

    /// <summary>
    /// 查价唯一标识
    /// </summary>
    public string SearchCode { get; set; } = default!;

    /// <summary>
    /// 房间代码
    /// </summary>
    public string RoomCode { get; set; } = default!;



    /// <summary>
    /// 餐食
    /// </summary>
    public bool IsBreakfast { get; set; } = default!;


    /// <summary>
    /// 总价
    /// </summary>
    public int TotalPrice
    {
        get {
            return DayPrice.Sum(dayPrice => dayPrice) * RoomNumber;
        }
    }

    /// <summary>
    /// 每日价格集合
    /// </summary>
    public List<int> DayPrice { get; set; } = new();

     int RoomNumber { get; set; }

}
