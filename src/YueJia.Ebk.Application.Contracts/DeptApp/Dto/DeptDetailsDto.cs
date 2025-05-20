namespace YueJia.Ebk.Application.Contracts.DeptApp.Dto;

public record DeptDetailsDto
{
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long? Id { get; set; }

    public string? DeptName { get; set; }

    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long? ParentDeptId { get; set; } = -1;

    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long? CompanyId { get; set; }
    /// <summary>
    /// 状态
    /// </summary>
    public YesOrNoType Status { get; set; } = YesOrNoType.Yes;
}
