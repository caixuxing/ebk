using YueJia.Ebk.Application.Contracts.EbkApp.Dto;
using YueJia.Ebk.Application.Contracts.EbkApp.Query;

namespace YueJia.Ebk.Application.Contracts.EbkApp;

/// <summary>
/// EbkApp接口
/// </summary>
public interface IEbkApp
{

    /// <summary>
    /// 查询酒店价格
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<HotelPriceDto>> PriceSearch(PriceSearchQry qry);


    /// <summary>
    /// 验价
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    Task<HotelPriceDto> PriceCheckQry(PriceCheckQry qry);
}
