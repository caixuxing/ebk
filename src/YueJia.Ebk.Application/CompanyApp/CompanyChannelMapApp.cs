using YueJia.Ebk.Application.Contracts.CompanyApp;
using YueJia.Ebk.Application.Contracts.CompanyApp.Commands;
using YueJia.Ebk.Application.Contracts.CompanyApp.Dto;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.Company;


namespace YueJia.Ebk.Application.CompanyApp;

public class CompanyChannelMapApp : ApplicationService, ICompanyChannelMapApp
{


    private ISimpleClient<CompanyChannelMapDo> CompanyChannelMapRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyChannelMapDo>>();

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

    private ISimpleClient<CompanyDO> CompanyRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyDO>>();

    private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();

     
    public async Task<bool> AssignChannelAsync(AssignChannelCmd cmd, long companyId)
    {

        var entityOld = CompanyChannelMapRepo.AsQueryable().Where(e => e.CompanyId == companyId).ToList();

        var companyEntry = await CompanyRepo.GetByIdAsync(companyId);

        var newEntityList = cmd.SalePlatCodeList.Select(e => CompanyChannelMapDo.Create(
            companyId,
            e,
            companyEntry.TenantId.GetValueOrDefault())
            ).ToList();

        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            //删除
            await db.Deleteable<CompanyChannelMapDo>().Where(vv => vv.CompanyId == companyId).ExecuteCommandAsync();
            //新增
            await db.Insertable(newEntityList).ExecuteReturnSnowflakeIdListAsync();

            return true;
        });
    }

    public async Task<AssignChannelDetailsDto> GetAssignChannelDetailsAsync(long companyId)
    {
        var entity = CompanyChannelMapRepo.AsQueryable().Where(e => e.CompanyId == companyId).ToList();
        var companyEntity = await CompanyRepo.GetByIdAsync(companyId);

        return new AssignChannelDetailsDto
        {
            CompanyId = companyId,
            CompanyName = companyEntity?.Name,
            SalePlatCodeList = entity.Select(e => e.SalePlatCode.ToString()).ToList()
        };
    }
}
