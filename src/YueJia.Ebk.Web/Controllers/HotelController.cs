using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
using YueJia.Ebk.Application.Contracts.OuterServiceApp;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Qry;
using YueJia.Ebk.Application.Contracts.SysApp;
using YueJia.Ebk.Web.ViewModels.Hotel;
namespace YueJia.Ebk.Web.Controllers;

public class HotelController : AbpController
{

    private IHotelPublishApp HotelPublishApp => LazyServiceProvider.LazyGetRequiredService<IHotelPublishApp>();

    private IYueJiaSysServiceApp YueJiaSysServiceApp => LazyServiceProvider.LazyGetRequiredService<IYueJiaSysServiceApp>();

    private ISysEnumApp SysEnumApp => LazyServiceProvider.LazyGetRequiredService<ISysEnumApp>();



    /// <summary>
    /// 酒店发布管理View
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> HotelPublishList()
    {
        ViewBag.CountryData = JsonConvert.SerializeObject(await YueJiaSysServiceApp.GetDropDownCountryListAsync(), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        ViewBag.HotelSaleTypeData = JsonConvert.SerializeObject(SysEnumApp.GetEnumDataList(nameof(HotelSaleTypeMnum)), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        return View();
    }


    /// <summary>
    /// 酒店发布管理
    /// </summary>
    /// <param name="requestQry"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/HotelPublishListData")]
    public async Task<IResult> GetPageList([FromBody] HotelPublishPageFilterQry requestQry) => ApiResult.HandleResult(await HotelPublishApp.GetMyHotelPublishPageListAsync(requestQry));



    [HttpPost, Route("[controller]/PublishHotel")]
    public async Task<IResult> PublishHotel([FromBody] CreateOrUpHotelPublishCmd cmd) => ApiResult.HandleLongResult(await HotelPublishApp.PublishHotelAsync(cmd));


    [HttpPut, Route("[controller]/{id}/UpdatePublishHotel")]
    public async Task<IResult> UpdatePublishHotel([FromBody] CreateOrUpHotelPublishCmd cmd, [FromRoute] string id) => ApiResult.HandleBoolResult(await HotelPublishApp.UpdatePublishHotelAsync(cmd, id.ToLong()));


    [HttpGet, Route("[controller]/{id}/Detail")]
    public async Task<IResult> GetHotelPublishDetail([FromRoute] string id) => ApiResult.HandleResult(await HotelPublishApp.GetHotelPublishDetailAsync(id.ToLong()));


    /// <summary>
    /// 酒店列表
    /// </summary>
    /// <param name="requestQry"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/HotelPageList")]
    public async Task<IResult> GetHotelList([FromBody] HotelPageListFilterQry requestQry) => ApiResult.HandleResult(await YueJiaSysServiceApp.GetHotelPageListAsync(requestQry));




    /// <summary>
    /// 酒店详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> ViewHotel(string id)
    {
        var hotelPublishDetail = await HotelPublishApp.GetHotelPublishDetailAsync(id.ToLong());
        ViewHotelVo mv = new ViewHotelVo()
        {
            Id = id.ToLong(),
            HotelName = hotelPublishDetail.HotelName,
            HotelNameEn = hotelPublishDetail.HotelNameEn,
            HotelCode = hotelPublishDetail.HotelCode,
            Address = hotelPublishDetail.Address,
            AddressEn = hotelPublishDetail.AddressEn,
            LowestPrice = hotelPublishDetail.LowestPrice,
            Status = hotelPublishDetail.Status,
            TelPhone = hotelPublishDetail.TelPhone,
            HotelSaleTypeJson = JsonConvert.SerializeObject(SysEnumApp.GetEnumDataList(nameof(HotelSaleTypeMnum)),
                                                             new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
        };
        return View(mv);
    }



    /// <summary>
    /// 添加酒店房间
    /// </summary>
    /// <param name="hotelCode"></param>
    /// <returns></returns>
    public async Task<IActionResult> AddHotelRoom(string hotelCode)
    {
        AddHotelRoomVo vm = new AddHotelRoomVo()
        {
            SelectRoomTypeJosn = JsonConvert.SerializeObject(await YueJiaSysServiceApp.GetDropDownRoomTypeByHotelCodeAsync(hotelCode)),
            SelectBedTypeJosn = JsonConvert.SerializeObject(SysEnumApp.GetEnumDataList(nameof(BedTypeEnum)))
        };
        return View(vm);
    }


    public IActionResult RoomAndPricePlan(string id, string hotelCode)
    {
        RoomAndPricePlanVo vm = new RoomAndPricePlanVo()
        {
            Id = id,
            HotelCode = hotelCode,

        };
        return View(vm);
    }
}
