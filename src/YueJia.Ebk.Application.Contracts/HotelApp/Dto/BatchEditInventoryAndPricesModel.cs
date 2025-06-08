using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    public class BatchEditInventoryAndPricesModel
    {

        public string startDate { get; set; }
        public string endDate { get; set; }

        public List<string> userRoomIdList { get; set; }
        public List<string> userPlanIdList { get; set; }

        /// <summary>
        /// 修改标识[库存数量]
        /// </summary>
        public bool numFlag { get; set; }
        /// <summary>
        /// 1:绝对值
        /// 2：增加
        /// </summary>
        public string numExecType { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int num { get; set; }
        /// <summary>
        /// 修改标识[库存状态]
        /// </summary>

        public bool numStateFlag { get; set; }

        public bool numState { get; set; }


        /// <summary>
        /// 修改标识[价格]
        /// </summary>

        public bool priceFlag { get; set; }
        /// <summary>
        /// 1:绝对值
        /// 2：增加
        /// 3：百分比
        /// </summary>
        public string priceExecType { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal price { get; set; }
        public bool priceStateFlag { get; set; }
        public bool priceState { get; set; }

        
    }
}
