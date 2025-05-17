using System.ComponentModel.DataAnnotations;

namespace YueJia.Ebk.Application.Contracts.CompanyApp.Commands
{

    /// <summary>
    /// 创建公司命令
    /// </summary>
    public record CreateOrUpdateCommpanyCmd
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        [Required]
        public string Name { get; set; } = default!;

        /// <summary>
        /// 责任人
        /// </summary>
        [Required]
        public string Responsible { get; set; } = default!;

        /// <summary>
        /// 联系电话
        /// </summary>
        [Required]
        public string ContactPhone { get; set; } = default!;

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        public string Email { get; set; } = default!;

        /// <summary>
        /// 公司地址
        /// </summary>
        [Required]
        public string CompanyAddr { get; set; } = default!;

        /// <summary>
        /// 是否渠道管理
        /// </summary>
        [Required]
        public YesOrNoType IsChannelManage { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        public YesOrNoType Status { get; set; }
    }

    /// <summary>
	/// 创建公司命令参数校验
	/// </summary>
	public class CreateCommpanyCmdValidator : AbstractValidator<CreateOrUpdateCommpanyCmd>
    {

        readonly ICompanyApp _companyApp;


        /// <summary>
        /// 构造函数
        /// </summary>
        public CreateCommpanyCmdValidator(ICompanyApp companyApp)
        {
            _companyApp = companyApp;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("公司名称不能为空！")
                .MaximumLength(30).WithMessage("公司名称长度不能超过30个字符！");
            RuleFor(x => x.Responsible)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("责任人不能为空！")
                .MaximumLength(8).WithMessage("责任人长度不能超过8个字符！");
            RuleFor(x => x.ContactPhone)
              .Cascade(CascadeMode.Stop)
              .NotEmpty().WithMessage("联系电话不能为空！")
              .MaximumLength(15).WithMessage("责任人长度不能超过15个字符！")
              .Matches(@"^[0-9+\-()\s]+$") // 基础格式校验（数字、+、-、括号、空格）
              .WithMessage("联系电话格式无效，仅允许数字、+、-或括号"); //示例有效值： +86(755) 1234 - 5678、0755 - 1234567、13800138000
                                                     //.MustAsync(async (phone, cancellation) => await IsContactPhoneExistAsync(phone)).WithMessage("联系电话已被占用");

            RuleFor(x => x.Email)
                .MaximumLength(30).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("邮箱长度不能超过30个字符！")
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("邮箱格式不正确");
            //.MustAsync(async (email, cancellation) => await IsEmailExistAsync(email)).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("邮箱已被占用");


            RuleFor(x => x.CompanyAddr)
               .Cascade(CascadeMode.Stop)
               .MaximumLength(80).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("公司地址长度不能超过80个字符！");

            RuleFor(x => x.IsChannelManage).IsInEnum().WithMessage("是否渠道管理参数无效！");

            RuleFor(x => x.Status).IsInEnum().WithMessage("状态参数无效！");

        }

        public async Task<bool> IsContactPhoneExistAsync(string contactPhone)
        {
            var result = await _companyApp.IsContactPhoneExistAsync(contactPhone);
            return !(result ? result : false);
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            var result = await _companyApp.IsEmailExistAsync(email);
            return !(result ? result : false);
        }
    }
}
