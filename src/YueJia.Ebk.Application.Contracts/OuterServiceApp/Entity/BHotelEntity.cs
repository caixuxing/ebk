using SqlSugar;

namespace YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity
{

    [SugarTable("b_hotel")]
    public partial class BHotelEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 酒店编号
        /// </summary>
        public string? hotelcode { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        public string? hotelname { get; set; }

        /// <summary>
        /// 酒店名称(英文)
        /// </summary>
        public string? hotelnameen { get; set; }

        /// <summary>
        /// 国家id
        /// </summary>
        public int? countryid { get; set; }

        /// <summary>
        /// 区域id
        /// </summary>
        public int? areaid { get; set; }

        /// <summary>
        /// 星级
        /// </summary>
        public int? starlevel { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string? address { get; set; }

        /// <summary>
        /// 地址(英文)
        /// </summary>
        public string? addressen { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string? telphone { get; set; }

        /// <summary>
        /// email
        /// </summary>
        public string? email { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string? fax { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        public string? website { get; set; }

        /// <summary>
        /// gps
        /// </summary>
        public int? gpstype { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string? latitude { get; set; }


        /// <summary>
        /// 经度
        /// </summary>
        public string? longitude { get; set; }


        /// <summary>
        /// 描叙
        /// </summary>
        public string? content { get; set; }

        /// <summary>
        /// 特色
        /// </summary>
        public string? sellingpoint { get; set; }

        /// <summary>
        /// 状态(0无效 1有效 2待审核 3已审核)
        /// </summary>
        public int? status { get; set; }


        /// <summary>
        /// 创建者
        /// </summary>
        public string? createuser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createtime { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public string? modifyuser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? modifytime { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime? uptime { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int? commentnum { get; set; }

        /// <summary>
        /// 开业时间
        /// </summary>
        public DateTime? opentime { get; set; }


        /// <summary>
        /// 检索关键词
        /// </summary>
        public string? searchkey { get; set; }

        /// <summary>
        /// 携程Code
        /// </summary>
        public string? ctripcode { get; set; }

        /// <summary>
        /// 携程名称
        /// </summary>
        public string? ctripcityname { get; set; }

        /// <summary>
        /// 携程状态
        /// </summary>
        public string? ctripstatus { get; set; }
    }
}
