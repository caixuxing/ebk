using SqlSugar;
using YueJia.Ebk.Application.Contracts.Comm.BaseObj;
using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
using YueJia.Ebk.Application.Contracts.OuterServiceApp;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Qry;
using YueJia.Ebk.Application.Contracts.SysApp;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Shared.Const;
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



    private ISimpleClient<HotelRoomDo> HotelRoomRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<HotelRoomDo>>();
    private ISimpleClient<PricePlanDo> PricePlanRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<PricePlanDo>>();

    private ISimpleClient<HotelPublishDo> HotelPublishRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<HotelPublishDo>>();


    private ISqlSugarClient SqlSugarClient => LazyServiceProvider.GetRequiredKeyedService<ISqlSugarClient>(DbConst.YueJiaSysDb);


    /// <summary>
    /// 酒店发布管理View
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> HotelPublishList()
    {
        ViewBag.CountryData = JsonConvert.SerializeObject(await YueJiaSysServiceApp.GetDropDownCountryListAsync(), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        ViewBag.HotelSaleTypeData = JsonConvert.SerializeObject(SysEnumApp.GetEnumDataList(nameof(HotelSaleTypeEnum)), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
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
        ViewBag.HotelName = $"{hotelPublishDetail.HotelName}({hotelPublishDetail.HotelNameEn})";
        CreateHotelRoomCmd vm = new CreateHotelRoomCmd()
        {
            HotelId = id,
            HotelCode = hotelPublishDetail.HotelCode,
            RoomType = "",
            HotelRoomTitle = "",
            MaximumNumberOfPeople = 2,
            AdultLimit = 2,
            ChildLimit = 0,
            EndDate = DateTime.Now.AddMonths(1).Date,
            StartDate = DateTime.Now.Date,
            BedType = BedTypeEnum.Unknown,
        };


        return View(vm);
    }

    /// <summary>
    /// 添加酒店房间
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/AddHotelRoom")]
    public async Task<IResult> AddHotelRoom([FromBody] CreateHotelRoomCmd cmd) => ApiResult.HandleBoolResult(await HotelApp.AddHotelRoomAsync(cmd));


    /// <summary>
    /// 删除酒店房间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete, Route("[controller]/DeleteHotelRoom/{id}")]
    public async Task<IResult> DeleteHotelRoom([FromRoute] string id) => ApiResult.HandleResult(await HotelApp.DeleteHotelRoomAsync(id.ToLong()));

    /// <summary>
    /// 切换价格状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/UpdateRoomState/{id}")]
    public async Task<IResult> UpdateRoomState(string id) => ApiResult.HandleBoolResult(await HotelApp.UpdateRoomStateAsync(id.ToLong()));


    #region 房间与价格计划
    /// <summary>
    /// 房间与价格计划（View）
    /// </summary>
    /// <param name="id">酒店唯一标识</param>
    /// <returns></returns>
    public async Task<IActionResult> RoomAndPricePlan(string id)
    {
        var hotel = await HotelPublishApp.GetHotelPublishDetailAsync(id.ToLong());




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
        });
    }
    #endregion

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
            x.StartDateString,
            x.EndDateString,
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

        ViewBag.HotelName = $"{hotel.HotelName}({hotel.HotelNameEn})";
        ViewBag.LowestPrice = $"{hotel.LowestPrice}";
        ViewBag.BedTypeName = $"{room.BedTypeName}";
        ViewBag.RoomTypeName = $"{room.RoomTypeName}";
        ViewBag.AdultLimit = $"{room.AdultLimit}";
        ViewBag.ChildLimit = $"{room.ChildLimit}";
        ViewBag.MaximumNumberOfPeople = $"{room.MaximumNumberOfPeople}";

        CreateOrUpdatePricePlanCmd vm = new CreateOrUpdatePricePlanCmd()
        {
            HotelRoomId = room.Id.ToString(),
            BreakfastType = BreakfastTypeEnum.Breakfast,
            DaysInAdvance = 1,
            ContinuousStayDays = 1,
            IsEnable = YesOrNoType.Yes,
            IsReservedRoom = YesOrNoType.Yes,
        };

        return View(vm);
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="id"></param>
    ///// <returns></returns>
    //public async Task<IActionResult> EditPricePlan(string id)
    //{
    //    var data = await HotelApp.GetPricePlanDetailsByIdAsync(id.ToLong());
    //    AddPricePlanVo vm = new AddPricePlanVo()
    //    {
    //        HotelId = data.HotelId,
    //        HotelRoomId = data.Id,
    //        HotelCode = data.HotelCode,
    //        HotelName = $"{data.HotelName}({data.HotelNameEn})",
    //        BedTypeName = data.BedTypeName,
    //        RoomTypeName = data.RoomTypeName,
    //        BreakfastType = data.BreakfastType,
    //        DaysInAdvance = data.DaysInAdvance,
    //        ContinuousStayDays = data.ContinuousStayDays,
    //        IsEnable = data.IsEnable,
    //        IsReservedRoom = data.IsEnable,
    //        //PricePlanId = data.Id,
    //    };
    //    return View("AddPricePlan", vm);
    //}



    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/CreatePricePlan")]
    public async Task<IResult> CreatePricePlan([FromBody] CreateOrUpdatePricePlanCmd cmd) => ApiResult.HandleBoolResult(await HotelApp.CreatePricePlanAsync(cmd));

    /// <summary>
    /// 删除价格计划
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete, Route("[controller]/DeletePricePlan/{id}")]
    public async Task<IResult> DeletePricePlan([FromRoute] string id) => ApiResult.HandleBoolResult(await HotelApp.DeletePricePlanAsync(id.ToLong()));

    /// <summary>
    /// 切换价格计划状态
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/UpdatePricePlanState/{id}")]
    public async Task<IResult> UpdatePricePlanState(string id) => ApiResult.HandleBoolResult(await HotelApp.UpdatePricePlanStateAsync(id.ToLong()));




    /// <summary>
    /// 库存和价格（View）
    /// </summary>
    /// <param name="id">酒店Id</param>
    /// <returns></returns>
    public async Task<IActionResult> InventoryAndPrice(string id)
    {
        long hotelId = id.ToLong();

        var entity = await HotelPublishRepo.GetByIdAsync(hotelId) ?? throw new InvalidOperationException("酒店不存在！");

        var room = await HotelRoomRepo.GetListAsync(x => x.HotelId == hotelId);

        InventoryAndPriceDetailsQry qry = new() { HotelId = id, RoomId = room.FirstOrDefault()?.Id.ToString() ?? "0" };
        var result = await HotelApp.InventoryAndPriceViewAsync(qry);

        result.HotelId = entity.Id.ToString();
        result.HotelName = entity.HotelName;
        result.HotelNameEn = entity.HotelNameEn;
        result.HotelCode = entity.HotelCode;
        result.RoomTypeValue = qry?.RoomId.ToString() ?? string.Empty;

        var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
                            .Where(q => q.pfcode == "D" && q.hotelcode == entity.HotelCode)
                            .Select(t => new { t.roomcode, t.roomname })
                            .ToListAsync();

        result.RoomDropDownList = room.Select(x => new SelectDataDto<string>()
        {
            Label = $"{x.Id} {currentHotelRoomTypeDate.FirstOrDefault(y => y.roomcode == int.Parse(x.RoomType))?.roomname ?? string.Empty},{x.BedType.ToDescription()}",
            Value = x.Id.ToString()
        }).ToList();



        return View(result);
    }


    /// <summary>
    /// 按条件筛选库存和价格
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/InventoryAndPriceDetailsByFilter")]
    public async Task<IResult> InventoryAndPriceDetailsByFilter([FromBody] InventoryAndPriceDetailsQry qry)
    {
        var result = await HotelApp.InventoryAndPriceViewAsync(qry);

        return ApiResult.HandleResult(result.RoomTypeInfo);

    }



    /// <summary>
    /// 加载库存和价格（View）
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> LoadingInventoryAndPrices(string id) => View(await HotelApp.LoadingInventoryAndPricesViewAsync(id.ToLong()));

}
