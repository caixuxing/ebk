using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;

namespace YueJia.Ebk.Application.Contracts.HotelApp;

/// <summary>
/// 酒店应用接口
/// </summary>
public interface IHotelApp
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


    /// <summary>
    /// 保存加载库存和价格
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<bool> SaveLoadingInventoryAndPricesAsync(LoadingInventoryAndPriceModel cmd);

    /// <summary>
    /// 按房间ID获取价格计划列表
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    Task<LoadingInventoryAndPriceModel> PricePlanListDataByRoomIdAsync(string userRoomId);

    /// <summary>
    /// 保存库存和价格
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<bool> SaveInventoryAndPriceAsync(InventoryAndPriceDto cmd);

    /// <summary>
    /// 添加酒店房间
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<bool> AddHotelRoomAsync(CreateHotelRoomCmd cmd);


    /// <summary>
    /// 删除房间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteHotelRoomAsync(long id);

    /// <summary>
    /// 切换房间状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> UpdateRoomStateAsync(long id);



    /// <summary>
    /// 按酒店Id获取酒店房间与价格计划列表
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<List<HotelRoomListDto>> GetHotelRoomListByIdAsync(long id);


    /// <summary>
    /// 按酒店房间ID获取酒店房间详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<HotelRoomDetailsDto> GetHotelRoomByIdAsync(long id);

    /// <summary>
    /// 创建价格计划
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<bool> CreatePricePlanAsync(CreateOrUpdatePricePlanCmd cmd);

    /// <summary>
    /// 价格计划详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<PricePlanDetailDto> GetPricePlanDetailsByIdAsync(long id);

    /// <summary>
    /// 更新价格计划
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> UpdatePricePlanStateAsync(long id);
    /// <summary>
    /// 删除价格计划
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeletePricePlanAsync(long id);




    /// <summary>
    /// 查询库存和价格详情
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    Task<InventoryAndPriceDetailsDto> GetInventoryAndPriceDetailsByFilterAsync(InventoryAndPriceDetailsQry qry);


    /// <summary>
    /// 按酒店ID获取树形下拉数据
    /// </summary>
    /// <returns></returns>
    Task<List<TreeSelectDataDto<string>>> GetHoteTreeSelectDataByHotelIdAsync(long hotelId);



    /// <summary>
    /// 加载库存和价格数据
    /// </summary>
    /// <returns></returns>
    Task<LoadingInventoryAndPricesDto> LoadingInventoryAndPricesViewAsync(long hotelId);

    /// <summary>
    /// Ebk房间信息
    /// </summary>
    /// <returns></returns>
    Task<List<HotelRoomListDto>> GetEbkOtaRoomList(long userHotelId);

    /// <summary>
    /// 获取库存集合
    /// </summary>
    /// <returns></returns>
    Task<List<DailyInventoryModel>> GetInventoryList(long userRoomId, int dateYear, int dateMonth);




    /// <summary>
    /// 库存和价格
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    Task<InventoryAndPriceDto> InventoryAndPriceViewAsync(InventoryAndPriceDetailsQry qry);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    Task<bool> SaveInventory(HotelRoomListDto ebkRoom, List<DailyInventoryModel> dailyInventoryList);

    Task<List<PricePlanListDto>> GetEbkPricePlanList(long userHotelId);

    Task<List<DailyPriceModel>> GetPriceList(long userPlanId, int dateYear, int dateMonth);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    Task<bool> SavePrice(string userPlanId, List<DailyPriceModel> priceList);


    Task<bool> BatchSaveInventoryAndPricesSimple(BatchEditInventoryAndPricesModel qry);

    Task<bool> BatchSaveInventoryAndPricesSenior(BatchEditInventoryAndPricesSeniorModel qry);


    /// <summary>
    /// 切换酒店状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> UpdateHotelState(string userHotelId);


    /// <summary>
    /// 批量修改酒店状态
    /// </summary>
    /// <param name="userHotelIds"></param>
    /// <param name="newSaleType"></param>
    /// <returns></returns>
    Task<bool> BatchUpdateHotelState(List<string> userHotelIds, HotelSaleTypeEnum newSaleType);


    Task<bool> UserHotelDelete(string userHotelId);



    Task<bool> CopeUserPlan(CopeUserPlanModel cmd);

}
