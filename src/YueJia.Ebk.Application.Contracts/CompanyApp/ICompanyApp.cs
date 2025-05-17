using System.Diagnostics.CodeAnalysis;
using YueJia.Ebk.Application.Contracts.CompanyApp.Commands;
using YueJia.Ebk.Application.Contracts.CompanyApp.Dto;
using YueJia.Ebk.Application.Contracts.CompanyApp.Query;
using YueJia.Ebk.Domain.Shared.Response;

namespace YueJia.Ebk.Application.Contracts.CompanyApp;

public interface ICompanyApp
{
    /// <summary>
    /// 创建公司
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<long> CreateCompanyAsync([NotNull] CreateOrUpdateCommpanyCmd cmd);


    /// <summary>
    /// 删除公司
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<bool> DeleteCompanyAsync([NotNull] long id);


    /// <summary>
    /// 更新公司信息
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<bool> UpdateCompanyAsync([NotNull] CreateOrUpdateCommpanyCmd cmd, [NotNull] long id);


    /// <summary>
    /// 按Id获取公司详情
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    Task<CompanyDetailsDto> GetCompanyById([NotNull] long id);


    /// <summary>
    /// 获取公司分页列表
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    Task<PageData<IEnumerable<CompanyPageListDto>>> GetPageListCompanyAsync([NotNull] CompanyPageListQry qry);



    /// <summary>
    /// 检查联系电话是否已被注册
    /// </summary>
    /// <param name="contactPhone"></param>
    /// <returns></returns>
    Task<bool> IsContactPhoneExistAsync([NotNull] string contactPhone);

    /// <summary>
    /// 检查邮箱是否已被注册
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<bool> IsEmailExistAsync([NotNull] string email);
}
