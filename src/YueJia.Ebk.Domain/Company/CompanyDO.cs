namespace YueJia.Ebk.Domain.Company;


/// <summary>
/// 公司
/// </summary>
[SugarTable("Company", "公司")]
public partial record CompanyDO : EntityTenant
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public CompanyDO() { }

    /// <summary>
    /// 公司名称
    /// </summary>
    [SugarColumn(ColumnDescription = "公司名称", Length = 30)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 责任人
    /// </summary>
    [SugarColumn(ColumnDescription = "责任人", Length = 8)]
    public string Responsible { get; set; } = default!;

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "联系电话", Length = 15)]
    public string ContactPhone { get; set; } = default!;

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "邮箱", Length = 30)]
    public string? Email { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(ColumnDescription = "密码", Length = 100)]
    public string Password { get; set; } = default!;

    /// <summary>
    /// 公司地址
    /// </summary>
    [SugarColumn(ColumnDescription = "公司地址", Length = 80)]
    public string? CompanyAddr { get; set; } = default!;

    /// <summary>
    /// 是否渠道管理
    /// </summary>
    [SugarColumn(ColumnDescription = "是否渠道管理", Length = 20)]
    public YesOrNoType IsChannelManage { get; set; } = default!;

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", Length = 1)]
    public YesOrNoType Status { get; set; }


    /// <summary>
    /// 调价类型
    /// </summary>
    [SugarColumn(ColumnDescription = "调价类型", IsNullable = true)]
    public AdjustmentPriceTypeEnum? AdjustmentPriceType { get; set; }

    /// <summary>
    /// 调价值
    /// </summary>
    [SugarColumn(ColumnDescription = "调价值", IsNullable = true)]
    public int? AdjustmentPriceValue { get; set; }

}

public partial record CompanyDO
{


    private CompanyDO(string name, string responsible, string contactPhone, string? email, string? companyAddr,
        YesOrNoType isChannelManage, YesOrNoType status, AdjustmentPriceTypeEnum? adjustmentPriceType, int? adjustmentPriceValue)
    {
        Name = name;
        Responsible = responsible;
        ContactPhone = contactPhone;
        Email = email;
        CompanyAddr = companyAddr;
        IsChannelManage = isChannelManage;
        Status = status;
        Password = EncryptUtils.MD5Encrypt("123456");
        this.Id = SnowFlakeSingle.instance.getID();
        this.TenantId = SnowFlakeSingle.instance.getID();
        if (adjustmentPriceType.HasValue)
        {
            this.AdjustmentPriceType = adjustmentPriceType;
            this.AdjustmentPriceValue = adjustmentPriceValue;
        }
    }

    public static CompanyDO Create(string name, string responsible, string contactPhone, string? email, string? companyAddr,
        YesOrNoType isChannelManage, YesOrNoType status, AdjustmentPriceTypeEnum? adjustmentPriceType, int? adjustmentPriceValue)
    {
        return new CompanyDO(name, responsible, contactPhone, email, companyAddr, isChannelManage, status, adjustmentPriceType, adjustmentPriceValue);
    }

    /// <summary>
    /// 设置公司名称
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public CompanyDO SetName(string name)
    {
        this.Name = name;
        return this;
    }
    public CompanyDO SetResponsible(string responsible)
    {
        this.Responsible = responsible;
        return this;
    }
    public CompanyDO SetContactPhone(string contactPhone)
    {
        this.ContactPhone = contactPhone;
        return this;
    }
    public CompanyDO SetEmail(string? email)
    {
        this.Email = email;
        return this;
    }

    public CompanyDO SetPassword(string password)
    {
        this.Password = EncryptUtils.MD5Encrypt(password);
        return this;
    }

    public CompanyDO SetCompanyAddr(string? companyAddr)
    {
        this.CompanyAddr = companyAddr;
        return this;
    }
    public CompanyDO SetIsChannelManage(YesOrNoType isChannelManage)
    {
        this.IsChannelManage = isChannelManage;
        return this;
    }
    public CompanyDO SetStatus(YesOrNoType status)
    {
        this.Status = status;
        return this;
    }
    public CompanyDO SetAdjustmentPriceType(AdjustmentPriceTypeEnum? adjustmentPriceType)
    {
        this.AdjustmentPriceType = adjustmentPriceType;
        return this;
    }
    public CompanyDO SetAdjustmentPriceValue(int? adjustmentPriceValue)
    {
        this.AdjustmentPriceValue = adjustmentPriceValue;
        return this;
    }
}
