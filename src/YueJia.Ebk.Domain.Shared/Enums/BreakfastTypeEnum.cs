using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums;

public enum BreakfastTypeEnum
{
    /// <summary>
    /// 含早
    /// </summary>
    [Description("含早")]
    Breakfast = 1,

    /// <summary>
    /// 无早
    /// </summary>
    [Description("无早")]
    NoBreakfast = 2,

}
