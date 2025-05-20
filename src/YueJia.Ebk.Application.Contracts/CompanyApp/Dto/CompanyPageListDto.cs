

namespace YueJia.Ebk.Application.Contracts.CompanyApp.Dto;

public record CompanyPageListDto
{

    /// <summary>
    /// ID
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public long Id { get; set; }

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
    /// 邮箱
    /// </summary>

    public string? Email { get; set; }

    /// <summary>
    /// 是否渠道管理
    /// </summary>
    public YesOrNoType IsChannelManage { get; set; }

    /// <summary>
    /// 是否渠道管理
    /// </summary>
    public string IsChannelManageName
    {
        get
        {
            return IsChannelManage.ToDescription();
        }
    }
    /// <summary>
    /// 状态
    /// </summary>
    public YesOrNoType Status { get; set; }


    /// <summary>
    /// 状态枚举描述
    /// </summary>
    public string StatusName
    {
        get
        {
            return Status.ToDescription();
        }
    }
    /// <summary>
    /// 创建日期
    /// </summary>
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime? CreateTime { get; set; }

}
