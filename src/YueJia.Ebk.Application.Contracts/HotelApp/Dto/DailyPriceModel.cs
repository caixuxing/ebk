using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto
{
    public class DailyPriceModel
    {
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        public DateTime CurrentDate { get; set; }


        public bool StatusBool { get; set; }

        public string CurrentDateString
        {
            get
            {
                return CurrentDate.ToString("yyyy-MM-dd");
            }
        }

        public decimal InitialPrice
        {
            get {
                return Price;
            }
        }
        public bool InitialStatusBool {
            get
            {
                return StatusBool;
            }
        }

        public string DayOfWeekName
        {
            get
            {
                return new List<string>() { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" }[(int)CurrentDate.DayOfWeek];

            }
        }

    }
}
