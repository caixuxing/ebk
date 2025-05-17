namespace YueJia.Ebk.Application.Contracts.DeptApp.Query
{

    public record DeptPageListQry : BasePageQry
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string? Name { get; set; }



        /// <summary>
        /// 状态
        /// </summary>
        public YesOrNoType? Status { get; set; }
    }
}
