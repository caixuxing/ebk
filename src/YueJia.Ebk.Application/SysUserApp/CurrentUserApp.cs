using Microsoft.AspNetCore.Http;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.Shared.Const;
using YueJia.Ebk.Domain.Shared.Enums;

namespace YueJia.Ebk.Application.SysUserApp;

public class CurrentUserApp : ApplicationService, ICurrentUserApp
{
    private readonly IHttpContextAccessor _accessor;

    /// <summary>
    ///
    /// </summary>
    /// <param name="accessor"></param>
    public CurrentUserApp(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    /// <summary>
    /// 用户Id
    /// </summary>
    public virtual string Id
    {
        get
        {
            var id = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserId);
            return id?.Value ?? string.Empty;
        }
    }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName
    {
        get
        {
            var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserName);
            return name?.Value ?? string.Empty;
        }
    }

    /// <summary>
    /// 姓名
    /// </summary>
    public string UserNickName
    {
        get
        {
            var name = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.UserNickName);
            return name?.Value ?? string.Empty;
        }
    }

    /// <summary>
    /// 租户Id
    /// </summary>
    public string TenantId
    {
        get
        {
            var orgcode = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.TenantId);
            return orgcode?.Value ?? string.Empty;
        }
    }

    public AccountTypeEnum? AccountType
    {
        get
        {
            var accountType = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.AccountType);
            return (AccountTypeEnum)Enum.ToObject(typeof(AccountTypeEnum), int.Parse(accountType?.Value ?? AccountTypeEnum.NormalUser.GetHashCode().ToString()));

        }
    }

    public string CompanyId
    {
        get
        {
            var companyId = _accessor?.HttpContext?.User?.FindFirst(ClaimAttributes.CompanyId);
            return companyId?.Value ?? string.Empty;
        }
    }
}
