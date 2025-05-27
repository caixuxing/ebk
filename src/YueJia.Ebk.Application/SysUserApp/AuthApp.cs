using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.Contracts.SysUserApp.Query;
using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.Dept;
using YueJia.Ebk.Domain.Shared.Const;
using YueJia.Ebk.Domain.SysUser;

namespace YueJia.Ebk.Application.SysUserApp;

public class AuthApp : ApplicationService, IAuthApp
{

    private ISimpleClient<SysUserDo> SysUserRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<SysUserDo>>();
    private ISqlSugarClient Db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

    private ISimpleClient<CompanyDO> CompanyRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyDO>>();

    /*
     *约束
     *1）：用户状态
     *2）：公司状态
     */
    private ISimpleClient<DepartmentDo> DepartmentRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<DepartmentDo>>();

    public async Task<ClaimsPrincipal> LoginAsync(LoginQry qry)
    {

        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<LoginQry>>().ValidateAndThrowAsync(qry);
        var data = await SysUserRepo.AsQueryable().SingleAsync(x => x.AccountName == qry.UserName);
        if (data is null || !data.Password.Equals(EncryptUtils.MD5Encrypt(qry.PassWord)))
            throw new InvalidOperationException("用户名或密码错误");

        if (data.IsEnabled == YesOrNoType.No)
        {
            throw new InvalidOperationException("用户状态无效");
        }

        var CompanyObj = await CompanyRepo.AsQueryable().SingleAsync(vv => vv.TenantId == data.TenantId && vv.Status == YesOrNoType.Yes);
        if (data.AccountType != AccountTypeEnum.SuperAdmin && CompanyObj == null)
        {
            throw new InvalidOperationException("用户公司无效");
        }

        var company = await CompanyRepo.GetSingleAsync(x => x.TenantId == data.TenantId);
        var claims = new List<Claim>{
          new (ClaimAttributes.UserId, data?.Id.ToString()??string.Empty),
          new (ClaimAttributes.UserName, data?.AccountName??string.Empty),
          new (ClaimAttributes.TenantId, data?.TenantId?.ToString()??string.Empty),
          new (ClaimAttributes.AccountType,data?.AccountType.GetHashCode().ToString()??string.Empty),
          new (ClaimAttributes.CompanyId,company?.Id.ToString()??""),
          new (ClaimAttributes.DeptId, data?.DeptId.ToString()??string.Empty)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        return principal;
    }
}
