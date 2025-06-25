namespace YueJia.Ebk.Web.Controllers
{


    /// <summary>
    /// Logs
    /// </summary>

    [Authorize]
    public class LogsController : Controller
    {
        /// <summary>
        /// 客户操作酒店日志（View）
        /// </summary>
        /// <returns></returns>
        public IActionResult CustoLogsMgr() => View();
        /// <summary>
        /// 客户操作酒店日志
        /// </summary>
        /// <returns></returns>
        public async Task<IResult> CustoLogsPageList()
        {
            await Task.Delay(1);
            return ApiResult.HandleResult(true);
        }

    }
}
