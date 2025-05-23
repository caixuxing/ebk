namespace YueJia.Ebk.Domain.Hotel;

/// <summary>
/// 酒店发布表
/// </summary>
[SugarTable("HotelPublish", "酒店发布表")]
public partial record HotelPublishDo : EntityTenant
{
    public HotelPublishDo() { }
    /// <summary>
    /// 酒店编码
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店编码", Length = 30)]
    public string HotelCode { get; set; } = default!;
    /// <summary>
    /// 酒店名称
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店名称", Length = 100)]
    public string HotelName { get; set; } = default!;
    /// <summary>
    /// 酒店名称（英文）  
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店名称(英文)", Length = 200)]
    public string HotelNameEn { get; set; } = default!;
    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public HotelSaleTypeMnum Status { get; set; }
    /// <summary>
    /// 酒店地址
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店地址", Length = 200)]
    public string Address { get; set; } = default!;
    /// <summary>
    /// 酒店地址（英文)
    /// </summary>
    [SugarColumn(ColumnDescription = "酒店地址(英文)", Length = 300)]
    public string AddressEn { get; set; } = default!;

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "联系电话", Length = 15)]
    public string TelPhone { get; set; } = default!;
    /// <summary>
    /// 最低价
    /// </summary>
    [SugarColumn(ColumnDescription = "最低价", ColumnDataType = "decimal(18,2)")]
    public decimal LowestPrice { get; set; }
}

public partial record HotelPublishDo
{
    private HotelPublishDo(string hotelCode, string hotelName, string hotelNameEn, HotelSaleTypeMnum status, string address, string addressEn, string telPhone, decimal lowestPrice)
    {
        HotelCode = hotelCode;
        HotelName = hotelName;
        HotelNameEn = hotelNameEn;
        Status = status;
        Address = address;
        AddressEn = addressEn;
        TelPhone = telPhone;
        LowestPrice = lowestPrice;
    }


    public static HotelPublishDo Create(string hotelCode, string hotelName, string hotelNameEn, HotelSaleTypeMnum status, string address, string addressEn, string telPhone, decimal lowestPrice)
    {
        return new HotelPublishDo(hotelCode, hotelName, hotelNameEn, status, address, addressEn, telPhone, lowestPrice);
    }

    public HotelPublishDo SetHotelCode(string hotelCode)
    {
        HotelCode = hotelCode;
        return this;
    }

    public HotelPublishDo SetHotelName(string hotelName)
    {
        HotelName = hotelName;
        return this;
    }

    public HotelPublishDo SetHotelNameEn(string hotelNameEn)
    {
        HotelNameEn = hotelNameEn;
        return this;
    }

    public HotelPublishDo SetStatus(HotelSaleTypeMnum status)
    {
        Status = status;
        return this;
    }

    public HotelPublishDo SetAddress(string address)
    {
        Address = address;
        return this;
    }

    public HotelPublishDo SetAddressEn(string addressEn)
    {
        AddressEn = addressEn;
        return this;
    }

    public HotelPublishDo SetTelPhone(string telPhone)
    {
        TelPhone = telPhone;
        return this;
    }

    public HotelPublishDo SetLowestPrice(decimal lowestPrice)
    {
        LowestPrice = lowestPrice;
        return this;
    }

}
