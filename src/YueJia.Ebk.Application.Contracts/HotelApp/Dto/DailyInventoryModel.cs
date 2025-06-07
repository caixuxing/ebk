using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    public class DailyInventoryModel
    {
        /// <summary>
        /// 当前日期
        /// </summary>
        public DateTime CurrentDate { get; set; }

        /// <summary>
        /// 当日库存数
        /// </summary>
        public int InventoryNum { get; set; }

   

        public bool StatusBool { get; set; }


        public string CurrentDateString {
            get {
                return CurrentDate.ToString("yyyy-MM-dd");
            }
        }

        public int InitialInventoryNum
        {
            get {
                return InventoryNum;
            }
        }

        public bool InitialStatusBool
        {
            get
            {
                return StatusBool;
            }
        }

    }
}
