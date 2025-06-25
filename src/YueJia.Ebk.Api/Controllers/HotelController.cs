using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
using YueJia.Ebk.Infrastructure.Extensions;

namespace YueJia.Ebk.Api.Controllers;

/// <summary>
/// 酒店
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HotelController : AbpController
{

    private IHotelApp HotelApp => LazyServiceProvider.LazyGetRequiredService<IHotelApp>();
    /// <summary>
    /// 查价
    /// </summary>
    /// <returns></returns>
    [HttpPost("SearchPrice")]
    public async Task<IResult> PriceSearch([FromBody] PriceSearchQry qry) => ApiResult.HandleResult(await HotelApp.SearchPrice(qry));

    /// <summary>
    /// 验价
    /// </summary>
    /// <returns></returns>
    [HttpPost("CheckPrice")]
    public async Task<IResult> PriceCheck([FromBody] PriceCheckQry qry) => ApiResult.HandleResult(await HotelApp.CheckPrice(qry));
}
