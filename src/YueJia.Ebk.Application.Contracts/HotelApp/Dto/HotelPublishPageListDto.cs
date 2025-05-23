namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;

public record HotelPublishPageListDto
{
    /// <summary>
    /// ID
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long Id { get; set; }

    /// <summary>
    /// 酒店编码
    /// </summary>
    public string HotelCode { get; set; } = default!;
    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;
    /// <summary>
    /// 酒店名称（英文）  
    /// </summary>
    public string HotelNameEn { get; set; } = default!;
    /// <summary>
    /// 状态
    /// </summary>
    public HotelSaleTypeMnum Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName
    {
        get
        {
            return Status.ToDescription();
        }
        private set { }
    }


    /// <summary>
    /// 酒店地址
    /// </summary>
    public string Address { get; set; } = default!;
    /// <summary>
    /// 酒店地址（英文)
    /// </summary>
    public string AddressEn { get; set; } = default!;

    /// <summary>
    /// 联系电话
    /// </summary>
    public string TelPhone { get; set; } = default!;
    /// <summary>
    /// 最低价
    /// </summary>
    public decimal LowestPrice { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
