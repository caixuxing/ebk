using static YueJia.Ebk.Application.Contracts.HotelApp.Commands.PriceCmd;
using static YueJia.Ebk.Application.Contracts.HotelApp.Commands.RoomInfoCmd;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands;

/// <summary>
/// 保存加载库存和价格命令
/// </summary>
public class SaveLoadingInventoryAndPricesCmd
{
    /// <summary>
    /// 酒店ID
    /// </summary>

    public string HotelId { get; set; } = default!;

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 房间信息
    /// </summary>
    public List<RoomInfoCmd> Rooms { get; set; } = new();


    /// <summary>
    /// 保存加载库存和价格模型参数校验
    /// </summary>
    public class SaveLoadingInventoryAndPricesCmdValidator : AbstractValidator<SaveLoadingInventoryAndPricesCmd>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SaveLoadingInventoryAndPricesCmdValidator()
        {
            RuleFor(x => x.HotelId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("酒店ID不能为空！");
            RuleFor(x => x.EndDate).Cascade(CascadeMode.Stop).NotNull().WithMessage("截至日期不能为空！");
            RuleFor(x => x.Rooms).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("房间信息不能为空！");
            RuleForEach(x => x.Rooms).SetValidator(new RoomInfoCmdValidator());


        }
    }

}

/// <summary>
/// 房间信息命令
/// </summary>
public class RoomInfoCmd
{
    /// <summary>
    /// 房间ID
    /// </summary>
    public string RoomId { get; set; } = default!;

    /// <summary>
    /// 库存（周几，库存数）
    /// </summary>
    public Dictionary<DayOfWeek, int?> Inventory { get; set; } = new();

    /// <summary>
    /// 价格明细
    /// </summary>
    public List<PriceCmd> Prices { get; set; } = new();


    /// <summary>
    /// 保存加载库存和价格模型参数校验
    /// </summary>
    public class RoomInfoCmdValidator : AbstractValidator<RoomInfoCmd>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RoomInfoCmdValidator()
        {
            RuleFor(x => x.RoomId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("房间ID不能为空！");
            //RuleFor(x => x.Inventory).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("库存集合不能为空！");
            RuleFor(x => x.Inventory)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("库存集合不能为空！")
            .Must(dict => dict != null && dict.Values.All(price => price.HasValue))
            .WithMessage((modes, dict) =>
            {
                var emptyDays = dict?.Where(kvp => !kvp.Value.HasValue)
                .Select(kvp => "周" + "日一二三四五六"[(int)kvp.Key])
                .ToList();
                return $"每周库存：{string.Join("、", emptyDays ?? new())}不能为空！";

            });


            RuleFor(x => x.Prices).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("价格计划集合不能为空！");
            RuleForEach(x => x.Prices).SetValidator(new PriceCmdValidator());


        }
    }
}

/// <summary>
/// 价格明细命令
/// </summary>
public class PriceCmd
{
    /// <summary>
    /// 价格计划ID
    /// </summary>
    public string PricePlanId { get; set; } = default!;

    /// <summary>
    /// （周几，价格）价格明细
    /// </summary>
    public Dictionary<DayOfWeek, decimal?> DailyPrices { get; set; } = new();


    /// <summary>
    /// 保存加载库存和价格模型参数校验
    /// </summary>
    public class PriceCmdValidator : AbstractValidator<PriceCmd>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PriceCmdValidator()
        {
            RuleFor(x => x.PricePlanId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("价格计划ID不能为空！");
            RuleFor(x => x.DailyPrices)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .WithMessage("价格集合不能为空！")
                        .Must(dict => dict != null && dict.Values.All(price => price.HasValue))
                        .WithMessage((modes, dict) =>
                        {
                            var emptyDays = dict?.Where(kvp => !kvp.Value.HasValue)
                            .Select(kvp => "周" + "日一二三四五六"[(int)kvp.Key])
                            .ToList();
                            return $"每日价格{string.Join("、", emptyDays ?? new())}不能为空！";

                        });
        }
    }
}
