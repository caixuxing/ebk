using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums;


/// <summary>
/// 账号类型枚举
/// </summary>
[Description("账号类型枚举")]
public enum AccountTypeEnum
{
    /// <summary>
    /// 超级管理员
    /// </summary>
    [Description("超级管理员")]
    SuperAdmin = 999,

    /// <summary>
    /// 系统管理员
    /// </summary>
    [Description("系统管理员")]
    SysAdmin = 888,

    /// <summary>
    /// 普通账号
    /// </summary>
    [Description("普通账号")]
    NormalUser = 777
}
