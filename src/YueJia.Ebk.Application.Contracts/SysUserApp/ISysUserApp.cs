using YueJia.Ebk.Application.Contracts.SysUserApp.Commands;
using YueJia.Ebk.Application.Contracts.SysUserApp.Dto;
using YueJia.Ebk.Application.Contracts.SysUserApp.Query;
using YueJia.Ebk.Domain.Shared.Response;

namespace YueJia.Ebk.Application.Contracts.SysUserApp;

public interface ISysUserApp
{

    Task<long> CreateAsync(CreateOrUpdateSysUserCmd cmd);


    Task<bool> UpdateAsync(CreateOrUpdateSysUserCmd cmd, long id);


    Task<SysUserDetailsDto> GetByIdAsync(long id);


    Task<bool> UpdatePassWordAsync(long id, string oldPassword, string newPassword);


    Task<bool> DeleteAsync(long id);


    Task<PageData<IEnumerable<SysUserPageListDto>>> GetPageListAsync(SysUserPageFilterQry qry);

}
