using YueJia.Ebk.Application.Contracts.OrderApp;
using YueJia.Ebk.Application.Contracts.OrderApp.Dto;
using YueJia.Ebk.Application.Contracts.OrderApp.Qry;
using YueJia.Ebk.Application.Contracts.OuterServiceApp;
using YueJia.Ebk.Application.Contracts.SysApp;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.OuterServiceApp;
using YueJia.Ebk.Application.SysUserApp;

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

        private ICurrentUserApp currentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();

        private ISysUserApp sysUserApp => LazyServiceProvider.LazyGetRequiredService<ISysUserApp>();

        private IYueJiaSysServiceApp yueJiaSysServiceApp => LazyServiceProvider.LazyGetRequiredService<IYueJiaSysServiceApp>();


        /// <summary>
        /// 用户订单管理（View）
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UserOrderMgr()
        {
            ViewBag.UserId = currentUserApp.Id.ToString();
            ViewBag.GetManageUserList = sysUserApp.GetManageUserList();
            ViewBag.CountryList = await yueJiaSysServiceApp.GetDropDownCountryListAsync();
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
        public async Task<IActionResult> OrderDetail(string orderId)
        {
            var orderModel = await OrderApp.OrderDetailByIdAsync(orderId.ToLong());
            ViewBag.PersonList = await OrderApp.GetOrderPersonList(orderModel.OrderNum);
            ViewBag.OrderDailyPriceList = await OrderApp.GetOrderDailyPriceList(orderModel.OrderNum);
            ViewBag.OrderLogList = await OrderApp.GetOrderLogList(orderModel.OrderNum);
            return View(orderModel);
        }

        

        [HttpPost, Route("[controller]/SetInputRemark")]

        public async Task<IResult> SetInputRemark([FromBody] OrderLogDto cmd)
        {
            return ApiResult.HandleBoolResult(await OrderApp.SetInputRemark(cmd.OrderNum, cmd.Describe));
        }


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
