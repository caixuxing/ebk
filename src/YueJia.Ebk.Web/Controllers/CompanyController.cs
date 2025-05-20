using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using YueJia.Ebk.Application.Contracts.CompanyApp;
using YueJia.Ebk.Application.Contracts.CompanyApp.Commands;
using YueJia.Ebk.Application.Contracts.CompanyApp.Query;
using YueJia.Ebk.Application.Contracts.SysApp;
using YueJia.Ebk.Domain.Shared.Enums;
using YueJia.Ebk.Domain.Shared.Utils;
using YueJia.Ebk.Web.ViewModels.Company;

namespace YueJia.Ebk.Web.Controllers;


[Authorize]
public class CompanyController : AbpController
{

    private ICompanyApp CompanyApp => LazyServiceProvider.LazyGetRequiredService<ICompanyApp>();

    private ISysEnumApp SysEnumApp => LazyServiceProvider.LazyGetRequiredService<ISysEnumApp>();

    private ICompanyChannelMapApp CompanyChannelMapApp => LazyServiceProvider.LazyGetRequiredService<ICompanyChannelMapApp>();

    /// <summary>
    /// 公司View
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> AssignChannel(long id)
    {
        var result = await CompanyChannelMapApp.GetAssignChannelDetailsAsync(id);
        AssignChannelVo modes = new()
        {
            CompanyId = result.CompanyId,
            CompanyName = result.CompanyName,
            SelectedChannelJosn = JsonConvert.SerializeObject(result.ChannelData),
            ChannelDataJosn = JsonConvert.SerializeObject(SysEnumApp.GetEnumDataList(nameof(ChannelType)))
        };
        return View(modes);
    }


    /// <summary>
    /// 分配渠道
    /// </summary>
    /// <param name="requestCmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/{id}/AssignChannel")]
    public async Task<IResult> AssignChannel([FromBody] AssignChannelCmd requestCmd, [FromRoute] string id)
    {
        var result = await CompanyChannelMapApp.AssignChannelAsync(requestCmd, id.ToLong());
        return ApiResult.HandleBoolResult(result);
    }



    /// <summary>
    /// 创建公司
    /// </summary>
    /// <param name="requestCmd"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/create")]
    public async Task<IResult> CreateCompany([FromBody] CreateOrUpdateCommpanyCmd requestCmd)
    {
        var result = await CompanyApp.CreateCompanyAsync(requestCmd);
        return ApiResult.HandleLongResult(result);
    }


    /// <summary>
    /// 删除公司
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete, Route("[controller]/{id}/delete")]
    public async Task<IResult> DeleteCompany([FromRoute] string id)
    {
        var result = await CompanyApp.DeleteCompanyAsync(id.ToLong());
        return ApiResult.HandleBoolResult(result);

    }



    /// <summary>
    /// 更新公司
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestCmd"></param>
    /// <returns></returns>
    [HttpPut, Route("[controller]/{id}/update")]
    public async Task<IResult> UpdateCompany([FromRoute] string id, [FromBody] CreateOrUpdateCommpanyCmd requestCmd)
    {
        var result = await CompanyApp.UpdateCompanyAsync(requestCmd, id.ToLong());
        return ApiResult.HandleBoolResult(result);
    }




    /// <summary>
    /// 获取公司详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("[controller]/{id}/details")]
    public async Task<IResult> GetCompanyById([FromRoute] string id)
    {
        var result = await CompanyApp.GetCompanyById(id.ToLong());
        return ApiResult.HandleResult(result);
    }




    /// <summary>
    /// 获取公司列表
    /// </summary>
    /// <param name="requestQry"></param>
    /// <returns></returns>
    [HttpPost, Route("[controller]/PageList")]
    public async Task<IResult> GetPageList([FromBody] CompanyPageListQry requestQry) => ApiResult.HandleResult(await CompanyApp.GetPageListCompanyAsync(requestQry));


}
