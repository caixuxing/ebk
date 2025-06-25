namespace YueJia.Ebk.Application.Contracts.OrderApp.Qry;

/// <summary>
/// 订单分页过滤条件模型参数
/// </summary>
public record OrderPageListFilterQry : BasePageQry
{
    /// <summary>
    /// 日期类型
    /// </summary>
    public int DateType { get; set; }

    public string StartDate { get; set; }
    public string EndDate { get; set; }

    /// <summary>
    /// 预订号
    /// </summary>
    public string OrderNum { get; set; }

    /// <summary>
    /// 酒店名称
    /// </summary>

    public string HotelName { get; set; }

    public string UserId { get; set; }


    /// <summary>
    /// 酒店编码
    /// </summary>
    public string HotelCode { get; set; }


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
