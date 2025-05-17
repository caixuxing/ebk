namespace YueJia.Ebk.Domain.Shared.Response
{
    /// <summary>
    /// 分页查询统一返回类
    /// </summary>
    /// <typeparam name="T">返回类型</typeparam>
    public class PageData<T>
    {
        public PageData(int total, int pageSize, int pageIndex, T data)
        {
            Total = total;
            PageSize = pageSize;
            PageIndex = pageIndex;
            this.PageCount = (int)Math.Ceiling(Total / (double)PageSize);
            this.List = data;
        }

        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 页码大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 分页索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// 返回的数据对象
        /// </summary>
        public T List { get; set; }
    }
}
