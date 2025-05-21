using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YueJia.Ebk.Application.Contracts.DeptApp;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.Contracts.SysUserApp.Commands;
using YueJia.Ebk.Application.Contracts.SysUserApp.Query;
using YueJia.Ebk.Domain.Shared.Utils;

namespace YueJia.Ebk.Web.Controllers
{
    public class SysUserController : AbpController
    {

        private ISysUserApp SysUserApp => LazyServiceProvider.LazyGetRequiredService<ISysUserApp>();

        private IDeptApp DeptApp => LazyServiceProvider.LazyGetRequiredService<IDeptApp>();


        public async Task<IActionResult> Index()
        {
            ViewBag.DeptData = JsonConvert.SerializeObject(await DeptApp.GetDeptTreeSelectData(), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return View();
        }


        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="requestCmd"></param>
        /// <returns></returns>
        [HttpPost, Route("[controller]/create")]
        public async Task<IResult> Create([FromBody] CreateOrUpdateSysUserCmd requestCmd)
        {
            var result = await SysUserApp.CreateAsync(requestCmd);
            return ApiResult.HandleLongResult(result);
        }


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("[controller]/{id}/delete")]
        public async Task<IResult> Delete([FromRoute] string id)
        {
            var result = await SysUserApp.DeleteAsync(id.ToLong());
            return ApiResult.HandleBoolResult(result);

        }


        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestCmd"></param>
        /// <returns></returns>
        [HttpPut, Route("[controller]/{id}/update")]
        public async Task<IResult> Update([FromRoute] string id, [FromBody] CreateOrUpdateSysUserCmd requestCmd)
        {
            var result = await SysUserApp.UpdateAsync(requestCmd, id.ToLong());
            return ApiResult.HandleBoolResult(result);
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("[controller]/{id}/details")]
        public async Task<IResult> GetById([FromRoute] string id)
        {
            var result = await SysUserApp.GetByIdAsync(id.ToLong());
            return ApiResult.HandleResult(result);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="requestQry"></param>
        /// <returns></returns>
        [HttpPost, Route("[controller]/PageList")]
        public async Task<IResult> GetPageList([FromBody] SysUserPageFilterQry requestQry) => ApiResult.HandleResult(await SysUserApp.GetPageListAsync(requestQry));
    }
}
