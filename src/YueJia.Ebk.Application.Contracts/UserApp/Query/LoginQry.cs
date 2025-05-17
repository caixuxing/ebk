namespace YueJia.Ebk.Application.Contracts.UserApp.Query
{
	public record LoginQry
	{
		/// <summary>
		/// 用户名
		/// </summary>
		public string UserName { get; set; } = default!;

		/// <summary>
		/// 用户密码
		/// </summary>
		public string PassWord { get; set; } = default!;
	}

	/// <summary>
	/// 登录参数校验
	/// </summary>
	public class LoginQryValidator : AbstractValidator<LoginQry>
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public LoginQryValidator()
		{

			RuleFor(x => x.UserName)
				.Cascade(CascadeMode.Stop)
				.NotNull().WithMessage("账户号不能为空！");
			RuleFor(x => x.PassWord).NotEmpty().WithMessage("账户密码不能为空！");
		}
	}
}
