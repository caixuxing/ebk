using Newtonsoft.Json;
using YueJia.Ebk.Application.Contracts.DeptApp;
using YueJia.Ebk.Application.Contracts.DeptApp.Commands;
using YueJia.Ebk.Application.Contracts.DeptApp.Dto;
using YueJia.Ebk.Application.Contracts.DeptApp.Query;
using YueJia.Ebk.Domain.Shared.Utils;

namespace YueJia.Ebk.Web.Controllers
{
    public class DeptController : AbpController
    {

        private IDeptApp DeptApp => LazyServiceProvider.LazyGetRequiredService<IDeptApp>();


        public IActionResult Index() => View();


        /// <summary>
        /// 新增或编辑部门
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AddOrEditDept(string? id, string? parentDeptId, string? deptName, string? companyId)
        {
            DeptDetailsDto model = new DeptDetailsDto();
            if (!string.IsNullOrWhiteSpace(parentDeptId) && long.TryParse(parentDeptId, out long outParentDeptId)) model.ParentDeptId = outParentDeptId;
            if (!string.IsNullOrWhiteSpace(deptName)) model.DeptName = deptName;
            if (!string.IsNullOrWhiteSpace(companyId) && long.TryParse(companyId, out long outCompanyId)) model.CompanyId = outCompanyId;
            if (!string.IsNullOrWhiteSpace(id) && long.TryParse(id, out long deptId)) model = await DeptApp.GetDeptById(deptId);
            ViewBag.DeptData = JsonConvert.SerializeObject(await DeptApp.GetTopLevelDeptData());
            return View(model);
        }
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="requestQry"></param>
        /// <returns></returns>
        [HttpPost, Route("[controller]/PageList")]
        public async Task<IResult> GetPageList([FromBody] DeptPageListQry requestQry) => ApiResult.HandleResult(await DeptApp.GetPageListDeptAsync(requestQry));


        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="requestCmd"></param>
        /// <returns></returns>
        [HttpPost, Route("[controller]/create")]
        public async Task<IResult> CreateCompany([FromBody] CreateOrUpdateDeptCmd requestCmd) => ApiResult.HandleLongResult(await DeptApp.CreateDeptAsync(requestCmd));


        /// <summary>
        /// 按Id删除部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("[controller]/{id}/delete")]
        public async Task<IResult> DeleteCompany([FromRoute] string id) => ApiResult.HandleBoolResult(await DeptApp.DeleteDeptAsync(id.ToLong()));


        /// <summary>
        /// 按ID更新部门
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestCmd"></param>
        /// <returns></returns>
        [HttpPut, Route("[controller]/{id}/update")]
        public async Task<IResult> UpdateCompany([FromRoute] string id, [FromBody] CreateOrUpdateDeptCmd requestCmd) => ApiResult.HandleBoolResult(await DeptApp.UpdateDeptAsync(requestCmd, id.ToLong()));

    }
}
