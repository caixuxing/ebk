using SqlSugar;

namespace YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity;


[SugarTable("b_area")]
public partial record BAreaEntity
{
    /// <summary>
    /// 
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    public int ParentareId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 英文名称
    /// </summary>
    public string EnName { get; set; } = default!;

    /// <summary>
    /// 星级
    /// </summary>
    public int level { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Code { get; set; } = default!;

    /// <summary>
    /// 区域目录
    /// </summary>
    public string AreaPath { get; set; } = default!;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdateTime { get; set; }

    /// <summary>
    /// 国家两位代码
    /// </summary>
    public string? CountryIosCode { get; set; }

}
