using YueJia.Ebk.Domain.Shared.Utils;

namespace YueJia.Ebk.Domain.SysUser;


/// <summary>
/// 系统用户
/// </summary>
[SugarTable("SysUser", "系统用户")]
public partial record SysUserDo : EntityTenant
{
    public SysUserDo() { }
    /// <summary>
    /// 登录账户
    /// </summary>
    [SugarColumn(ColumnDescription = "登录账户", Length = 30)]
    public string AccountName { get; set; } = default!;
    /// <summary>
    /// 登录密码
    /// </summary>
    [SugarColumn(ColumnDescription = "登录密码", Length = 100)]
    public string Password { get; set; } = default!;
    /// <summary>
    /// 真实姓名（昵称）
    /// </summary>
    [SugarColumn(ColumnDescription = "真实姓名（昵称）", Length = 10)]
    public string RealName { get; set; } = default!;
    /// <summary>
    /// 账户类型
    /// </summary>
    [SugarColumn(ColumnDescription = "账户类型")]
    public AccountTypeEnum AccountType { get; set; }
    /// <summary>
    /// 账户状态(是否启用)
    /// </summary>
    [SugarColumn(ColumnDescription = "账户状态(是否启用)")]
    public YesOrNoType IsEnabled { get; set; }
    /// <summary>
    /// 部门ID
    /// </summary>
    [SugarColumn(ColumnDescription = "部门ID", IsNullable = true)]
    public long? DeptId { get; set; }
    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "联系电话", Length = 15)]
    public string? ContactPhone { get; set; }
}


public partial record SysUserDo
{
    private SysUserDo(string accountName, string password, string realName, AccountTypeEnum accountType, YesOrNoType isEnabled, long? deptId, string? contactPhone)
    {
        AccountName = accountName;
        Password = password;
        RealName = realName;
        AccountType = accountType;
        IsEnabled = isEnabled;
        DeptId = deptId;
        ContactPhone = contactPhone;
    }



    public static SysUserDo Create(string accountName, string realName, AccountTypeEnum accountType, YesOrNoType isEnabled, long? deptId, string? contactPhone)
    {
        var password = EncryptUtils.MD5Encrypt("123456");
        return new SysUserDo(accountName, password, realName, accountType, isEnabled, deptId, contactPhone);
    }


    public SysUserDo SetAccountName(string accountName)
    {
        AccountName = accountName;
        return this;
    }

    public SysUserDo SetPassword(string password)
    {
        Password = password;
        return this;
    }

    public SysUserDo SetRealName(string realName)
    {
        RealName = realName;
        return this;
    }

    public SysUserDo SetAccountType(AccountTypeEnum accountType)
    {
        AccountType = accountType;
        return this;
    }
    public SysUserDo SetIsEnabled(YesOrNoType isEnabled)
    {
        IsEnabled = isEnabled;
        return this;
    }

    public SysUserDo SetDeptId(long? deptId)
    {
        DeptId = deptId;
        return this;
    }

    public SysUserDo SetContactPhone(string? contactPhone)
    {
        ContactPhone = contactPhone;
        return this;
    }

}
