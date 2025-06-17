namespace YueJia.Ebk.Application.Contracts.HotelApp.Query;


/// <summary>
/// 验价检查Qry
/// </summary>
public record PriceCheckQry : PriceSearchQry
{
    /// <summary>
    /// 查价唯一标识
    /// </summary>
    public string SearchCode { get; set; } = default!;
}


/// <summary>
/// 创建公司命令参数校验
/// </summary>
public class PriceCheckQryValidator : AbstractValidator<PriceCheckQry>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public PriceCheckQryValidator()
    {

        Include(new PriceSearchQryValidator());

        RuleFor(x => x.SearchCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("查价唯一标识不能为空！");

    }
}
