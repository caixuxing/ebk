namespace YueJia.Ebk.Application.Contracts.DeptApp.Dto
{
    public class DeptPageListDto
    {
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long DeptId { get; set; }

        public string? DeptName { get; set; }

        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long ParentDeptId { get; set; }

        public string? CompanyName { get; set; }

        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long? CompanyId { get; set; }

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
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? CreateTime { get; set; }
    }
}
