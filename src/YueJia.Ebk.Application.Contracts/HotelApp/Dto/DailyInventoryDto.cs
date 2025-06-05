using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    public class DailyInventoryDto
    {
        public string Id { get; set; }
        /// <summary>
        /// 房间Id
        /// </summary>
        public long RoomId { get; init; }

        /// <summary>
        /// 当前日期
        /// </summary>
        public DateTime CurrentDate { get; set; }

        /// <summary>
        /// 当日库存数
        /// </summary>
        public int InventoryNum { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public YesOrNoType IsEnabled { get; set; }


        public string CurrentDateString {
            get {
                return CurrentDate.ToString("yyyy-MM-dd");
            }
        }


    }
}
