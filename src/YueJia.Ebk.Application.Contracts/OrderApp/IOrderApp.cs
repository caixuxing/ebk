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
    }
}
