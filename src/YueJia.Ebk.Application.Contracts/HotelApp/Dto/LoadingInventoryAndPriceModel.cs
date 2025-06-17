using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    public class LoadingInventoryAndPriceModel
    {
        public string UserHotelId { get; set; }

        public string UserRoomId { get; set; }

        public string StartDateString { get; set; }
        public string EndDateString { get; set; }
        public string NewEndDateString { get; set; }

        public int BatchSetNumber { get; set; }

        /// <summary>
        /// 星期一
        /// </summary>
        public int Monday { get; set; }
        /// <summary>
        /// 星期二
        /// </summary>
        public int Tuesday { get; set; }
        /// <summary>
        /// 星期三
        /// </summary>
        public int Wednesday { get; set; }
        /// <summary>
        /// 星期四
        /// </summary>
        public int Thursday { get; set; }
        /// <summary>
        /// 星期五
        /// </summary>
        public int Friday { get; set; }
        /// <summary>
        /// 星期六
        /// </summary>
        public int Saturday { get; set; }
        /// <summary>
        /// 星期天
        /// </summary>
        public int Sunday { get; set; }

        public List<LoadingInventoryAndPricePlanModel> PlanList { get; set; }
    }


    public class LoadingInventoryAndPricePlanModel
    {

        public string UserPlanId { get; set; }
        public string UserPlanTitel { get; set; }

        public bool UserPlanStatusBool { get; set; }

        public int BatchSetPrice { get; set; }

        /// <summary>
        /// 星期一
        /// </summary>
        public decimal Monday { get; set; }
        /// <summary>
        /// 星期二
        /// </summary>
        public decimal Tuesday { get; set; }
        /// <summary>
        /// 星期三
        /// </summary>
        public decimal Wednesday { get; set; }
        /// <summary>
        /// 星期四
        /// </summary>
        public decimal Thursday { get; set; }
        /// <summary>
        /// 星期五
        /// </summary>
        public decimal Friday { get; set; }
        /// <summary>
        /// 星期六
        /// </summary>
        public decimal Saturday { get; set; }
        /// <summary>
        /// 星期天
        /// </summary>
        public decimal Sunday { get; set; }
    }
}
