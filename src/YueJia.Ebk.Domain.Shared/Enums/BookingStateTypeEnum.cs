using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums;


/// <summary>
/// 预订状态类型
/// </summary>

public enum BookingStateTypeEnum
{

    /// <summary>
    /// 待确认
    /// </summary>
    [Description("待确认")]
    ToBeConfirmed = 0,

    /// <summary>
    /// 待处理
    /// </summary>
    [Description("待处理")]
    Pending = 1,

    /// <summary>
    /// 已确认
    /// </summary>
    [Description("已确认")]
    Confirmed = 2,

    /// <summary>
    /// 已取消
    /// </summary>
    [Description("已取消")]
    Canceled = 3,

    /// <summary>
    /// 已拒单
    /// </summary>
    [Description("已拒单")]
    Rejected = 4

}
