using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    public class BatchEditInventoryAndPricesSeniorModel
    {

        public string userHotelId { get; set; }

        public List<int> weekIndexList { get; set; }


        public string startDate { get; set; }
        public string endDate { get; set; }
        public List<UserRoomModel> userRoomList { get; set; }
        public List<UserPlanList> userPlanList { get; set; }
    }

    public class UserRoomModel { 
        public string Id { get; set; }

        /// <summary>
        ///  ：无变化
        /// 1:绝对值
        /// 2：增加
        /// </summary>
        public string inventoryNumExecType { get; set; }
        

        /// <summary>
        /// 数量
        /// </summary>
        public int inventoryNum { get; set; }

        /// <summary>
        /// ：无变化
        /// 1：启用
        /// 2：停用
        /// </summary>

        public string inventoryStateType { get; set; }
    }

    public class UserPlanList {

        public string Id { get; set; }
        /// <summary>
        /// :无变化
        /// 1:绝对值
        /// 2：增加
        /// </summary>
        public string planPriceExecType { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal planPrice { get; set; }

        /// <summary>
        /// ：无变化
        /// 1：启用
        /// 2：停用
        /// </summary>

        public string planPriceStateType { get; set; }
    }
}
