namespace YueJia.Ebk.Domain.AggRoot
{
    /// <summary>
    /// 假删除接口过滤器
    /// </summary>
    public interface IDeletedFilter
    {
        /// <summary>
        /// 软删除
        /// </summary>
        bool IsDelete { get; set; }
    }
}
