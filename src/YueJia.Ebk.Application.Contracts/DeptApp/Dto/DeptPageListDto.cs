namespace YueJia.Ebk.Application.Contracts.DeptApp.Dto
{
    public class DeptPageListDto
    {
        public long DeptId { get; set; }

        public string DeptName { get; set; }

        public long DeptParentId { get; set; }

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
    }
}
