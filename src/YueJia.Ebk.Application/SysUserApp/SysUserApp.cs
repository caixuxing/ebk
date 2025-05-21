using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.Contracts.SysUserApp.Commands;
using YueJia.Ebk.Application.Contracts.SysUserApp.Dto;
using YueJia.Ebk.Application.Contracts.SysUserApp.Query;
using YueJia.Ebk.Domain.Dept;
using YueJia.Ebk.Domain.Shared.Enums;
using YueJia.Ebk.Domain.SysUser;

namespace YueJia.Ebk.Application.SysUserApp;

[DisableValidation]
public class SysUserApp : ApplicationService, ISysUserApp
{

    private ISimpleClient<SysUserDo> SysUserRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<SysUserDo>>();

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();


    public async Task<long> CreateAsync(CreateOrUpdateSysUserCmd cmd)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpdateSysUserCmd>>().ValidateAndThrowAsync(cmd);
        if (await SysUserRepo.IsAnyAsync(x => x.ContactPhone == cmd.ContactPhone)) throw new InvalidOperationException("联系电话已存在！");
        if (await SysUserRepo.IsAnyAsync(x => x.AccountName == cmd.AccountName)) throw new InvalidOperationException("账户已存在！");
        var entity = SysUserDo.Create(cmd.AccountName, cmd.RealName, AccountTypeEnum.NormalUser, cmd.IsEnabled, cmd.DeptId?.ToLong(), cmd.ContactPhone) ?? throw new InvalidOperationException("系统用户创建失败！");

        return await SysUserRepo.InsertReturnSnowflakeIdAsync(entity);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await SysUserRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"系统用户ID:{id}资源不存在！");
        var delEntity = entity with { };
        delEntity.IsDelete = true;
        if (entity.Equals(delEntity)) return true;
        var affectedRows = await SysUserRepo.AsUpdateable(delEntity)
         .UpdateColumns(it => new { it.IsDelete, it.Version, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime })
         .ExecuteCommandWithOptLockAsync();
        if (affectedRows is not 1) throw new InvalidOperationException($"用户删除失败!");
        return true;
    }

    public async Task<bool> UpdateAsync(CreateOrUpdateSysUserCmd cmd, long id)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpdateSysUserCmd>>().ValidateAndThrowAsync(cmd);
        if (await SysUserRepo.IsAnyAsync(x => x.ContactPhone == cmd.ContactPhone && x.Id != id)) throw new InvalidOperationException("联系电话已存在！");
        if (await SysUserRepo.IsAnyAsync(x => x.AccountName == cmd.AccountName && x.Id != id)) throw new InvalidOperationException("账户已存在！");
        var entity = await SysUserRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"用户ID:{id}资源不存在！");
        var upEntity = entity with { };
        upEntity.SetAccountName(cmd.AccountName)
             .SetRealName(cmd.RealName)
             .SetContactPhone(cmd.ContactPhone)
             .SetDeptId(cmd.DeptId?.ToLong())
             .SetIsEnabled(cmd.IsEnabled);

        if (entity.Equals(upEntity)) return true;

        var affectedRows = await SysUserRepo.AsUpdateable(upEntity).ExecuteCommandWithOptLockAsync();
        if (affectedRows is not 1) throw new InvalidOperationException($"用户信息更新失败!");
        return true;
    }

    public async Task<bool> UpdatePassWordAsync(long id, string oldPassword, string newPassword)
    {
        //读取公司原始信息
        var entity = await SysUserRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"用户ID:{id}资源不存在！");
        if (EncryptUtils.MD5Encrypt(oldPassword) != entity.Password) throw new InvalidOperationException($"旧密码错误!");
        entity.SetPassword(EncryptUtils.MD5Encrypt(newPassword));
        var affectedRows = await SysUserRepo.AsUpdateable(entity).ExecuteCommandWithOptLockAsync();
        if (affectedRows is not 1) throw new InvalidOperationException($"用户信息更新失败!");
        return true;
    }

    public async Task<PageData<IEnumerable<SysUserPageListDto>>> GetPageListAsync(SysUserPageFilterQry qry)
    {
        RefAsync<int> total = 0;

        var query = SysUserRepo.AsQueryable()
              .LeftJoin<DepartmentDo>((t, t1) => t.DeptId == t1.Id).With(SqlWith.NoLock)
               .WhereIF(!string.IsNullOrWhiteSpace(qry.AccountName), x => SqlFunc.Like(x.AccountName, $"{qry.AccountName}%"))
               .WhereIF(!string.IsNullOrWhiteSpace(qry.RealName), x => SqlFunc.Like(x.RealName, $"{qry.RealName}%"))
               .WhereIF(qry.IsEnabled.HasValue, (t, t1) => t.IsEnabled.Equals(qry.IsEnabled))
               .Select((t, t1) => new SysUserPageListDto()
               {
                   Id = t.Id,
                   AccountName = t.AccountName,
                   RealName = t.RealName,
                   DeptId = t1.Id,
                   DeptName = t1.Name,
                   IsEnabled = t.IsEnabled,
                   CreateTime = t.CreateTime,
                   ContactPhone = t.ContactPhone ?? string.Empty

               });
        var data = await query.ToPageListAsync(qry.PageIndex, qry.PageSize, total);

        return new PageData<IEnumerable<SysUserPageListDto>>(total, qry.PageSize, qry.PageIndex, data);
    }

    public async Task<SysUserDetailsDto> GetByIdAsync(long id)
    {
        var entity = await SysUserRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"用户ID:{id}资源不存在！");

        return new SysUserDetailsDto()
        {
            Id = entity.Id,
            AccountName = entity.AccountName,
            RealName = entity.RealName,
            IsEnabled = entity.IsEnabled,
            ContactPhone = entity.ContactPhone,
            DeptId = entity.DeptId,
        };
    }
}
