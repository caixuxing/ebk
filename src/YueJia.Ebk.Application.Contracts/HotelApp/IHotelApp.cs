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
    Task<bool> UpdatePricePlanStateAsync( long id);
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

}
