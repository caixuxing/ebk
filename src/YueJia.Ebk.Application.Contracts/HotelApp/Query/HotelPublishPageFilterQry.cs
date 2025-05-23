namespace YueJia.Ebk.Application.Contracts.HotelApp.Query;

public record HotelPublishPageFilterQry : BasePageQry
{
    /// <summary>
    /// 酒店编码
    /// </summary>
    public string? HotelCode { get; set; }
    /// <summary>
    /// 酒店名称Or英文名称
    /// </summary>
    public string? HotelName { get; set; }
    /// <summary>
    /// 状态
    /// </summary>
    public HotelSaleTypeMnum? Status { get; set; }
}
