using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums;

public enum ChannelType
{
    /// <summary>
    /// 携程
    /// </summary>
    [Description("携程")]
    Ctrip = 1,
    /// <summary>
    /// 去哪儿
    /// </summary>
    [Description("去哪儿")]
    WhereTo = 2,
    /// <summary>
    /// 飞猪
    /// </summary>
    [Description("飞猪")]
    Fliggy = 3,
    /// <summary>
    /// Agoda
    /// </summary>
    [Description("Agoda")]
    Agoda = 4,
    /// <summary>
    /// 途灵
    /// </summary>
    [Description("途灵")]
    Touring = 5
}
