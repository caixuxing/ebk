namespace YueJia.Ebk.Domain.Dict
{

    /// <summary>
    /// 字典分类
    /// </summary>
    [SugarTable("DictCategory")]
    public partial record DictCategoryDo : EntityBase
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        [SugarColumn(ColumnDescription = "分类名称", Length = 50)]
        public string Name { get; set; } = default!;
        /// <summary>
        /// 分类编码
        /// </summary>
        [SugarColumn(ColumnDescription = "分类编码", Length = 50)]
        public string Code { get; set; } = default!;
    }
}
