using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums
{
    /// <summary>
    /// 推送类型
    /// </summary>
    public enum PushTypeEnum
    {
        /// <summary>
        /// 邮件
        /// </summary>
        [Description("邮件")]
        Email = 1,

        /// <summary>
        /// 短信
        /// </summary>
        [Description("短信")]
        SMS = 2, // 短信推送

        /// <summary>
        /// 钉钉
        /// </summary>
        [Description("钉钉")]
        DingTalk = 3

    }
}
