using Microsoft.AspNetCore.Authorization;

namespace YueJia.Ebk.Web.Controllers
{
    [Authorize]
    public class HomeController : AbpController
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
