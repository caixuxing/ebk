using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums;

/// <summary>
/// 酒店销售状态
/// </summary>
public enum HotelSaleTypeMnum
{
    /// <summary>
    /// 上架
    /// </summary>
    [Description("上架")]
    Up = 1,

    /// <summary>
    /// 下架
    /// </summary>
    [Description("下架")]
    Down = 2,

    /// <summary>
    /// 暂停
    /// </summary>
    [Description("暂停")]
    Stop = 3
}
