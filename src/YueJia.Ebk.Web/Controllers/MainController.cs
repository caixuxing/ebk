using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.Shared.Response;


namespace YueJia.Ebk.Web.Controllers
{
    /// <summary>
    /// Main
    /// </summary>
    [Authorize]
    public class MainController : AbpController
    {

        private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();



        public IActionResult Index()
        {
            ViewBag.MenuList = new MenuManage().UserMenuList(CurrentUserApp.AccountType.Value);
            return View();
        }


        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("[controller]/logout")]
        public async Task<IResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return R.Ok(ServiceResult<string>.Success("/"));
        }
    }
}
