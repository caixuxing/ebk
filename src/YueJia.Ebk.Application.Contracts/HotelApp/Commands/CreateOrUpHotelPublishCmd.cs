namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands;



/// <summary>
/// 创建Or更新酒店发布命令模型
/// </summary>
public class CreateOrUpHotelPublishCmd
{
    /// <summary>
    /// 酒店编号
    /// </summary>
    public string HotelCode { get; set; } = default!;
    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } = default!;

    /// <summary>
    /// 酒店名称(英文)
    /// </summary>
    public string HotelNameEn { get; set; } = default!;

    /// <summary>
    /// 销售状态
    /// </summary>
    public HotelSaleTypeMnum Status { get; set; } = HotelSaleTypeMnum.Stop;

    /// <summary>
    /// 酒店地址
    /// </summary>
    public string Address { get; set; } = default!;

    /// <summary>
    /// 酒店地址（英文）
    /// </summary>
    public string AddressEn { get; set; } = default!;

    /// <summary>
    /// 联系电话
    /// </summary>
    public string TelPhone { get; set; } = default!;

    /// <summary>
    /// 最低价
    /// </summary>
    public decimal LowestPrice { get; set; } = 0;


}

/// <summary>
/// 创建公司命令参数校验
/// </summary>
public class CreateOrUpHotelPublishCmdValidator : AbstractValidator<CreateOrUpHotelPublishCmd>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public CreateOrUpHotelPublishCmdValidator()
    {

        RuleFor(x => x.Status).IsInEnum().WithMessage("状态参数无效！");

    }
}