namespace YueJia.Ebk.Application.Contracts.HotelApp.Query;

public record HotelPublishPageFilterQry : BasePageQry
{
    /// <summary>
    /// 酒店编码
    /// </summary>
    public string? HotelCode { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 酒店名称Or英文名称
    /// </summary>
    public string? HotelName { get; set; }
    /// <summary>
    /// 状态
    /// </summary>
    public HotelSaleTypeEnum? Status { get; set; }

    /// <summary>
    /// 国家 Ios
    /// </summary>
    public int? countryId { get; set; }

    /// <summary>
    /// 城市名称
    /// </summary>
    public string cityName { get; set; }
}
