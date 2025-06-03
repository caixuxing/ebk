using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
using YueJia.Ebk.Application.Contracts.OuterServiceApp;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Qry;
using YueJia.Ebk.Application.Contracts.SysApp;
using YueJia.Ebk.Web.ViewModels.Hotel;
namespace YueJia.Ebk.Web.Controllers;


/// <summary>
/// 酒店管理
/// </summary>
[Authorize]
public class HotelController : AbpController
{

    private IHotelPublishApp HotelPublishApp => LazyServiceProvider.LazyGetRequiredService<IHotelPublishApp>();

    private IYueJiaSysServiceApp YueJiaSysServiceApp => LazyServiceProvider.LazyGetRequiredService<IYueJiaSysServiceApp>();

    private ISysEnumApp SysEnumApp => LazyServiceProvider.LazyGetRequiredService<ISysEnumApp>();

    private IHotelApp HotelApp => LazyServiceProvider.LazyGetRequiredService<IHotelApp>();



    /// <summary>
    /// 酒店发布管理View
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> HotelPublishList()
    {
        ViewBag.CountryData = JsonConvert.SerializeObject(await YueJiaSysServiceApp.GetDropDownCountryListAsync(), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        ViewBag.HotelSaleTypeData = JsonConvert.SerializeObject(SysEnumApp.GetEnumDataList(nameof(HotelSaleTypeEnum)), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        await Task.Delay(1);
        return View();
    }


    /// <summary>
    /// 酒店发布管理
    /// </summary>
    /// <param name="requestQry"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/HotelPublishListData")]
    public async Task<IResult> GetPageList([FromBody] HotelPublishPageFilterQry requestQry) => ApiResult.HandleResult(await HotelPublishApp.GetMyHotelPublishPageListAsync(requestQry));


    /// <summary>
    /// 用户添加酒店
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> UserAddHotelMgr()
    {

        ViewBag.CountryList = await YueJiaSysServiceApp.GetDropDownCountryListAsync();
        return View();
    }


    /// <summary>
    /// 用户酒店导航
    /// </summary>
    /// <returns></returns>
    public IActionResult UserHotelNavigationMgr()
    {
        return View();
    }


    /// <summary>
    /// 确认添加酒店发布
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/PublishHotel")]
    public async Task<IResult> PublishHotel([FromBody] CreateOrUpHotelPublishCmd cmd) => ApiResult.HandleBoolResult(await HotelPublishApp.PublishHotelAsync(cmd));

    /// <summary>
    /// 更新酒店发布
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut, Route("[controller]/{id}/UpdatePublishHotel")]
    public async Task<IResult> UpdatePublishHotel([FromBody] CreateOrUpHotelPublishCmd cmd, [FromRoute] string id) => ApiResult.HandleBoolResult(await HotelPublishApp.UpdatePublishHotelAsync(cmd, id.ToLong()));

    /// <summary>
    /// 酒店发布详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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
            Id = id,
            HotelName = hotelPublishDetail.HotelName,
            HotelNameEn = hotelPublishDetail.HotelNameEn,
            HotelCode = hotelPublishDetail.HotelCode,
            Address = hotelPublishDetail.Address,
            AddressEn = hotelPublishDetail.AddressEn,
            LowestPrice = hotelPublishDetail.LowestPrice,
            Status = hotelPublishDetail.Status,
            TelPhone = hotelPublishDetail.TelPhone,
        };
        ViewBag.HotelSaleTypeList = SysEnumApp.GetEnumDataList(nameof(HotelSaleTypeEnum));
        return View(mv);
    }





    /// <summary>
    /// 添加酒店房间(View)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> AddHotelRoom(string id)
    {
        var hotelPublishDetail = await HotelPublishApp.GetHotelPublishDetailAsync(id.ToLong());
        AddHotelRoomVo vm = new AddHotelRoomVo()
        {
            HotelId = id,
            HotelName = $"{hotelPublishDetail.HotelName}({hotelPublishDetail.HotelNameEn})",
            HotelCode = hotelPublishDetail.HotelCode,
            BedType = "",
            RoomType = "",
            MaximumNumberOfPeople = 2,
            AdultLimit = 2,
            ChildLimit = 0,
            Stock = new StockVo()
            {
                EndDate = DateTime.Now,
                StartDate = DateTime.Now,
                Stock = new Dictionary<DayOfWeek, int>() {
                    { DayOfWeek.Monday, 0 },
                    { DayOfWeek.Tuesday, 0 },
                    { DayOfWeek.Wednesday, 0 },
                    { DayOfWeek.Thursday, 0 },
                    { DayOfWeek.Friday, 0 },
                    { DayOfWeek.Saturday, 0 },
                    { DayOfWeek.Sunday, 0 }
                }
            }
        };
        return View(vm);
    }

    /// <summary>
    /// 添加酒店房间
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/AddHotelRoom")]
    public async Task<IResult> AddHotelRoom([FromBody] CreateHotelRoomCmd cmd) => ApiResult.HandleLongResult(await HotelApp.AddHotelRoomAsync(cmd));


    /// <summary>
    /// 删除酒店房间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete, Route("[controller]/DeleteHotelRoom/{id}")]
    public async Task<IResult> DeleteHotelRoom([FromRoute] string id) => ApiResult.HandleResult(await HotelApp.DeleteHotelRoomAsync(id.ToLong()));



    /// <summary>
    /// 房间与价格计划（View）
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> RoomAndPricePlan(string id)
    {

        var hotel = await HotelPublishApp.GetHotelPublishDetailAsync(id.ToLong());
        var roomAndPricePlan = await HotelApp.GetHotelRoomListByIdAsync(id.ToLong());

        var roomAndPricePlanVm = roomAndPricePlan.Select(x => new
        {
            Id = x.Id.ToString(),
            x.RoomType,
            x.RoomTypeName,
            x.BedType,
            x.BedTypeName,
            x.MaximumNumberOfPeople,
            x.AdultLimit,
            x.ChildLimit,
            x.IsEnabledName,
            pricePlans = x.PricePlans,
            ShowContent = false,
            ShowFooter = true
        });

        ViewBag.mv = new ViewHotelVo()
        {
            Id = id,
            HotelName = hotel.HotelName,
        };

        return View(new RoomAndPricePlanVo()
        {
            Id = id,
            HotelCode = hotel.HotelCode,
            HotelName = hotel.HotelName,
            HotelNameEn = hotel.HotelNameEn,
            HotelRoomListJson = JsonConvert.SerializeObject(roomAndPricePlanVm, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })

        });
    }

    /// <summary>
    /// 房间与价格计划列表
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("[controller]/RoomAndPricePlanList/{id}")]
    public async Task<IResult> RoomAndPricePlanList([FromRoute] string id)
    {
        var roomAndPricePlan = await HotelApp.GetHotelRoomListByIdAsync(id.ToLong());
        return ApiResult.HandleResult(roomAndPricePlan.Select(x => new
        {
            Id = x.Id.ToString(),
            x.RoomType,
            x.RoomTypeName,
            x.BedType,
            x.BedTypeName,
            x.MaximumNumberOfPeople,
            x.AdultLimit,
            x.ChildLimit,
            x.IsEnabledName,
            pricePlans = x.PricePlans,
            ShowContent = false,
            ShowFooter = true
        }));
    }



    /// <summary>
    ///  新增价格计划（View）
    /// </summary>
    /// <param name="id">房间ID</param>
    /// <returns></returns>
    public async Task<IActionResult> AddPricePlan(string id)
    {
        var room = await HotelApp.GetHotelRoomByIdAsync(id.ToLong());
        var hotel = await HotelPublishApp.GetHotelPublishDetailAsync(room.HotelId);
        AddPricePlanVo vm = new AddPricePlanVo()
        {
            HotelId = room.HotelId.ToString(),
            HotelRoomId = room.Id.ToString(),
            HotelCode = hotel.HotelCode,
            HotelName = $"{hotel.HotelName}({hotel.HotelNameEn})",
            BedTypeName = room.BedTypeName,
            RoomTypeName = room.RoomTypeName,

            BreakfastType = null,
            DaysInAdvance = 1,
            ContinuousStayDays = 1,
            IsEnable = YesOrNoType.Yes,
            IsReservedRoom = YesOrNoType.Yes,
        };

        return View(vm);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> EditPricePlan(string id)
    {
        var data = await HotelApp.GetPricePlanDetailsByIdAsync(id.ToLong());
        AddPricePlanVo vm = new AddPricePlanVo()
        {
            HotelId = data.HotelId,
            HotelRoomId = data.Id,
            HotelCode = data.HotelCode,
            HotelName = $"{data.HotelName}({data.HotelNameEn})",
            BedTypeName = data.BedTypeName,
            RoomTypeName = data.RoomTypeName,
            BreakfastType = data.BreakfastType,
            DaysInAdvance = data.DaysInAdvance,
            ContinuousStayDays = data.ContinuousStayDays,
            IsEnable = data.IsEnable,
            IsReservedRoom = data.IsEnable,
            PricePlanId = data.Id,
        };
        return View("AddPricePlan", vm);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/CreatePricePlan")]
    public async Task<IResult> CreatePricePlan([FromBody] CreateOrUpdatePricePlanCmd cmd) => ApiResult.HandleLongResult(await HotelApp.CreatePricePlanAsync(cmd));

    /// <summary>
    /// 删除价格计划
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete, Route("[controller]/DeletePricePlan/{id}")]
    public async Task<IResult> DeletePricePlan([FromRoute] string id) => ApiResult.HandleBoolResult(await HotelApp.DeletePricePlanAsync(id.ToLong()));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut, Route("[controller]/UpdatePricePlan/{id}")]
    public async Task<IResult> UpdatePricePlan([FromBody] CreateOrUpdatePricePlanCmd cmd, string id) => ApiResult.HandleBoolResult(await HotelApp.UpdatePricePlanAsync(cmd, id.ToLong()));




    /// <summary>
    /// 库存和价格（View）
    /// </summary>
    /// <returns></returns>
    public IActionResult InventoryAndPrice()
    {

        return View();
    }



    /// <summary>
    /// 加载库存和价格（View）
    /// </summary>
    /// <returns></returns>
    public IActionResult LoadingInventoryAndPrices()
    {

        return View();
    }
}
