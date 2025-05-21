using YueJia.Ebk.Application.Contracts.CompanyApp;
using YueJia.Ebk.Application.Contracts.CompanyApp.Commands;
using YueJia.Ebk.Application.Contracts.CompanyApp.Dto;
using YueJia.Ebk.Application.Contracts.CompanyApp.Query;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.Utils;
using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.Shared.Enums;
using YueJia.Ebk.Domain.SysUser;


namespace YueJia.Ebk.Application.CompanyApp;

[DisableValidation]
public class CompanyApp : ApplicationService, ICompanyApp
{

    private ISimpleClient<CompanyDO> CompanyRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyDO>>();

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

    private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();

    public async Task<long> CreateCompanyAsync([NotNull] CreateOrUpdateCommpanyCmd cmd)
    {
        //验证
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpdateCommpanyCmd>>().ValidateAndThrowAsync(cmd);


        if (CurrentUserApp.AccountType != AccountTypeEnum.SuperAdmin) throw new InvalidOperationException("当前用户无权创建公司！");


        //校验唯一性
        if (await CompanyRepo.IsAnyAsync(x => x.ContactPhone == cmd.ContactPhone)) throw new InvalidOperationException("公司联系电话已存在！");
        if (!string.IsNullOrWhiteSpace(cmd.Email))
            if (await CompanyRepo.IsAnyAsync(x => x.Email == cmd.Email)) throw new InvalidOperationException("公司邮箱已存在！");

        //创建公司
        var entity = CompanyDO.Create(name: cmd.Name,
                                    responsible: cmd.Responsible,
                                    contactPhone: cmd.ContactPhone,
                                    email: cmd.Email,
                                    companyAddr: cmd.CompanyAddr,
                                    isChannelManage: cmd.IsChannelManage,
                                    status: cmd.Status
                                    ) ?? throw new InvalidOperationException("公司创建失败！");

        // var DeptEntity = DepartmentDo.Create("默认部门", -1, entity.Id, YesOrNoType.Yes, entity.TenantId.GetValueOrDefault(0)) ?? throw new InvalidOperationException("部门创建失败！");



        var UserEntity = SysUserDo.Create(cmd.ContactPhone, cmd.Responsible, AccountTypeEnum.SysAdmin, YesOrNoType.Yes, null, cmd.ContactPhone);
        UserEntity.TenantId = entity.TenantId;


        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {

            await db.Insertable(entity).ExecuteCommandAsync();

            await db.Insertable(UserEntity).ExecuteCommandAsync();

            return entity.Id;
        });



    }



    public async Task<bool> DeleteCompanyAsync([NotNull] long id)
    {
        var entity = await CompanyRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"公司ID:{id}资源不存在！");
        var delEntity = entity with { };
        delEntity.IsDelete = true;
        if (entity.Equals(delEntity)) return true;
        var affectedRows = await CompanyRepo.AsUpdateable(entity)
         .UpdateColumns(it => new { it.IsDelete, it.Version, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime })
         .ExecuteCommandWithOptLockAsync();
        if (affectedRows is not 1) throw new InvalidOperationException($"公司信息删除失败!");
        return true;
    }

    public async Task<bool> UpdateCompanyAsync([NotNull] CreateOrUpdateCommpanyCmd cmd, [NotNull] long id)
    {
        //验证更新模型参数
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpdateCommpanyCmd>>().ValidateAndThrowAsync(cmd);

        //校验唯一性 排除自己
        if (await CompanyRepo.IsAnyAsync(x => x.ContactPhone == cmd.ContactPhone && x.Id != id)) throw new InvalidOperationException("公司联系电话已存在！");
        if (!string.IsNullOrWhiteSpace(cmd.Email))
            if (await CompanyRepo.IsAnyAsync(x => x.Email == cmd.Email && x.Id != id)) throw new InvalidOperationException("公司邮箱已存在！");

        //读取公司原始信息
        var entity = await CompanyRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"公司ID:{id}资源不存在！");
        //Copy更新信息
        var upEntity = entity with { };
        upEntity.SetName(cmd.Name)
             .SetResponsible(cmd.Responsible)
             .SetContactPhone(cmd.ContactPhone)
             .SetEmail(cmd.Email)
             .SetCompanyAddr(cmd.CompanyAddr)
             .SetIsChannelManage(cmd.IsChannelManage)
             .SetStatus(cmd.Status);
        //比较更新前后信息
        if (entity.Equals(upEntity)) return true;

        //指定更新列并返回受影响行数
        var affectedRows = await CompanyRepo.AsUpdateable(upEntity)
           .UpdateColumns(it => new
           {
               it.Name,
               it.Responsible,
               it.ContactPhone,
               it.Email,
               it.CompanyAddr,
               it.IsChannelManage,
               it.Status,
               it.Version,
               it.LastModifiedbyId,
               it.LastModifiedbyName,
               it.LastModifiedTime
           })
           .ExecuteCommandWithOptLockAsync();
        if (affectedRows is not 1) throw new InvalidOperationException($"用户信息更新失败!");
        return true;
    }

    public async Task<PageData<IEnumerable<CompanyPageListDto>>> GetPageListCompanyAsync([NotNull] CompanyPageListQry qry)
    {
        RefAsync<int> total = 0;

        var query = CompanyRepo.AsQueryable()
               .WhereIF(!string.IsNullOrWhiteSpace(qry.Name), x => SqlFunc.Like(x.Name, $"{qry.Name}%"))
               .WhereIF(!string.IsNullOrWhiteSpace(qry.Responsible), x => SqlFunc.Like(x.Name, $"{qry.Responsible}%"))
               .WhereIF(!string.IsNullOrWhiteSpace(qry.ContactPhone), x => x.ContactPhone.Equals(qry.ContactPhone))
               .WhereIF(qry.IsChannelManage.HasValue, x => x.IsChannelManage.Equals(qry.IsChannelManage))
               .WhereIF(qry.Status.HasValue, x => x.Status.Equals(qry.Status))
               .MapCompanyPageListAsync();
        var data = await query.ToPageListAsync(qry.PageIndex, qry.PageSize, total);

        return new PageData<IEnumerable<CompanyPageListDto>>(total, qry.PageSize, qry.PageIndex, data);

    }

    public Task<bool> IsContactPhoneExistAsync([NotNull] string contactPhone) => CompanyRepo.IsAnyAsync(x => x.ContactPhone == contactPhone);


    public Task<bool> IsEmailExistAsync([NotNull] string email) => CompanyRepo.IsAnyAsync(x => x.Email == email);


    public async Task<CompanyDetailsDto> GetCompanyById([NotNull] long id)
    {
        var entity = await CompanyRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"公司ID:{id}资源不存在！");

        return new CompanyDetailsDto()
        {
            Name = entity.Name,
            Responsible = entity.Responsible,
            ContactPhone = entity.ContactPhone,
            Email = entity.Email,
            CompanyAddr = entity.CompanyAddr,
            IsChannelManage = entity.IsChannelManage,
            Status = entity.Status
        };
    }



}

/// <summary>
/// Map扩展
/// </summary>
internal static partial class MapExt
{

    /// <summary>
    /// MapUserDtoList
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    internal static ISugarQueryable<CompanyPageListDto> MapCompanyPageListAsync(this ISugarQueryable<CompanyDO> entities)
    {
        var query = entities.Select(x => new CompanyPageListDto()
        {
            Name = x.Name,
            Responsible = x.Responsible,
            ContactPhone = x.ContactPhone,
            Email = x.Email,
            IsChannelManage = x.IsChannelManage,
            Status = x.Status,
            CreateTime = x.CreateTime,
            Id = x.Id
        });
        return query;
    }
}
