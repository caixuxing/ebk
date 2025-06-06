using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands
{
    public class SavePriceCmd
    {
        public string userPlanId { get; set; }

        public List<DailyPriceModel> priceList { get; set; }
    }
}
