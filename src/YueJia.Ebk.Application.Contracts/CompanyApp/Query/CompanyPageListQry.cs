namespace YueJia.Ebk.Application.Contracts.CompanyApp.Query;

public record CompanyPageListQry : BasePageQry
{
    /// <summary>
    /// 公司名称
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 责任人
    /// </summary>
    public string? Responsible { get; set; }
    /// <summary>
    /// 联系电话
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 是否渠道管理
    /// </summary>
    public YesOrNoType? IsChannelManage { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public YesOrNoType? Status { get; set; }
}
