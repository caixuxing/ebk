namespace YueJia.Ebk.Application.Contracts.UserApp;

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
}
