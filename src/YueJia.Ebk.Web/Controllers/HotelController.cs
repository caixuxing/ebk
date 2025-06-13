using SqlSugar;
using YueJia.Ebk.Application.Contracts.Comm.BaseObj;
using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;
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


    #region 用户酒店
    /// <summary>
    /// 用户酒店
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> UserHotelMgr()
    {
        ViewBag.CountryList = await YueJiaSysServiceApp.GetDropDownCountryListAsync();
        ViewBag.HotelSaleTypeData = JsonConvert.SerializeObject(SysEnumApp.GetEnumDataList(nameof(HotelSaleTypeEnum)), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        return View();
    }

    /// <summary>
    /// 用户酒店列表
    /// </summary>
    /// <param name="requestQry"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/UserHotelPage")]
    public async Task<IResult> UserHotelPage([FromBody] HotelPublishPageFilterQry requestQry) => ApiResult.HandleResult(await HotelPublishApp.GetMyHotelPublishPageListAsync(requestQry));

    /// <summary>
    /// 批量上架
    /// </summary>
    /// <param name="userHotelIds"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/UserHotelBatchUp")]
    public async Task<IResult> UserHotelBatchUp([FromBody] List<string> userHotelIds)
    {
        return ApiResult.HandleBoolResult(await HotelApp.BatchUpdateHotelState(userHotelIds, HotelSaleTypeEnum.Up));
    }

    /// <summary>
    /// 批量下架
    /// </summary>
    /// <param name="userHotelIds"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/UserHotelBatchDown")]
    public async Task<IResult> UserHotelBatchDown([FromBody] List<string> userHotelIds)
    {
        return ApiResult.HandleBoolResult(await HotelApp.BatchUpdateHotelState(userHotelIds, HotelSaleTypeEnum.Down));
    }

    /// <summary>
    /// 切换酒店状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/UserHotelChangeState/{id}")]
    public async Task<IResult> UserHotelChangeState(string id) => ApiResult.HandleBoolResult(await HotelApp.UpdateHotelState(id));

    /// <summary>
    /// 酒店删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/UserHotelDelete/{id}")]
    public async Task<IResult> UserHotelDelete(string id) => ApiResult.HandleBoolResult(await HotelApp.UserHotelDelete(id));


    /// <summary>
    /// 用户添加酒店
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> AddUserHotelMgr()
    {

        ViewBag.CountryList = await YueJiaSysServiceApp.GetDropDownCountryListAsync();
        return View();
    }

    /// <summary>
    /// 酒店列表
    /// </summary>
    /// <param name="requestQry"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/GetBaseHotelPageList")]
    public async Task<IResult> GetBaseHotelPageList([FromBody] HotelPageListFilterQry requestQry) => ApiResult.HandleResult(await YueJiaSysServiceApp.GetHotelPageListAsync(requestQry));


    /// <summary>
    /// 添加用户酒店
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/AddUserHotel")]
    public async Task<IResult> AddUserHotel([FromBody] CreateOrUpHotelPublishCmd cmd) => ApiResult.HandleBoolResult(await HotelPublishApp.PublishHotelAsync(cmd));

    /// <summary>
    /// 酒店详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> UserHotelDetails(string id)
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
    /// 更新用户酒店
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut, Route("[controller]/{id}/UpdateUserHotel")]
    public async Task<IResult> UpdateUserHotel([FromBody] CreateOrUpHotelPublishCmd cmd, [FromRoute] string id) => ApiResult.HandleBoolResult(await HotelPublishApp.UpdatePublishHotelAsync(cmd, id.ToLong()));


    #endregion

    /// <summary>
    /// 用户酒店导航
    /// </summary>
    /// <returns></returns>
    public IActionResult UserHotelNavigationMgr()
    {
        return View();
    }


    #region 用户房间 / 价格计划
    /// <summary>
    /// 房间与价格计划（View）
    /// </summary>
    /// <param name="id">酒店唯一标识</param>
    /// <returns></returns>
    public async Task<IActionResult> UserRoomAndPlanMgr(string id)
    {
        var hotel = await HotelPublishApp.GetHotelPublishDetailAsync(id.ToLong());
        ViewBag.mv = new ViewHotelVo()
        {
            Id = id,
            HotelName = hotel.HotelName,
        };
        return View();
    }


    /// <summary>
    /// 房间与价格计划列表
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("[controller]/GetUserRoomAndPlan/{id}")]
    public async Task<IResult> GetUserRoomAndPlan([FromRoute] string id)
    {
        var roomAndPricePlan = await HotelApp.GetHotelRoomListByIdAsync(id.ToLong());
        return ApiResult.HandleResult(roomAndPricePlan.Select(x => new
        {
            Id = x.Id.ToString(),
            x.RoomType,
            x.HotelRoomTitle,
            x.BedType,
            x.BedTypeName,
            x.MaximumNumberOfPeople,
            x.AdultLimit,
            x.ChildLimit,
            x.IsEnabled,
            x.IsEnabledName,
            x.PlanList,
            ShowContent = false,
            x.StartDateString,
            x.EndDateString,
            ShowFooter = true
        }));
    }


    /// <summary>
    /// 添加酒店房间(View)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> AddUserRoomMgr(string id)
    {
        var userHotel = await HotelPublishApp.GetHotelPublishDetailAsync(id.ToLong());
        ViewBag.HotelName = $"{userHotel.HotelName}({userHotel.HotelNameEn})";
        CreateHotelRoomCmd vm = new CreateHotelRoomCmd()
        {
            HotelId = id,
            HotelCode = userHotel.HotelCode,
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
    [HttpPost, Route("[controller]/AddUserRoom")]
    public async Task<IResult> AddUserRoom([FromBody] CreateHotelRoomCmd cmd) => ApiResult.HandleBoolResult(await HotelApp.AddHotelRoomAsync(cmd));


    /// <summary>
    /// 删除酒店房间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete, Route("[controller]/DeleteUserRoom/{id}")]
    public async Task<IResult> DeleteUserRoom([FromRoute] string id) => ApiResult.HandleResult(await HotelApp.DeleteHotelRoomAsync(id.ToLong()));

    /// <summary>
    /// 切换房间状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/ChangeUserRoomState/{id}")]
    public async Task<IResult> ChangeUserRoomState(string id) => ApiResult.HandleBoolResult(await HotelApp.UpdateRoomStateAsync(id.ToLong()));

    /// <summary>
    ///  新增价格计划（View）
    /// </summary>
    /// <param name="id">房间ID</param>
    /// <returns></returns>
    public async Task<IActionResult> AddUserPlanMgr(string id)
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
            BreakfastType = BreakfastTypeEnum.NoBreakfast,
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
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/AddUserPlan")]
    public async Task<IResult> AddUserPlan([FromBody] CreateOrUpdatePricePlanCmd cmd) => ApiResult.HandleBoolResult(await HotelApp.CreatePricePlanAsync(cmd));

    /// <summary>
    /// 删除价格计划
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete, Route("[controller]/DeleteUserPlan/{id}")]
    public async Task<IResult> DeleteUserPlan([FromRoute] string id) => ApiResult.HandleBoolResult(await HotelApp.DeletePricePlanAsync(id.ToLong()));

    /// <summary>
    /// 切换价格计划状态
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/ChangeUserPlanState/{id}")]
    public async Task<IResult> ChangeUserPlanState(string id) => ApiResult.HandleBoolResult(await HotelApp.UpdatePricePlanStateAsync(id.ToLong()));
    #endregion









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
    /// 保存库存和价格
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/SaveInventoryAndPrice")]
    public async Task<IResult> SaveInventoryAndPrice([FromBody] SaveInventoryAndPriceCmd cmd) => ApiResult.HandleBoolResult(await HotelApp.SaveInventoryAndPriceAsync(cmd));



    /// <summary>
    /// 加载库存和价格（View）
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> LoadingInventoryAndPrices(string id) => View(await HotelApp.LoadingInventoryAndPricesViewAsync(id.ToLong()));



    /// <summary>
    /// 保存加载库存和价格
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/SaveLoadingInventoryAndPrices")]
    public async Task<IResult> SaveLoadingInventoryAndPrices([FromBody] SaveLoadingInventoryAndPricesCmd cmd)
    {
        return ApiResult.HandleBoolResult(await HotelApp.SaveLoadingInventoryAndPricesAsync(cmd));
    }

    /// <summary>
    /// 按房间ID获取价格计划列表
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    [HttpGet, Route("[controller]/{roomId}/RoomPricePlanList")]
    public async Task<IResult> PricePlanListDataByRoomId([FromRoute] string roomId)
    {
        var result = await HotelApp.PricePlanListDataByRoomIdAsync(roomId.ToLong());
        return ApiResult.HandleResult(result);
    }




    #region 库存日历
    /// <summary>
    /// 库存日历
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> InventoryCalendarMgr(string userHotelId)
    {
        //加载酒店信息
        ViewBag.hotelModel = await HotelPublishApp.GetHotelPublishDetailAsync(Convert.ToInt64(userHotelId));
        ViewBag.mv = new ViewHotelVo()
        {
            Id = userHotelId,
            HotelName = (ViewBag.hotelModel as HotelPublishDetailDto).HotelName,
        };

        ViewBag.ebkOtaRoomList = await HotelApp.GetEbkOtaRoomList(Convert.ToInt64(userHotelId));
        return View();
    }

    [HttpGet, Route("[controller]/GetInventoryListResult")]
    public async Task<IResult> GetInventoryListResult(string userRoomId, int dateYear, int dateMonth)
    {
        return ApiResult.HandleResult(await HotelApp.GetInventoryList(Convert.ToInt64(userRoomId), dateYear, dateMonth));
    }

    [HttpPost, Route("[controller]/SaveInventoryCalendar")]
    public async Task<IResult> SaveInventoryCalendar([FromBody] SaveInventoryCmd qry)
    {
        return ApiResult.HandleResult(await HotelApp.SaveInventory(qry.EbkRoom, qry.DailyInventoryList));
    }
    #endregion

    #region 价格日历
    /// <summary>
    /// 价格日历
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> PriceCalendarMgr(string userHotelId)
    {
        //加载酒店信息
        ViewBag.hotelModel = await HotelPublishApp.GetHotelPublishDetailAsync(Convert.ToInt64(userHotelId));
        ViewBag.mv = new ViewHotelVo()
        {
            Id = userHotelId,
            HotelName = (ViewBag.hotelModel as HotelPublishDetailDto).HotelName,
        };
        ViewBag.ebkOtaPricePlanList = await HotelApp.GetEbkPricePlanList(Convert.ToInt64(userHotelId));
        return View();
    }

    [HttpGet, Route("[controller]/GetPriceListResult")]
    public async Task<IResult> GetPriceListResult(string userPlanId, int dateYear, int dateMonth)
    {
        return ApiResult.HandleResult(await HotelApp.GetPriceList(Convert.ToInt64(userPlanId), dateYear, dateMonth));
    }



    [HttpPost, Route("[controller]/SavePriceCalendar")]
    public async Task<IResult> SavePriceCalendar([FromBody] SavePriceCmd qry)
    {

        return ApiResult.HandleResult(await HotelApp.SavePrice(qry.userPlanId, qry.priceList));
    }
    #endregion

    #region 批量编辑库存与价格

    /// <summary>
    /// 批量编辑库存与价格
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> BatchEditInventoryAndPricesMgr(string userHotelId)
    {
        //加载酒店信息
        ViewBag.hotelModel = await HotelPublishApp.GetHotelPublishDetailAsync(Convert.ToInt64(userHotelId));
        ViewBag.mv = new ViewHotelVo()
        {
            Id = userHotelId,
            HotelName = $@"{(ViewBag.hotelModel as HotelPublishDetailDto).HotelNameEn} [{(ViewBag.hotelModel as HotelPublishDetailDto).HotelName}]"
        };
        ViewBag.userRoomList = await HotelApp.GetHotelRoomListByIdAsync(userHotelId.ToLong());
        return View();
    }


    public async Task<IResult> BatchSaveInventoryAndPricesSimple([FromBody] BatchEditInventoryAndPricesModel qry)
    {

        return ApiResult.HandleResult(await HotelApp.BatchSaveInventoryAndPricesSimple(qry));
    }



    public async Task<IResult> BatchSaveInventoryAndPricesSenior([FromBody] BatchEditInventoryAndPricesSeniorModel qry)
    {

        return ApiResult.HandleResult(await HotelApp.BatchSaveInventoryAndPricesSenior(qry));
    }

    #endregion



}



















///// <summary>
///// 酒店发布详情
///// </summary>
///// <param name="id"></param>
///// <returns></returns>
//[HttpGet, Route("[controller]/{id}/Detail")]
//public async Task<IResult> GetHotelPublishDetail([FromRoute] string id) => ApiResult.HandleResult(await HotelPublishApp.GetHotelPublishDetailAsync(id.ToLong()));


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