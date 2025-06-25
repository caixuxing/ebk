namespace YueJia.Ebk.Application.Contracts.SysUserApp.Commands;

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

/// <summary>
/// 创建公司命令参数校验
/// </summary>
public class UpdatePasswordSysUserCmdValidator : AbstractValidator<UpdatePasswordSysUserCmd>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public UpdatePasswordSysUserCmdValidator()
    {
        RuleFor(x => x.OldPassword)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("当前密码不能为空！")
            .MaximumLength(30).WithMessage("当前密码长度不能超过30个字符！");

        RuleFor(x => x.NewFirstPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("新密码不能为空！")
            .MaximumLength(30).WithMessage("新密码长度不能超过30个字符！");


        RuleFor(x => x.NewConfirmPassword)
          .Cascade(CascadeMode.Stop)
          .NotEmpty().WithMessage("确认密码不能为空！")
          .MaximumLength(30).WithMessage("确认密码长度不能超过30个字符！")
          .Equal(x => x.NewFirstPassword).WithMessage("两次输入的密码不一致！");



    }
}
