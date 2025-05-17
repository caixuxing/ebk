using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using YueJia.Ebk.Domain.Shared.Response;


namespace YueJia.Ebk.Web.Controllers
{

    [Authorize]
    public class MainController : AbpController
    {
        public IActionResult Index()
        {
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
