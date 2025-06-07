namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    /// <summary>
    /// 房间价格计划列表DTO
    /// </summary>
    public class RoomPricingPlanListDto
    {
        /// <summary>
        /// 价格计划ID
        /// </summary>
        public string PricePlanId { get; set; } = default!;
        /// <summary>
        /// 价格计划标题
        /// </summary>
        public string PricePlanTitle { get; set; } = default!;
        /// <summary>
        /// 是否启用
        /// </summary>
        public YesOrNoType IsEnabled { get; set; }
        /// <summary>
        /// 是否启用名称
        /// </summary>
        public string IsEnabledName
        {

            get { return IsEnabled.ToString(); }
        }
        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomId { get; set; } = default!;
    }
}
