using YueJia.Ebk.Application.Contracts.EbkApp;
using YueJia.Ebk.Application.Contracts.EbkApp.Commands;
using YueJia.Ebk.Application.Contracts.EbkApp.Query;

namespace YueJia.Ebk.Web.Controllers;



/// <summary>
/// Ebk接口
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class EbkController : AbpController
{
    private IEbkApp EbkApp => LazyServiceProvider.LazyGetRequiredService<IEbkApp>();

    /// <summary>
    /// 查价
    /// </summary>
    /// <returns></returns>
    [HttpPost("PriceSearch")]
    public async Task<IResult> PriceSearch([FromBody] PriceSearchQry qry) => ApiResult.HandleResult(await EbkApp.PriceSearch(qry));


    /// <summary>
    /// 验价
    /// </summary>
    /// <returns></returns>
    [HttpPost("PriceCheck")]
    public async Task<IResult> PriceCheck([FromBody] PriceCheckQry qry) => ApiResult.HandleResult(await EbkApp.PriceCheckQry(qry));


    /// <summary>
    /// 创建订单
    /// </summary>
    /// <returns></returns>
    [HttpPost("CreateOrder")]
    public async Task<IResult> CreateOrder([FromBody] CreateOrderCmd qry) => ApiResult.HandleResult(string.Empty);




}
