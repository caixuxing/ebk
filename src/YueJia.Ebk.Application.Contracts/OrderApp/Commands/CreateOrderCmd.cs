namespace YueJia.Ebk.Application.Contracts.OrderApp.Commands;

/// <summary>
/// 创建订单命令
/// </summary>
public record CreateOrderCmd
{
    public string TrackId { get; set; }

    /// <summary>
    /// 订单号
    /// </summary>
    [Required]
    public string OrderCode { set; get; } = default!;
    /// <summary>
    /// 酒店code
    /// </summary>
    [Required]
    public string HotelCode { set; get; } = default!;
    /// <summary>
    /// Ota房型Code
    /// </summary>
    [Required]
    public string OtaRoomCode { set; get; } = default!;
    /// <summary>
    /// 入住日期[格式:yyyy-MM-dd]
    /// </summary>
    [Required]
    public string CheckInDate { set; get; } = default!;
    /// <summary>
    /// 离店日期[格式:yyyy-MM-dd]
    /// </summary>
    [Required]
    public string CheckOutDate { set; get; } = default!;
    /// <summary>
    /// 房间人员信息
    /// </summary>
    [Required]
    public List<HotelRoomModel> RoomList { set; get; } = new();


    /// <summary>
    /// 销售价格
    /// </summary>
    [Required]
    public decimal SalePrice { set; get; }

    /// <summary>
    /// 客人特殊要求
    /// </summary>
    public string? SpecialRemark { set; get; }

    /// <summary>
    /// 是否含早
    /// </summary>
    public bool IsBreakfast { set; get; }

    /// <summary>
    /// 查价唯一标识
    /// </summary>
    public string SearchCode { get; set; }



    /// <summary>
    /// 几晚
    /// </summary>
    public int NightNumber
    {
        get
        {
            return (Convert.ToDateTime(CheckOutDate) - Convert.ToDateTime(CheckInDate)).Days;
        }
    }

}

/// <summary>
/// 创建订单命令模型参数校验
/// </summary>
public class CreateOrderCmdValidator : AbstractValidator<CreateOrderCmd>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public CreateOrderCmdValidator()
    {
        RuleFor(x => x.OrderCode)
         .Cascade(CascadeMode.Stop)
         .NotEmpty().WithMessage("订单号不能为空！")
         .MaximumLength(50).WithMessage("订单号长度不能超过50个字符！");
        RuleFor(x => x.HotelCode)
         .Cascade(CascadeMode.Stop)
         .NotEmpty().WithMessage("酒店编码不能为空！")
         .MaximumLength(30).WithMessage("酒店编码长度不能超过30个字符！");
        RuleFor(x => x.OtaRoomCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("房型不能为空！")
            .MaximumLength(10).WithMessage("房型长度不能超过10个字符！");
 

        RuleFor(x => x.RoomList).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("房间集合不能为空！");
        RuleForEach(x => x.RoomList).SetValidator(new HotelRoomModelValidator());

   
        RuleFor(x => x.SalePrice)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("销售价格必须大于0！");

        RuleFor(x => x.SearchCode)
        .Cascade(CascadeMode.Stop)
        .NotEmpty().WithMessage("查价标识不能为空！");
    }
}

/// <summary>
/// 酒店房间对象
/// </summary>
public class HotelRoomModel
{
    /// <summary>
    /// 儿童数
    /// </summary>
    public int ChildNumber { get; set; }

    /// <summary>
    /// 成人数
    /// </summary>
    [Required]
    public int AdultNumber { get; set; }

    /// <summary>
    /// 入住人员年龄结构
    /// </summary>
    [Required]
    public List<PersonModel> PersonList { get; set; } = new();
}

/// <summary>
/// 酒店房间对象校验
/// </summary>
public class HotelRoomModelValidator : AbstractValidator<HotelRoomModel>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public HotelRoomModelValidator()
    {
        RuleFor(x => x.AdultNumber)
        .Cascade(CascadeMode.Stop)
        .GreaterThan(0).WithMessage("成人数必须大于0！");

        RuleFor(x => x.PersonList).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("入住人员年龄结构集合不能为空！");
        RuleForEach(x => x.PersonList).SetValidator(new PersonModelValidator());
    }
}

/// <summary>
/// 入住人信息
/// </summary>
public class PersonModel
{
    /// <summary>
    /// 房间序号 从第1间 开始
    /// </summary>
    [Required]
    public int RoomIndex { get; set; }

    /// <summary>
    ///名拼音
    /// </summary>
    [Required]
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// 姓拼音
    /// </summary>
    [Required]
    public string LastName { get; set; } = default!;

    /// <summary>
    /// 类型 (成人:1 儿童:2)
    /// </summary>
    [Required]
    public PersonTypeEnum Type { get; set; }

    /// <summary>
    /// 年龄(儿童年龄必须要)
    /// </summary>
    [Required]
    public int Age { get; set; }
}

/// <summary>
/// 酒店房间对象校验
/// </summary>
public class PersonModelValidator : AbstractValidator<PersonModel>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public PersonModelValidator()
    {
        RuleFor(x => x.RoomIndex)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("房间序号必须大于0！");

        RuleFor(x => x.FirstName)
         .Cascade(CascadeMode.Stop)
         .NotEmpty().WithMessage("名拼音不能为空！")
         .MaximumLength(50).WithMessage("名拼音长度不能超过50个字符！");

        RuleFor(x => x.LastName)
         .Cascade(CascadeMode.Stop)
         .NotEmpty().WithMessage("姓拼音不能为空！")
         .MaximumLength(50).WithMessage("姓拼音长度不能超过50个字符！");

        RuleFor(x => x.Type).IsInEnum().WithMessage("人类型参数无效！");

        When(x => x.Type == PersonTypeEnum.Adult, () =>
        {
            RuleFor(x => x.Age)
                .GreaterThan(17)
                .WithMessage("成人年龄必须大于17岁");
        });
        When(x => x.Type == PersonTypeEnum.Child, () =>
        {
            RuleFor(x => x.Age)
                .GreaterThan(0).WithMessage("儿童年龄必须大于0岁")
                .LessThan(17).WithMessage("儿童年龄必须小于17岁");
        });
    }
}