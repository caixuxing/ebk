namespace YueJia.Ebk.Domain.Other;




[SugarTable("TaskPublish", "任务推送表")]
[SugarIndex("index_{table}_C", nameof(CeateTime), OrderByType.Asc)]
public partial record TaskPublishDo : EntityBaseId
{
    /// <summary>
    /// 推送类型
    /// </summary>
    [SugarColumn(ColumnDescription = "推送类型")]
    public PushTypeEnum PushType { get; set; }

    /// <summary>
    /// 业务ID
    /// </summary>
    [SugarColumn(ColumnDescription = "业务ID")]
    public string PushKey { get; set; } = default!;

    /// <summary>
    /// 内容
    /// </summary>
    [SugarColumn(ColumnDescription = "推送类型", ColumnDataType = "text")]
    public string Content { get; set; } = default!;

    /// <summary>
    /// 推送次数
    /// </summary>
    [SugarColumn(ColumnDescription = "推送次数")]
    public int PushCount { get; set; }

    /// <summary>
    /// 最后推送时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后推送时间", IsNullable = true)]
    public DateTime? LastPushTime { get; set; }

    /// <summary>
    /// 响应结果
    /// </summary>
    [SugarColumn(ColumnDescription = "推送类型", ColumnDataType = "text", IsNullable = true)]
    public string? ResponseMsg { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public TaskPushStatusTypeEnum Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CeateTime { get; set; }
}


public partial record TaskPublishDo
{

    public TaskPublishDo() { }



    private TaskPublishDo(PushTypeEnum pushType, string pushKey, string content, int pushCount, DateTime? lastPushTime, TaskPushStatusTypeEnum status, DateTime ceateTime)
    {
        PushType = pushType;
        PushKey = pushKey;
        Content = content;
        LastPushTime = lastPushTime;
        PushCount = pushCount;
        Status = status;
        CeateTime = ceateTime;
    }

    public static TaskPublishDo Create(PushTypeEnum pushType, string pushKey, string content, int pushCount, DateTime? lastPushTime, TaskPushStatusTypeEnum status, DateTime ceateTime)
    {
        return new TaskPublishDo(pushType, pushKey, content, pushCount, lastPushTime, status, ceateTime);
    }

}