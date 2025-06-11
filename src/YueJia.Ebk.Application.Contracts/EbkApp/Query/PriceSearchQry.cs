namespace YueJia.Ebk.Application.Contracts.EbkApp.Query;

/// <summary>
/// 查价检索模型
/// </summary>
public record PriceSearchQry
{
    /// <summary>
    /// 酒店Code
    /// </summary>
    [Required]
    public string HotelCode { get; set; } = default!;
    /// <summary>
    /// 入店日期
    /// </summary>
    [Required]
    public DateTime CheckInDate { get; set; } = DateTime.Now.Date;
    /// <summary>
    /// 离店日期
    /// </summary>
    [Required]
    public DateTime CheckOutDate { get; set; } = DateTime.Now.AddDays(1);

    /// <summary>
    /// 成人数量
    /// </summary>
    [Required]
    public int AdultNum { get; set; } = 1;
    /// <summary>
    /// 儿童数量
    /// </summary>
    [Required]
    public int ChildNum { get; set; } = 0;
    /// <summary>
    /// 房间数量
    /// </summary>
    [Required]
    public int RoomNum { get; set; } = 1;
}
/// <summary>
/// 创建公司命令参数校验
/// </summary>
public class PriceSearchQryValidator : AbstractValidator<PriceSearchQry>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public PriceSearchQryValidator()
    {

        RuleFor(x => x.HotelCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("酒店代码不能为空！");

        RuleFor(x => x.CheckInDate)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("入店日期不能为空！")
            .Must((x) => x != default(DateTime)).WithMessage("入店日期格式错误！");

        RuleFor(x => x.CheckOutDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("离店日期不能为空！")
            .Must((x) => x != default(DateTime)).WithMessage("离店日期格式错误！")
            .GreaterThan(x => x.CheckInDate).WithMessage("离店日期必须晚于入店日期！");
        RuleFor(x => x.AdultNum)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("成人数最小值为1！");

        RuleFor(x => x.RoomNum)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("房间数最小值为1！");
    }


}
