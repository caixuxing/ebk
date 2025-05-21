using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.Contracts.SysUserApp.Query;
using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.Shared.Const;
using YueJia.Ebk.Domain.SysUser;

namespace YueJia.Ebk.Application.SysUserApp
{
    public class AuthApp : ApplicationService, IAuthApp
    {

        private ISimpleClient<SysUserDo> SysUserRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<SysUserDo>>();
        private ISqlSugarClient Db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

        private ISimpleClient<CompanyDO> CompanyRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyDO>>();

        public async Task<ClaimsPrincipal> LoginAsync(LoginQry qry)
        {

            await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<LoginQry>>().ValidateAndThrowAsync(qry);
            var data = await SysUserRepo.AsQueryable().SingleAsync(x => x.AccountName == qry.UserName);
            if (data is null || !data.Password.Equals(EncryptUtils.MD5Encrypt(qry.PassWord)))
                throw new InvalidOperationException("用户名或密码错误");

            var company = await CompanyRepo.GetSingleAsync(x => x.TenantId == data.TenantId);
            var claims = new List<Claim>{
              new (ClaimAttributes.UserId, data.Id.ToString()),
              new (ClaimAttributes.UserName, data.AccountName),
              new (ClaimAttributes.TenantId, data.TenantId?.ToString()??string.Empty),
              new (ClaimAttributes.AccountType,data.AccountType.GetHashCode().ToString()),
              new (ClaimAttributes.CompanyId,company?.Id.ToString()??""),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
