using YueJia.Ebk.Application.Contracts.OuterServiceApp.Dto;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Qry;
using YueJia.Ebk.Domain.Shared.Response;

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
    }
}


