using YueJia.Ebk.Application.Contracts.CompanyApp;
using YueJia.Ebk.Application.Contracts.CompanyApp.Commands;
using YueJia.Ebk.Application.Contracts.CompanyApp.Dto;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.Utils;
using YueJia.Ebk.Domain.Company;


namespace YueJia.Ebk.Application.CompanyApp
{
    public class CompanyChannelMapApp : ApplicationService, ICompanyChannelMapApp
    {


        private ISimpleClient<CompanyChannelMapDo> CompanyChannelMapRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyChannelMapDo>>();

        private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

        private ISimpleClient<CompanyDO> CompanyRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyDO>>();

        private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();


        public async Task<bool> AssignChannelAsync(AssignChannelCmd cmd, long companyId)
        {



            var entityOld = CompanyChannelMapRepo.AsQueryable().Where(e => e.CompanyId == companyId).ToList();

            var entityNew = await CompanyRepo.GetByIdAsync(companyId);

            var entity = cmd.ChannelData.Select(e => CompanyChannelMapDo.Create(
                companyId,
                e,
                !string.IsNullOrWhiteSpace(CurrentUserApp.TenantId) ? CurrentUserApp.TenantId.ToLong() : entityNew.TenantId.GetValueOrDefault()))
                .ToList();

            return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
            {
                await db.Deleteable(entityOld).ExecuteCommandAsync();

                await db.Insertable(entity).ExecuteReturnSnowflakeIdListAsync();

                return true;
            });
        }

        public async Task<AssignChannelDetailsDto> GetAssignChannelDetailsAsync(long companyId)
        {
            var entity = CompanyChannelMapRepo.AsQueryable().Where(e => e.CompanyId == companyId).ToList();
            var channels = entity.Select(e => e.ChannelId).ToList();
            var companyEntity = await CompanyRepo.GetByIdAsync(companyId);

            return new AssignChannelDetailsDto
            {
                CompanyId = companyId,
                CompanyName = companyEntity?.Name,
                ChannelData = channels
            };
        }
    }
}
