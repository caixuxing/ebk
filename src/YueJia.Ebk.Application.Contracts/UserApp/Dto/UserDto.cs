namespace YueJia.Ebk.Application.Contracts.UserApp.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }

        public string UserPwd { get; set; }

        public long? TenantId { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public AccountTypeEnum AccountType { get; set; }

    }
}
