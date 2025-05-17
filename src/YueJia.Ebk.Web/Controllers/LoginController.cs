using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using YueJia.Ebk.Application.Contracts.UserApp;
using YueJia.Ebk.Application.Contracts.UserApp.Query;

namespace YueJia.Ebk.Web.Controllers
{
    public class LoginController : AbpController
    {


        private IAuthApp AuthApp => LazyServiceProvider.LazyGetRequiredService<IAuthApp>();

        public IActionResult Index()
        {
#if DEBUG
            ViewBag.UserNmae = "admin";
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
            var result = await AuthApp.LoginAsync(qry);
            if (result is not null)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result);
                return ApiResult.HandleResult("/Main/Index", "登录成功!");
            }
            return ApiResult.HandleResult("/", "登录失败");
        }
    }
}
