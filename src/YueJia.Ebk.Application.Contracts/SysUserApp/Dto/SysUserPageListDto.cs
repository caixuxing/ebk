namespace YueJia.Ebk.Application.Contracts.SysUserApp.Dto;

public record SysUserPageListDto
{
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long Id { get; set; }

    public string AccountName { get; set; } = default!;

    /// <summary>
    /// 真实姓名（昵称）
    /// </summary>
    public string RealName { get; set; } = default!;


    /// <summary>
    /// 联系电话
    /// </summary>
    public string ContactPhone { get; set; } = default!;

    /// <summary>
    /// 账户状态(是否启用)
    /// </summary>
    public YesOrNoType IsEnabled { get; set; }

    /// <summary>
    /// 状态枚举描述
    /// </summary>
    public string IsEnabledName
    {
        get { return IsEnabled.ToDescription(); }
        private set { }
    }
    /// <summary>
    /// 部门ID
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long? DeptId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string? DeptName { get; set; }

    /// <summary>
    /// 创建日期
    /// </summary>
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime CreateTime { get; set; }

}
