namespace YueJia.Ebk.Web.ViewModels.Hotel
{
    public class ViewHotelVo
    {

        public long Id { get; set; }

        public string HotelCode { get; set; }

        public string HotelName { get; set; }

        public string HotelNameEn { get; set; }

        public string Address { get; set; }

        public string AddressEn { get; set; }

        public string TelPhone { get; set; }
        public decimal LowestPrice { get; set; }

        public HotelSaleTypeEnum Status { get; set; }

        /// <summary>
        /// 销售类型Josn
        /// </summary>
        public string HotelSaleTypeJson { get; set; } = default!;
    }
}
