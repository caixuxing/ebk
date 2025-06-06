using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Validation;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands
{

    [DisableValidation]
    public class SaveInventoryCmd
    {
        /// <summary>
        /// Ebk 房型信息
        /// </summary>
        public HotelRoomListDto EbkRoom { get; set; }

        public List<DailyInventoryModel> DailyInventoryList { get; set; }

    }
}
