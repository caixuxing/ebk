using YueJia.Ebk.Application.Contracts.DeptApp;
using YueJia.Ebk.Application.Contracts.SysApp;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Application.Contracts.SysUserApp.Commands;
using YueJia.Ebk.Application.Contracts.SysUserApp.Dto;
using YueJia.Ebk.Application.Contracts.SysUserApp.Query;

namespace YueJia.Ebk.Web.Controllers;

/// <summary>
/// 系统用户管理
/// </summary>
[Authorize]
public class SysUserController : AbpController
{

    private ISysUserApp SysUserApp => LazyServiceProvider.LazyGetRequiredService<ISysUserApp>();

    private IDeptApp DeptApp => LazyServiceProvider.LazyGetRequiredService<IDeptApp>();

    private ISysEnumApp SysEnumApp => LazyServiceProvider.LazyGetRequiredService<ISysEnumApp>();


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        ViewBag.YesOrNoTypeList = SysEnumApp.GetEnumDataList(nameof(YesOrNoType));
        return View();
    }


    /// <summary>
    /// 新增编辑
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IActionResult> AddEditMgr(long id)
    {
        ViewBag.DeptData = (await DeptApp.GetPageListDeptAsync(new Application.Contracts.DeptApp.Query.DeptPageListQry() { PageIndex = 1, PageSize = int.MaxValue })).List.ToList();

        var model = new SysUserDetailsDto() { IsEnabled = YesOrNoType.Yes };
        if (id > 0)
        {
            model = await SysUserApp.GetByIdAsync(id);
            if (model == null)
            {
                return View("../Home/ErrorMgr");
            }
        }
        ViewBag.id = id;
        ViewBag.model = new CreateOrUpdateSysUserCmd()
        {
            AccountName = model.AccountName,
            RealName = model.RealName,
            DeptId = model.DeptId == null ? "0" : model.DeptId.ToString(),
            ContactPhone = model.ContactPhone,
            IsEnabled = model.IsEnabled,
            DeptAdmin = model.DeptAdmin,
            Email = model.Email,
        };
        return View();
    }


    /// <summary>
    /// 密码修改
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> UpdatePasswordMgr()
    {
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
    /// 重置密码
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut, Route("[controller]/{id}/resetPassword")]
    public async Task<IResult> ResetPassword([FromRoute] string id)
    {
        var result = await SysUserApp.ResetPasswordAsync(id.ToLong());
        return ApiResult.HandleBoolResult(result);
    }

    /// <summary>
    /// 密码修改
    /// </summary>
    /// <returns></returns>
    [HttpPut, Route("[controller]/updatePassword")]
    public async Task<IResult> UpdatePassword([FromBody] UpdatePasswordSysUserCmd requestCmd)
    {
        var result = await SysUserApp.UpdatePasswordAsync(requestCmd);
        return ApiResult.HandleBoolResult(result);
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="requestQry"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/PageList")]
    public async Task<IResult> GetPageList([FromBody] SysUserPageFilterQry requestQry) => ApiResult.HandleResult(await SysUserApp.GetPageListAsync(requestQry));
}

