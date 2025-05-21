namespace YueJia.Ebk.Application.Contracts.DeptApp.Dto
{
    public record DeptPageListDto : DeptPageListBaseDto
    {

        /// <summary>
        /// 子级
        /// </summary>
        public List<DeptPageListBaseDto> Children { get; set; } = new();
    }



    public record DeptPageListBaseDto
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
