using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums;


/// <summary>
/// 预订状态类型
/// </summary>

public enum BookingStateTypeEnum
{
    /// <summary>
    /// 失败
    /// </summary>
    [Description("失败")]
    BookFail = 0,

    /// <summary>
    /// 成功
    /// </summary>
    [Description("成功")]
    BookConfirmed = 1,

    /// <summary>
    /// 成功
    /// </summary>
    [Description("成功")]
    BookInvalid = 1,
}
