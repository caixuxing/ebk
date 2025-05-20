using YueJia.Ebk.Application.Contracts.CompanyApp.Commands;
using YueJia.Ebk.Application.Contracts.CompanyApp.Dto;

namespace YueJia.Ebk.Application.Contracts.CompanyApp;

public interface ICompanyChannelMapApp
{

    /// <summary>
    /// 分配渠道
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<bool> AssignChannelAsync(AssignChannelCmd cmd, long companyId);


    /// <summary>
    /// 按公司ID获取分配渠道详情
    /// </summary>
    /// <param name="companyId"></param>
    /// <returns></returns>
    Task<AssignChannelDetailsDto> GetAssignChannelDetailsAsync(long companyId);
}
