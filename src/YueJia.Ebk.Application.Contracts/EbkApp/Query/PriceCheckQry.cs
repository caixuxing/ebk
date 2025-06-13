namespace YueJia.Ebk.Application.Contracts.EbkApp.Query;


/// <summary>
/// 验价检查Qry
/// </summary>
public record PriceCheckQry : PriceSearchQry
{
    /// <summary>
    /// 查价唯一标识
    /// </summary>
    public string SearchCode { get; set; } = default!;
}
