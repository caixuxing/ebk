namespace YueJia.Ebk.Application.Contracts.SysApp.Dto;

/// <summary>
/// 枚举实体
/// </summary>
public record EnumDto
{
    /// <summary>
    /// 枚举的描述
    /// </summary>
    public string Describe { set; get; } = string.Empty;
    /// <summary>
    /// 枚举名称
    /// </summary>
    public string Name { set; get; } = string.Empty;

    /// <summary>
    /// 枚举对象的值
    /// </summary>
    public int Value { set; get; }
}
