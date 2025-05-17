using System.ComponentModel.DataAnnotations;

namespace YueJia.Ebk.Application.Contracts.Comm.BaseObj
{
    public record BasePageQry
    {
        /// <summary>
        /// 分页索引
        /// </summary>
        [Required]
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 分页大小
        /// </summary>
        [Required]
        public int PageSize { get; set; } = 50;
    }
}
