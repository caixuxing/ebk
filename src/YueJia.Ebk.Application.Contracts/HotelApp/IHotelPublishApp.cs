using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
using YueJia.Ebk.Domain.Shared.Response;

namespace YueJia.Ebk.Application.Contracts.HotelApp;

public interface IHotelPublishApp
{

    /// <summary>
    /// 添加发布酒店
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<long> PublishHotelAsync(CreateOrUpHotelPublishCmd cmd);



    /// <summary>
    ///更新发布酒店
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<bool> UpdatePublishHotelAsync(CreateOrUpHotelPublishCmd cmd, long id);


    /// <summary>
    /// 发布酒店详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<HotelPublishDetailDto> GetHotelPublishDetailAsync(long id);


    /// <summary>
    /// 获取我的酒店分页列表
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    Task<PageData<IEnumerable<HotelPublishPageListDto>>> GetMyHotelPublishPageListAsync(HotelPublishPageFilterQry qry);
}


