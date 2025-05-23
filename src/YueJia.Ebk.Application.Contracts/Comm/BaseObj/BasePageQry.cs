using System.ComponentModel.DataAnnotations;

namespace YueJia.Ebk.Application.Contracts.Comm.BaseObj
{
    public record BasePageQry
    {



        private int _pageIndex = 1;
        /// <summary>
        /// 分页大小
        /// </summary>
        [Required]
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value <= 0 ? 1 : value;
        }

        private int _PageSize = 50;
        /// <summary>
        /// 分页大小
        /// </summary>
        [Required]
        public int PageSize
        {
            get => _PageSize;
            set => _PageSize = value <= 0 ? 20 : value;
        }




    }
}
