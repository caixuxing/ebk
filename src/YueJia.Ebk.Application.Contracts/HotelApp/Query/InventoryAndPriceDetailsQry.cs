namespace YueJia.Ebk.Application.Contracts.HotelApp.Query
{
    /// <summary>
    /// 查询库存和价格详情
    /// </summary>
    public class InventoryAndPriceDetailsQry
    {

        /// <summary>
        /// 酒店ID
        /// </summary>
        public string HotelId { get; set; }
        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Now.Date;

        /// <summary>
        /// 天数
        /// </summary>
        public int Days { get; set; } = 7;
    }
}
