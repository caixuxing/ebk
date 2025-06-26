using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums
{
    /// <summary>
    /// 任务推送状态类型
    /// </summary>
    public enum TaskPushStatusTypeEnum
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        Pending = 0,

        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 1,

        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Failed = 2,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Exception = 3,

        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Invalid = 4,
    }
}
