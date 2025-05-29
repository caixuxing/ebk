using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;

namespace YueJia.Ebk.Application.Contracts.HotelApp;

public interface IHotelApp
{
    /// <summary>
    /// 添加酒店房间
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<long> AddHotelRoomAsync(CreateHotelRoomCmd cmd);


    /// <summary>
    /// 按酒店代码获取酒店房间与价格计划列表
    /// </summary>
    /// <param name="hotelCode"></param>
    /// <returns></returns>
    Task<List<HotelRoomListDto>> GetHotelRoomByHotelCodeAsync(string hotelCode);

}
