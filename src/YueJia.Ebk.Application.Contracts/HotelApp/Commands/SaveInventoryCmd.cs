using YueJia.Ebk.Application.Contracts.HotelApp.Dto;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands
{


    public class SaveInventoryCmd
    {
        /// <summary>
        /// Ebk 房型信息
        /// </summary>
        public HotelRoomListDto EbkRoom { get; set; }

        public List<DailyInventoryModel> DailyInventoryList { get; set; }

    }
}
