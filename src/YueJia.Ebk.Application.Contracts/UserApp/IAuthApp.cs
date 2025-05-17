using System.Security.Claims;
using YueJia.Ebk.Application.Contracts.UserApp.Query;

namespace YueJia.Ebk.Application.Contracts.UserApp;

public interface IAuthApp
{
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    Task<ClaimsPrincipal> LoginAsync(LoginQry qry);
}
