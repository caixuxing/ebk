namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;


/// <summary>
/// 价格计划集合Dto
/// </summary>
public record PricePlanListDto
{
    /// <summary>
    /// 价格计划Id
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long Id { get; set; }
}
