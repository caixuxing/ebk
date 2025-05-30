using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;

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
    Task<long> AddHotelRoomAsync(CreateHotelRoomCmd cmd);


    /// <summary>
    /// 删除房间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteHotelRoomAsync(long id);


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
    Task<long> CreatePricePlanAsync(CreateOrUpdatePricePlanCmd cmd);

    /// <summary>
    /// 更新价格计划
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> UpdatePricePlanAsync(CreateOrUpdatePricePlanCmd cmd, long id);
    /// <summary>
    /// 删除价格计划
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeletePricePlanAsync(long id);

}
