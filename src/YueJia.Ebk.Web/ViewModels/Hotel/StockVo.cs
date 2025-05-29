namespace YueJia.Ebk.Web.ViewModels.Hotel
{
    public class StockVo
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public Dictionary<DayOfWeek, int> Stock { get; set; } = new();
    }
}
