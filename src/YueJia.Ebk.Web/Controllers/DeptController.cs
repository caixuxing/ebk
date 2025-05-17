using YueJia.Ebk.Application.Contracts.DeptApp;
using YueJia.Ebk.Application.Contracts.DeptApp.Query;

namespace YueJia.Ebk.Web.Controllers
{
    public class DeptController : AbpController
    {

        private IDeptApp DeptApp => LazyServiceProvider.LazyGetRequiredService<IDeptApp>();


        public IActionResult Index() => View();




        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="requestQry"></param>
        /// <returns></returns>
        [HttpPost, Route("[controller]/PageList")]
        public async Task<IResult> GetPageList([FromBody] DeptPageListQry requestQry) => ApiResult.HandleResult(await DeptApp.GetPageListDeptAsync(requestQry));

    }
}
