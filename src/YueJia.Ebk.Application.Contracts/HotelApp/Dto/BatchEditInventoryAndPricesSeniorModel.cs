using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    public class BatchEditInventoryAndPricesSeniorModel
    {
        public List<UserRoomModel> userRoomList { get; set; }
        public List<UserPlanList> userPlanList { get; set; }
    }

    public class UserRoomModel { 
        public string Id { get; set; }

        /// <summary>
        /// 1:绝对值
        /// 2：增加
        /// 3：无变化
        /// </summary>
        public string numExecType { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// 1：启用
        /// 2：停用
        /// 3：无变化
        /// </summary>

        public string numState { get; set; }
    }

    public class UserPlanList {

        public string Id { get; set; }
        /// <summary>
        /// 1:绝对值
        /// 2：增加
        /// </summary>
        public string priceExecType { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 1：启用
        /// 2：停用
        /// 3：无变化
        /// </summary>

        public string priceState { get; set; }
    }
}
