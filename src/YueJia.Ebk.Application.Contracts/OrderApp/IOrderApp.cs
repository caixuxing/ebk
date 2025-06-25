using YueJia.Ebk.Application.Contracts.OrderApp.Commands;
using YueJia.Ebk.Application.Contracts.OrderApp.Dto;
using YueJia.Ebk.Application.Contracts.OrderApp.Qry;

namespace YueJia.Ebk.Application.Contracts.OrderApp
{
    /// <summary>
    /// 订单应用层接口
    /// </summary>
    public interface IOrderApp
    {

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        Task<bool> CreateOrderAsync(CreateOrderCmd cmd);


        /// <summary>
        /// 查询订单列表
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        Task<PageData<IEnumerable<OrderPageListDto>>> QueryOrderPageAsync(OrderPageListFilterQry qry);



        /// <summary>
        /// 按Id查询订单详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        Task<OrderDetailDto> OrderDetailByIdAsync(long id);


        /// <summary>
        /// 保存订单酒店确认号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="confirmNum"></param>
        /// <returns></returns>
        Task<bool> SaveOrderConfirmNumAsync(long id, string confirmNum);

        Task<bool> SetInputRemark(string orderNum, string inputRemark);

        Task<List<OrderPersonDto>> GetOrderPersonList(string orderNum);

        Task<List<OrderDailyPriceDto>> GetOrderDailyPriceList(string orderNum);

        Task<List<OrderLogDto>> GetOrderLogList(string orderNum);
    }
}
