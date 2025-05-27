using System.Security.Claims;

namespace YueJia.Ebk.Domain.Shared.Dto
{
    /// <summary>
    /// 当前账户所属公司
    /// </summary>
    public class CurrentAccountCompanyDto : Claim
    {


        public CurrentAccountCompanyDto(string type, string value, string? companyId, string? companyName, string? tenantId)
      : base(type, value)
        {
            this.CompanyId = companyId;
            this.CompanyName = companyName;
            this.TenantId = tenantId;
        }

        /// <summary>
        /// 公司ID
        /// </summary>
        public string? CompanyId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        ///租户ID
        /// </summary>
        public string? TenantId { get; set; }
    }
}
