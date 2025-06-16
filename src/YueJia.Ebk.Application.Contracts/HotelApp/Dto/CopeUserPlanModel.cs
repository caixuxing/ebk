using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    public class CopeUserPlanModel
    {
        public string CopeUserPlanId { get; set; }
        /// <summary>
        /// 早餐类型
        /// </summary>
        public BreakfastTypeEnum BreakfastType { get; set; }
        /// <summary>
        /// 提前天数
        /// </summary>
        public int DaysInAdvance { get; set; }

        /// <summary>
        /// 连住天数
        /// </summary>
        public int ContinuousStayDays { get; set; }

        /// <summary>
        /// 是否保留房
        /// </summary>
        public YesOrNoType IsReservedRoom { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public YesOrNoType IsEnable { get; set; }
        /// <summary>
        /// 增加
        /// </summary>
        public int AddPrice { get; set; }
    }
}
