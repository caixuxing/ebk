using System.ComponentModel;
namespace YueJia.Ebk.Domain.Shared.Enums;

/// <summary>
/// 人-类型枚举
/// </summary>
public enum PersonTypeEnum
{

    /// <summary>
    /// 成人
    /// </summary>
    [Description("成人")]
    Adult = 1,

    /// <summary>
    /// 儿童
    /// </summary>
    [Description("儿童")]
    Child = 2

}
