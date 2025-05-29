using System.Text.Json;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands;

/// <summary>
/// 床间酒店房间模型参数
/// </summary>
public class CreateHotelRoomCmd
{
    /// <summary>
    /// 酒店代码
    /// </summary>
    public string HotelCode { get; set; } = default!;
    /// <summary>
    /// 房型
    /// </summary>
    public string RoomType { get; set; } = default!;

    /// <summary>
    /// 床型
    /// </summary>
    public BedTypeEnum BedType { get; set; }

    /// <summary>
    /// 人数上限
    /// </summary>
    public int MaximumNumberOfPeople { get; set; }

    /// <summary>
    /// 成人上限
    /// </summary>
    public int? AdultLimit { get; set; }
    /// <summary>
    /// 儿童上限
    /// </summary>

    public int? ChildLimit { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime EndDate { get; set; }
    /// <summary>
    /// 库存初始值Json
    /// </summary>
    public string StockInitValJosn
    {
        get
        {
            return JsonSerializer.Serialize(Stock, new JsonSerializerOptions
            {
                //WriteIndented = true, // 格式化输出
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
    /// <summary>
    /// 库存初始值
    /// </summary>
    public Dictionary<DayOfWeek, int> Stock { get; set; } = default!;
}


/// <summary>
/// 床间酒店房间模型参数校验
/// </summary>
public class CreateHotelRoomCmdValidator : AbstractValidator<CreateHotelRoomCmd>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public CreateHotelRoomCmdValidator()
    {
        RuleFor(x => x.HotelCode)
         .Cascade(CascadeMode.Stop)
         .NotEmpty().WithMessage("酒店编码不能为空！")
         .MaximumLength(30).WithMessage("酒店编码长度不能超过30个字符！");
        RuleFor(x => x.RoomType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("房型不能为空！")
            .MaximumLength(10).WithMessage("房型长度不能超过10个字符！");
        RuleFor(x => x.MaximumNumberOfPeople)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("人数上限必须大于0！");
        RuleFor(x => x.AdultLimit)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("成人上限必须大于0！");


        RuleFor(x => x.StartDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("开始日期不能为空！")
            .LessThanOrEqualTo(x => x.EndDate).WithMessage("开始日期必须小于等于结束日期！");


        RuleFor(x => x.EndDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("结束日期不能为空！")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("结束日期必须大于等于开始日期！");


        RuleFor(x => x.Stock)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("库存初始值不能为空！");


        RuleFor(x => x.BedType).IsInEnum().WithMessage("状态参数无效！");

    }


}
