using YueJia.Ebk.Application.Contracts.OuterServiceApp.Dto;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Qry;

namespace YueJia.Ebk.Application.Contracts.OuterServiceApp
{
    public interface IYueJiaSysServiceApp
    {
        /// <summary>
        /// 获取下拉国家集合列表
        /// </summary>
        /// <returns></returns>
        Task<List<SelectDataDto<int>>> GetDropDownCountryListAsync();

        /// <summary>
        /// 查询酒店列表
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        Task<PageData<IEnumerable<HotelPageListDto>>> GetHotelPageListAsync(HotelPageListFilterQry qry);

        /// <summary>
        /// 获取下拉房型集合列表
        /// </summary>
        /// <returns></returns>
        Task<List<SelectDataDto<string>>> GetDropDownRoomTypeByHotelCodeAsync(string hotelCode);


        Task<List<OtaRoomEntity>> GetOTARoomAsync(string hotelCode);

        
    }
}


