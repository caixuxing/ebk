namespace YueJia.Ebk.Domain.Dict
{
    /// <summary>
    /// 字典数据
    /// </summary>
    [SugarTable("DictData")]
    public partial record DictDataDo : EntityBase
    {
        /// <summary>
        /// 字典分类ID
        /// </summary>
        [SugarColumn(ColumnDescription = "字典分类ID")]
        public long CategoryId { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>
        [SugarColumn(ColumnDescription = "字典名称", Length = 50)]
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(ColumnDescription = "排序")]
        public int Sort { get; set; }
    }
}
