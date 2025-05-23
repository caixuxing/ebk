namespace YueJia.Ebk.Application.Contracts.OuterServiceApp.Dto;


/// <summary>
/// 
/// </summary>
public record HotelPageListDto
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 酒店名称
    /// </summary>
    public string? HotelName { get; set; }
    /// <summary>
    /// 酒店名称（英文）
    /// </summary>
    public string? HotelNameEn { get; set; }

    public string HotelCode { get; set; } = default!;

    /// <summary>
    /// 星级
    /// </summary>
    public int StarLevel { get; set; }

    /// <summary>
    /// 国家/地区
    /// </summary>
    public string? CountryName { get; set; }

    public string? CountryNameEn { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? AreaName { get; set; }

    public string? AreaNameEn { get; set; }
    /// <summary>
    /// 地址
    /// </summary>

    public string? Address { get; set; }

    /// <summary>
    /// 地址（英文）
    /// </summary>
    public string? AddressEn { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string? TelPhone { get; set; }
}
