namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    public class BatchEditInventoryAndPricesModel
    {
        public string userHotelId { get; set; }

        public List<int> weekIndexList { get; set; }

        public string startDate { get; set; }
        public string endDate { get; set; }

        public List<string> userRoomIdList { get; set; }
        public List<string> userPlanIdList { get; set; }




        /// <summary>
        /// 修改标识[库存数量]
        /// </summary>
        public bool inventoryNumFlag { get; set; }
        /// <summary>
        /// 1:绝对值
        /// 2：增加
        /// </summary>
        public string inventoryNumExecType { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int inventoryNum { get; set; }
        /// <summary>
        /// 修改标识[库存状态]
        /// </summary>

        public bool inventoryStateFlag { get; set; }

        public bool inventoryState { get; set; }



        /// <summary>
        /// 修改范围 周
        /// </summary>
        public List<int> planPriceUpRange { get; set; } = new();

        /// <summary>
        /// 修改标识[价格]
        /// </summary>
        public bool planPriceFlag { get; set; }
        /// <summary>
        /// 1:绝对值
        /// 2：增加
        /// 3：百分比
        /// </summary>
        public string planPriceExecType { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal planPrice { get; set; }
        public bool planPriceStateFlag { get; set; }
        public bool planPriceState { get; set; }

    }
}
