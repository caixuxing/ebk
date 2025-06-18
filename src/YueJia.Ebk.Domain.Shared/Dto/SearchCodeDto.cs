namespace YueJia.Ebk.Domain.Shared.Dto
{
    /// <summary>
    /// SearchCode对象
    /// </summary>
    public record SearchCodeDto
    {
        /// <summary>
        /// 每日价格ID
        /// </summary>
        public List<long> DailyPriceIds { get; set; } = default!;

        /// <summary>
        /// 每日库存ID
        /// </summary>
        public List<long> DailyInventoryIds { get; set; } = default!;
    }
}
