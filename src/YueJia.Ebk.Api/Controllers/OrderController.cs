using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using YueJia.Ebk.Application.Contracts.OrderApp;
using YueJia.Ebk.Application.Contracts.OrderApp.Commands;
using YueJia.Ebk.Application.Contracts.OrderApp.Qry;
using YueJia.Ebk.Infrastructure.Extensions;


namespace YueJia.Ebk.Api.Controllers;


/// <summary>
/// 订单
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrderController : AbpController
{

    private IOrderApp OrderApp => LazyServiceProvider.LazyGetRequiredService<IOrderApp>();


    /// <summary>
    /// 创建订单
    /// </summary>
    /// <returns></returns>
    [HttpPost("CreateOrder")]
    public async Task<IResult> CreateOrder([FromBody] CreateOrderCmd cmd) => ApiResult.HandleBoolResult(await OrderApp.CreateOrderAsync(cmd));


    ///// <summary>
    ///// 订单列表
    ///// </summary>
    ///// <returns></returns>
    //[HttpPost("OrderPageList")]
    //public async Task<IResult> OrderPageList([FromBody] OrderPageListFilterQry qry) => ApiResult.HandleResult(await OrderApp.QueryOrderPageAsync(qry));
}
