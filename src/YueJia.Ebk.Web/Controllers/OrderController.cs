using YueJia.Ebk.Application.Contracts.OrderApp;
using YueJia.Ebk.Application.Contracts.OrderApp.Qry;
using YueJia.Ebk.Application.Contracts.SysApp;

namespace YueJia.Ebk.Web.Controllers
{

    /// <summary>
    /// 订单控制器
    /// </summary>
    [Authorize]
    public class OrderController : AbpController
    {

        private IOrderApp OrderApp => LazyServiceProvider.LazyGetRequiredService<IOrderApp>();

        private ISysEnumApp SysEnumApp => LazyServiceProvider.LazyGetRequiredService<ISysEnumApp>();

        /// <summary>
        /// 用户订单管理（View）
        /// </summary>
        /// <returns></returns>
        public IActionResult UserOrderMgr()
        {
            ViewBag.OrderStateData = JsonConvert.SerializeObject(SysEnumApp.GetEnumDataList(nameof(BookingStateTypeEnum)), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return View();
        }




        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("[controller]/OrderPageList")]
        public async Task<IResult> OrderPageList([FromBody] OrderPageListFilterQry qry) => ApiResult.HandleResult(await OrderApp.QueryOrderPageAsync(qry));



        /// <summary>
        /// 订单详情
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OrderDetail(string id) => View(await OrderApp.OrderDetailByIdAsync(id.ToLong()));




        /// <summary>
        /// 酒店确认号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="confirmNum"></param>
        /// <returns></returns>
        [HttpPut, Route("[controller]/HotelConfirmNum/{id}")]

        public async Task<IResult> HotelConfirmNum([FromRoute] string id, string confirmNum)
        {
            return ApiResult.HandleBoolResult(await OrderApp.SaveOrderConfirmNumAsync(id.ToLong(), confirmNum));
        }

    }
}
