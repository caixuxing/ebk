namespace YueJia.Ebk.Web.Controllers;

[Authorize]
public class HomeController : AbpController
{

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult ErrorMgr() {
        return View();
    }

}
