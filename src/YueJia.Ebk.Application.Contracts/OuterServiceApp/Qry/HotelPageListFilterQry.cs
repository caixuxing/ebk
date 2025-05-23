namespace YueJia.Ebk.Application.Contracts.OuterServiceApp.Qry;



/// <summary>
/// 酒店页面列表过滤查询模型
/// </summary>
public record HotelPageListFilterQry : BasePageQry
{
    /// <summary>
    /// 酒店编码
    /// </summary>
    public string? HotelCode { get; set; }
    /// <summary>
    /// 酒店名称
    /// </summary>
    public string? HotelName { get; set; }
    /// <summary>
    /// 国家ID
    /// </summary>
    public int? CountryId { get; set; }

}
