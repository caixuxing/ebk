using System.ComponentModel.DataAnnotations;

namespace YueJia.Ebk.Application.Contracts.DeptApp.Commands
{
    /// <summary>
    /// 创建部门命令
    /// </summary>
    public record CreateOrUpdateDeptCmd
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [Required]
        public string Name { get; set; } = default!;

        /// <summary>
        /// 父级部门ID
        /// </summary>
        [Required]
        public string ParentDeptId { get; set; } = default!;

        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        public YesOrNoType Status { get; set; }
    }

    /// <summary>
	/// 创建公司命令参数校验
	/// </summary>
	public class CreateOrUpdateDeptCmdValidator : AbstractValidator<CreateOrUpdateDeptCmd>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CreateOrUpdateDeptCmdValidator()
        {

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("部门名称不能为空！")
                .MaximumLength(15).WithMessage("公司名称长度不能超过15个字符！");

            RuleFor(x => x.Status).IsInEnum().WithMessage("状态参数无效！");

        }
    }
}
