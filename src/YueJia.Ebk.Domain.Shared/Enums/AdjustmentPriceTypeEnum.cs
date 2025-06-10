using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums;

public enum AdjustmentPriceTypeEnum
{
    /// <summary>
    /// 固定值上调
    /// </summary>
    [Description("固定值上调")]
    FixedValueIncrease = 1,
    /// <summary>
    /// 百分比上调
    /// </summary>
    [Description("百分比上调")]
    PercentageIncrease = 2,
}
