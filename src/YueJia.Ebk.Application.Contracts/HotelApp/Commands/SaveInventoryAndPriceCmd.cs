using static YueJia.Ebk.Application.Contracts.HotelApp.Commands.InventoryOb;
using static YueJia.Ebk.Application.Contracts.HotelApp.Commands.PriceOb;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands;

/// <summary>
/// 保存库存和价格模型
/// </summary>
public record SaveInventoryAndPriceCmd
{
    /// <summary>
    /// 库存
    /// </summary>
    [Required]
    public List<InventoryOb> Inventorys { get; set; } = new();
    /// <summary>
    /// 价格列表
    /// </summary>
    [Required]
    public List<PriceOb> Prices { get; set; } = new();



    /// <summary>
    /// 保存库存和价格模型参数校验
    /// </summary>
    public class SaveInventoryAndPriceCmdValidator : AbstractValidator<SaveInventoryAndPriceCmd>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SaveInventoryAndPriceCmdValidator()
        {
            RuleFor(x => x.Inventorys).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("库存集合不能为空！");
            RuleForEach(x => x.Inventorys).SetValidator(new InventoryObValidator());


            RuleFor(x => x.Prices).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("价格集合不能为空！");
            RuleForEach(x => x.Prices).SetValidator(new PriceObValidator());


        }
    }
}
/// <summary>
/// 库存
/// </summary>
public record InventoryOb
{
    /// <summary>
    /// 库存
    /// </summary>
    [Required]
    public string InventoryId { get; set; } = default!;
    /// <summary>
    /// 库存数量
    /// </summary>
    [Required]
    public int InventoryNum { get; set; }
    /// <summary>
    /// 是否启用
    /// </summary>
    [Required]
    public YesOrNoType Status { get; set; }


    /// <summary>
    /// 库存模型参数校验
    /// </summary>
    public class InventoryObValidator : AbstractValidator<InventoryOb>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public InventoryObValidator()
        {
            RuleFor(item => item.InventoryId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("库存ID不能为空！").MaximumLength(20).WithMessage("库存ID长度不能超过20个字符！");
            RuleFor(item => item.InventoryNum).Cascade(CascadeMode.Stop).NotNull().WithMessage("库存数量不能为空！").GreaterThanOrEqualTo(0).WithMessage("库存数量不能小于0！");
            RuleFor(x => x.Status).Cascade(CascadeMode.Stop).IsInEnum().WithMessage("库存状态参数无效！");

        }
    }
}

/// <summary>
/// 价格
/// </summary>
public record PriceOb
{
    /// <summary>
    /// 价格ID
    /// </summary>
    [Required]
    public string PriceId { get; set; } = default!;
    /// <summary>
    /// 价格
    /// </summary>
    [Required]
    public decimal Price { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [Required]
    public YesOrNoType Status { get; set; }

    /// <summary>
    /// 库存模型参数校验
    /// </summary>
    public class PriceObValidator : AbstractValidator<PriceOb>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PriceObValidator()
        {
            RuleFor(item => item.PriceId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("价格ID不能为空!").MaximumLength(20).WithMessage("价格ID长度不能超过20个字符!");
            RuleFor(item => item.Price).Cascade(CascadeMode.Stop).NotNull().WithMessage("价格不能为空！").GreaterThanOrEqualTo(0).WithMessage("价格必须小于0！");
            RuleFor(x => x.Status).Cascade(CascadeMode.Stop).IsInEnum().WithMessage("价格状态参数无效!");

        }
    }
}