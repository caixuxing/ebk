namespace YueJia.Ebk.Application.Contracts.SysUserApp.Commands
{
    /// <summary>
    /// 更新密码
    /// </summary>
    public class UpdatePasswordSysUserCmd
    {
        /// <summary>
        /// 原始密码
        /// </summary>
        [Required]
        public string OldPassword { get; set; } = default!;

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string NewFirstPassword { get; set; } = default!;
        /// <summary>
        /// 确认新密码
        /// </summary>
        [Required]
        public string NewConfirmPassword { get; set; } = default!;

    }
}
