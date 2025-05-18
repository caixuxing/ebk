using Microsoft.AspNetCore.Authorization;
using YueJia.Ebk.Application.Contracts.CompanyApp;
using YueJia.Ebk.Application.Contracts.CompanyApp.Commands;
using YueJia.Ebk.Application.Contracts.CompanyApp.Query;

namespace YueJia.Ebk.Web.Controllers;


[Authorize]
public class CompanyController : AbpController
{

    private ICompanyApp CompanyApp => LazyServiceProvider.LazyGetRequiredService<ICompanyApp>();


    /// <summary>
    /// 公司View
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AssignChannel() => View();



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
    public async Task<IResult> DeleteCompany([FromRoute] long id)
    {
        var result = await CompanyApp.DeleteCompanyAsync(id);
        return ApiResult.HandleBoolResult(result);

    }



    /// <summary>
    /// 更新公司
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestCmd"></param>
    /// <returns></returns>
    [HttpPut, Route("[controller]/{id}/update")]
    public async Task<IResult> UpdateCompany([FromRoute] long id, [FromBody] CreateOrUpdateCommpanyCmd requestCmd)
    {
        var result = await CompanyApp.UpdateCompanyAsync(requestCmd, id);
        return ApiResult.HandleBoolResult(result);
    }




    /// <summary>
    /// 获取公司详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("[controller]/{id}/details")]
    public async Task<IResult> GetCompanyById([FromRoute] long id)
    {
        var result = await CompanyApp.GetCompanyById(id);
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
