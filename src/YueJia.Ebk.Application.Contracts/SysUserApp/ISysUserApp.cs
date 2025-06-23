using YueJia.Ebk.Application.Contracts.SysUserApp.Commands;
using YueJia.Ebk.Application.Contracts.SysUserApp.Dto;
using YueJia.Ebk.Application.Contracts.SysUserApp.Query;


namespace YueJia.Ebk.Application.Contracts.SysUserApp;

public interface ISysUserApp
{

    Task<long> CreateAsync(CreateOrUpdateSysUserCmd cmd);


    Task<bool> UpdateAsync(CreateOrUpdateSysUserCmd cmd, long id);


    Task<SysUserDetailsDto> GetByIdAsync(long id);

    Task<bool> DeleteAsync(long id);

    Task<bool> ResetPasswordAsync(long id);

    Task<bool> UpdatePasswordAsync(UpdatePasswordSysUserCmd requestCmd);

    Task<PageData<IEnumerable<SysUserPageListDto>>> GetPageListAsync(SysUserPageFilterQry qry);

    List<SysUserDetailsDto> GetManageUserList();

}
