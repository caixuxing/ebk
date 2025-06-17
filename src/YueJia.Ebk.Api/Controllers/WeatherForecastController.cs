using Microsoft.AspNetCore.Mvc;
using YueJia.Ebk.Application.Contracts.EbkApp;
using YueJia.Ebk.Application.Contracts.EbkApp.Dto;
using YueJia.Ebk.Application.Contracts.EbkApp.Query;

namespace YueJia.Ebk.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase, IEbkApp
    {


        public Task<HotelPriceDto> PriceCheckQry(PriceCheckQry qry)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<HotelPriceDto>> PriceSearch(PriceSearchQry qry)
        {
            throw new NotImplementedException();
        }
    }
}
