using YueJia.Ebk.Application.Contracts.OrderApp;
using YueJia.Ebk.Application.Contracts.OrderApp.Qry;

namespace YueJia.Ebk.Web.Controllers
{

    /// <summary>
    /// 订单控制器
    /// </summary>
    [Authorize]
    public class OrderController : AbpController
    {

        private IOrderApp OrderApp => LazyServiceProvider.LazyGetRequiredService<IOrderApp>();


        /// <summary>
        /// 用户订单管理（View）
        /// </summary>
        /// <returns></returns>
        public IActionResult UserOrderMgr() => View();




        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("[controller]/OrderPageList")]
        public async Task<IResult> OrderPageList([FromBody] OrderPageListFilterQry qry) => ApiResult.HandleResult(await OrderApp.QueryOrderPageAsync(qry));

    }
}
