using System.ComponentModel;

namespace YueJia.Ebk.Domain.Shared.Enums
{
    public enum BedTypeEnum
    {
        /// <summary>
        /// 大床
        /// </summary>
        [Description("大床")]
        BigBed = 1,
        /// <summary>
        /// 双床
        /// </summary>
        [Description("双床")]
        TwinBed,
        /// <summary>
        /// 大床或双床
        /// </summary>
        [Description("大床或双床")]
        BigBedOrTwinBed,
        /// <summary>
        /// 单人床
        /// </summary>
        [Description("单人床")]
        SingleBed,
        /// <summary>
        /// 小型双人床
        /// </summary>
        [Description("小型双人床")]
        SmallDoubleBed,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown,
        /// <summary>
        /// 多张床
        /// </summary>
        [Description("多张床")]
        MultipleBeds
    }
}
