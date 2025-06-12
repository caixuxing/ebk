using YueJia.Ebk.Application.Contracts.CompanyApp;
using YueJia.Ebk.Application.Contracts.CompanyApp.Commands;
using YueJia.Ebk.Application.Contracts.DeptApp;
using YueJia.Ebk.Application.Contracts.DeptApp.Commands;
using YueJia.Ebk.Application.Contracts.DeptApp.Dto;
using YueJia.Ebk.Application.Contracts.DeptApp.Query;
using YueJia.Ebk.Application.Contracts.SysApp;
using YueJia.Ebk.Application.Contracts.SysUserApp;

namespace YueJia.Ebk.Web.Controllers
{
    public class DeptController : AbpController
    {

        private IDeptApp DeptApp => LazyServiceProvider.LazyGetRequiredService<IDeptApp>();

        private ISysEnumApp SysEnumApp => LazyServiceProvider.LazyGetRequiredService<ISysEnumApp>();

        private ICurrentUserApp currentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();
        private ICompanyApp CompanyApp => LazyServiceProvider.LazyGetRequiredService<ICompanyApp>();


        /// <summary>
        /// 部门管理View
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            ViewBag.YesOrNoTypeList = SysEnumApp.GetEnumDataList(nameof(YesOrNoType));
            ViewBag.CompanyChannelManage = (await CompanyApp.GetCompanyById(currentUserApp.Company.CompanyId?.ToLong() ?? 0)).IsChannelManage == YesOrNoType.Yes;
            return View();
        }


        /// <summary>
        /// 新增编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddEditMgr(long id)
        {
            var model = new DeptDetailsDto() { Status = YesOrNoType.Yes, ParentDeptId = -1 };
            if (id > 0)
            {
                model = await DeptApp.GetDeptById(id);
                if (model == null)
                {
                    return View("../Home/ErrorMgr");
                }
            }
            ViewBag.id = id;
            ViewBag.model = new CreateOrUpdateDeptCmd()
            {
                Name = model.DeptName,
                Status = model.Status,
                ParentDeptId = model.ParentDeptId.ToString() ?? string.Empty
            };
            return View();
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



        /// <summary>
        /// 分配销售渠道
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> AssignChannelMgr(long id)
        {
            var entry = await DeptApp.GetAssignChannelDetailsAsync(id);
            return View(entry);
        }

        /// <summary>
        /// 分配渠道
        /// </summary>
        /// <param name="requestCmd"></param>
        /// <returns></returns>
        [HttpPost, Route("[controller]/{id}/HandleAssignChannel")]
        public async Task<IResult> HandleAssignChannel([FromBody] AssignChannelCmd requestCmd, [FromRoute] string id)
        {
            var result = await DeptApp.AssignChannelAsync(requestCmd.SalePlatCodeList, id.ToLong());
            return ApiResult.HandleBoolResult(result);
        }



    }
}

