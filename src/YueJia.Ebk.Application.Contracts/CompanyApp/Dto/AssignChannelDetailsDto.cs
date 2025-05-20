namespace YueJia.Ebk.Application.Contracts.CompanyApp.Dto;


/// <summary>
/// 分配渠道详情
/// </summary>
public record AssignChannelDetailsDto
{
    /// <summary>
    /// 公司名称
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// 公司ID
    /// </summary>
    public long CompanyId { get; init; }

    /// <summary>
    /// 渠道集合
    /// </summary>
    public List<long>? ChannelData { get; init; }

}
