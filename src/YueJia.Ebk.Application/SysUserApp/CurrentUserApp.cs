using Microsoft.AspNetCore.Http;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.Contracts.SysUserApp.Dto;
using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.Dept;
using YueJia.Ebk.Domain.Shared.Const;
using YueJia.Ebk.Domain.SysUser;

namespace YueJia.Ebk.Application.SysUserApp;

public class CurrentUserApp : ApplicationService, ICurrentUserApp
{
    private readonly IHttpContextAccessor _accessor;

    private ISimpleClient<CompanyDO> CompanyRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyDO>>();


    public ISimpleClient<SysUserDo> SysUserRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<SysUserDo>>();

    private ISimpleClient<DepartmentDo> DepartmentRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<DepartmentDo>>();

    /// <summary>
    ///
    /// </summary>
    /// <param name="accessor"></param>
    public CurrentUserApp(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    /// <summary>
    /// 用户Id
    /// </summary>
    public virtual string Id
    {
        get
        {
            var id = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserId);
            return id?.Value ?? string.Empty;
        }
    }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName
    {
        get
        {
            var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserName);
            return name?.Value ?? string.Empty;
        }
    }

    /// <summary>
    /// 姓名
    /// </summary>
    public string UserNickName
    {
        get
        {
            var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserNickName);
            return name?.Value ?? string.Empty;
        }
    }

    /// <summary>
    /// 租户Id
    /// </summary>
    public string TenantId
    {
        get
        {
            var orgcode = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.TenantId);
            return orgcode?.Value ?? string.Empty;
        }
    }

    public AccountTypeEnum? AccountType
    {
        get
        {
            var accountType = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.AccountType);
            return (AccountTypeEnum)Enum.ToObject(typeof(AccountTypeEnum), int.Parse(accountType?.Value ?? AccountTypeEnum.NormalUser.GetHashCode().ToString()));

        }
    }

    private string CompanyId
    {
        get
        {
            var companyId = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.CompanyId);
            return companyId?.Value ?? string.Empty;
        }
    }
    private long? DeptId
    {
        get
        {
            var deptId = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.DeptId);
            return deptId?.Value.ToLong() ?? null;
        }
    }

    public CurrentAccountCompanyDto Company
    {

        get
        {

            if (this.AccountType != AccountTypeEnum.SuperAdmin)
            {
                long companyId = this.CompanyId.ToLong();
                var model = CompanyRepo.AsQueryable().With(SqlWith.NoLock).Single(x => x.Id == companyId);
                return new CurrentAccountCompanyDto()
                {
                    CompanyId = model?.Id.ToString(),
                    CompanyName = model?.Name
                };
            }
            return new();
        }
    }

    public CurrentAccountDeptDto Dept
    {
        get
        {
            if (this.AccountType != AccountTypeEnum.SuperAdmin)
            {
                var data = DepartmentRepo.AsQueryable().With(SqlWith.NoLock).Where(x => x.Id == DeptId || x.ParentId == DeptId).ToList();
                var first = data.FirstOrDefault(x => x.Id == DeptId);
                return new CurrentAccountDeptDto()
                {
                    DeptId = first?.Id,
                    DeptName = first?.Name,
                    ParentDeptId = first?.ParentId,
                    ChildDeptIds = data.Select(x => x.Id).ToList()
                };
            }
            return new();
        }

    }

    public bool IsDeptAdmin
    {
        get
        {
            long id = this.Id.ToLong();
            return SysUserRepo.AsQueryable().With(SqlWith.NoLock).Single(t => t.Id == id)?.DeptAdmin ?? false;
        }
    }
}
