using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.Contracts.SysUserApp.Query;

namespace YueJia.Ebk.Web.Controllers;

public class LoginController : AbpController
{
    private IAuthApp AuthApp => LazyServiceProvider.LazyGetRequiredService<IAuthApp>();

    public IActionResult Index()
    {
#if DEBUG
        ViewBag.UserNmae = "15580808032";//"admin";
        ViewBag.Password = "123456";
#endif
        return View();
    }


    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    [HttpPost, Route("login")]
    public async Task<IResult> Login([FromBody] LoginQry qry)
    {
        var Result = ApiResult.HandleResult("/", "登录失败");
        var LoginResult = await AuthApp.LoginAsync(qry);
        if (LoginResult is not null)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, LoginResult);
            Result = ApiResult.HandleResult("/Main/Index", "登录成功!");
        }
        return Result;
    }
}
