namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands;

/// <summary>
/// 创建或更新价格计划参数
/// </summary>
public record CreateOrUpdatePricePlanCmd
{


    /// <summary>
    /// 酒店房间ID
    /// </summary>
    public string HotelRoomId { get; set; } = default!;
    /// <summary>
    /// 早餐类型
    /// </summary>
    public BreakfastTypeEnum BreakfastType { get; set; }
    /// <summary>
    /// 提前天数
    /// </summary>
    public int DaysInAdvance { get; set; }

    /// <summary>
    /// 连住天数
    /// </summary>
    public int ContinuousStayDays { get; set; }

    /// <summary>
    /// 是否保留房
    /// </summary>
    public YesOrNoType IsReservedRoom { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public YesOrNoType IsEnable { get; set; }


    /// <summary>
    /// 星期一
    /// </summary>
    public decimal Monday { get; set; }
    /// <summary>
    /// 星期二
    /// </summary>
    public decimal Tuesday { get; set; }
    /// <summary>
    /// 星期三
    /// </summary>
    public decimal Wednesday { get; set; }
    /// <summary>
    /// 星期四
    /// </summary>
    public decimal Thursday { get; set; }
    /// <summary>
    /// 星期五
    /// </summary>
    public decimal Friday { get; set; }
    /// <summary>
    /// 星期六
    /// </summary>
    public decimal Saturday { get; set; }
    /// <summary>
    /// 星期天
    /// </summary>
    public decimal Sunday { get; set; }
}

/// <summary>
/// 创建或更新价格计划参数校验
/// </summary>
public class CreateOrUpdatePricePlanCmdValidator : AbstractValidator<CreateOrUpdatePricePlanCmd>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public CreateOrUpdatePricePlanCmdValidator()
    {
        RuleFor(x => x.HotelRoomId).NotEmpty().WithMessage("酒店房间ID不能为空！");
        RuleFor(x => x.BreakfastType).IsInEnum().WithMessage("早餐类型参数无效！");
        RuleFor(x => x.DaysInAdvance).GreaterThan(0).WithMessage("提前天数必须大于0！");
        RuleFor(x => x.ContinuousStayDays).GreaterThan(0).WithMessage("连住天数必须大于0！");
        RuleFor(x => x.IsReservedRoom).IsInEnum().WithMessage("是否保留房参数无效！");
        RuleFor(x => x.IsEnable).IsInEnum().WithMessage("是否启用参数无效！");
    }
}
