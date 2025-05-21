using System.ComponentModel.DataAnnotations;

namespace YueJia.Ebk.Application.Contracts.SysUserApp.Commands;

/// <summary>
/// 创建公司命令
/// </summary>
public class CreateOrUpdateSysUserCmd
{
    /// <summary>
    /// 登录账户
    /// </summary>
    [Required]
    public string AccountName { get; set; } = default!;
    /// <summary>
    /// 真实姓名（昵称）
    /// </summary>
    [Required]
    public string RealName { get; set; } = default!;
    /// <summary>
    /// 是否启用
    /// </summary>
    [Required]
    public YesOrNoType IsEnabled { get; set; } = default!;
    /// <summary>
    /// 部门ID
    /// </summary>
    [Required]
    public string? DeptId { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? ContactPhone { get; set; }

}

/// <summary>
/// 创建公司命令参数校验
/// </summary>
public class CreateOrUpdateSysUserCmdValidator : AbstractValidator<CreateOrUpdateSysUserCmd>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public CreateOrUpdateSysUserCmdValidator()
    {
        RuleFor(x => x.AccountName)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("登录账户号不能为空！")
            .MaximumLength(30).WithMessage("登录账户长度不能超过30个字符！");
        RuleFor(x => x.RealName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("责任人不能为空！")
            .MaximumLength(10).WithMessage("责任人长度不能超过10个字符！");
        RuleFor(x => x.ContactPhone)
          .Cascade(CascadeMode.Stop)
          .NotEmpty().WithMessage("联系电话不能为空！")
          .MaximumLength(15).WithMessage("责任人长度不能超过15个字符！")
          .Matches(@"^[0-9+\-()\s]+$") // 基础格式校验（数字、+、-、括号、空格）
          .WithMessage("联系电话格式无效，仅允许数字、+、-或括号"); //示例有效值： +86(755) 1234 - 5678、0755 - 1234567、13800138000
                                                 //.MustAsync(async (phone, cancellation) => await IsContactPhoneExistAsync(phone)).WithMessage("联系电话已被占用");

        RuleFor(x => x.IsEnabled).IsInEnum().WithMessage("是否启用参数无效！");

    }
}
