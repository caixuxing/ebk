using System.ComponentModel.DataAnnotations;

namespace YueJia.Ebk.Application.Contracts.CompanyApp.Commands
{
    public record AssignChannelCmd
    {

        /// <summary>
        /// 渠道集合
        /// </summary>
        [Required]
        public List<int> ChannelData { get; set; } = default!;
    }
}
