namespace YueJia.Ebk.Application.Contracts.CompanyApp.Dto
{
    public class CompanyDetailsDto
    {

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 责任人
        /// </summary>
        public string Responsible { get; set; } = default!;

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhone { get; set; } = default!;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string CompanyAddr { get; set; } = default!;

        /// <summary>
        /// 是否渠道管理
        /// </summary>
        public YesOrNoType IsChannelManage { get; set; } = default!;

        /// <summary>
        /// 状态
        /// </summary>
        public YesOrNoType Status { get; set; }
    }
}
