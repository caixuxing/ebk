using YueJia.Ebk.Application.Contracts.EbkApp;
using YueJia.Ebk.Application.Contracts.EbkApp.Query;

namespace YueJia.Ebk.Web.Controllers;




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
    public async Task<IResult> PriceSearch([FromBody] PriceSearchQry qry)
    {
        var result = await EbkApp.PriceSearch(qry);
        return ApiResult.HandleResult(result);
    }
}
