using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public string DayOfWeekName
        {
            get {
               return new List<string>() { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" }[(int)CurrentDate.DayOfWeek];

            }
        }

    }
}
