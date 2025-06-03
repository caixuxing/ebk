using YueJia.Ebk.Application.Contracts.SysUserApp.Dto;

namespace YueJia.Ebk.Application.Contracts.SysUserApp;

public interface ICurrentUserApp
{
    /// <summary>
    /// 用户Id
    /// </summary>
    string Id { get; }

    /// <summary>
    /// 用户名
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string UserNickName { get; }

    /// <summary>
    ///租户Id
    /// </summary>
    string TenantId { get; }

    /// <summary>
    /// 是否部门管理
    /// </summary>
    bool IsDeptAdmin { get; }

    /// <summary>
    /// 账户类型
    /// </summary>
    AccountTypeEnum? AccountType { get; }

    /// <summary>
    /// 公司
    /// </summary>
    CurrentAccountCompanyDto Company { get; }

    /// <summary>
    /// 部门
    /// </summary>
    CurrentAccountDeptDto Dept { get; }

}
