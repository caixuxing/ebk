using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using YueJia.Ebk.Application.Contracts.UserApp;
using YueJia.Ebk.Application.Contracts.UserApp.Dto;
using YueJia.Ebk.Application.Contracts.UserApp.Query;
using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.Shared.Const;
using YueJia.Ebk.Domain.Shared.Utils;

namespace YueJia.Ebk.Application.UserApp
{
    public class AuthApp : ApplicationService, IAuthApp
    {

        private ISimpleClient<CompanyDO> CompanyRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<CompanyDO>>();
        private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

        public async Task<ClaimsPrincipal> LoginAsync(LoginQry qry)
        {

            await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<LoginQry>>().ValidateAndThrowAsync(qry);

            UserDto data = new UserDto();
            if (qry.UserName.Equals("admin"))
            {
                data = new UserDto() { Id = 11360847, UserName = "admin", UserPwd = EncryptUtils.MD5Encrypt("123456") };
            }
            else
            {
                var result = await db.Queryable<CompanyDO>().SingleAsync(c => c.ContactPhone == qry.UserName && !c.IsDelete);
                if (result is not null) data = new UserDto() { Id = result.Id, UserName = result.ContactPhone, UserPwd = result.Password };
            }
            if (data is null || !data.UserPwd.Equals(EncryptUtils.MD5Encrypt(qry.PassWord)))
                throw new InvalidOperationException("用户名或密码错误");
            var claims = new List<Claim>{
              new (ClaimAttributes.UserId, data.Id.ToString()),
              new (ClaimAttributes.UserName, data.UserName),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
