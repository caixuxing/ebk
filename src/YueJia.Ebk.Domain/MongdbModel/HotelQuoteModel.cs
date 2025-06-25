using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Domain.MongdbModel
{
    [BsonIgnoreExtraElements]
    public class HotelQuoteModel
    {
        public long Id { get; init; }

        /// <summary>
        /// 酒店编码
        /// </summary>
        public string HotelCode { get; set; } = default!;

        /// <summary>
        /// 房型
        /// </summary>
        public string RoomCode { get; set; } = default!;



        /// <summary>
        /// 人数上限
        /// </summary>
        public int MaximumNumberOfPeople { get; set; }
        /// <summary>
        /// 成人上限
        /// </summary>
        public int AdultLimit { get; set; }

        /// <summary>
        /// 儿童上限
        /// </summary>
        public int? ChildLimit { get; set; }

        /// <summary>
        /// 早餐类型
        /// </summary>
        public BreakfastTypeEnum BreakfastType { get; set; }
        /// <summary>
        /// 提前天数
        /// </summary>
        public int DaysInAdvance { get; set; }

        /// <summary>
        /// 连住天数
        /// </summary>
        public int ContinuousStayDays { get; set; }

        /// <summary>
        /// 当前日期
        /// </summary>
        public int CurrentDate { get; set; }

        /// <summary>
        /// 当日库存数
        /// </summary>
        public int InventoryNum { get; set; }

        /// <summary>
        /// 当日价格
        /// </summary>
        public int Price { get; set; }


        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = default!;

        /// <summary>
        /// 部门Id
        /// </summary>
        public string DeptId { get; set; } = default!;


        /// <summary>
        /// 公司Id
        /// </summary>
        public string CompanyId { get; set; } = default!;

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastModifiedTime { get; set; }

        public string UserPricePlanId { get; set; }

        /// <summary>
        /// 调价类型
        /// </summary>
        public AdjustmentPriceTypeEnum AdjustmentPriceType { get; set; }
        /// <summary>
        /// 调价值
        /// </summary>
        public int AdjustmentPriceValue { get; set; }

    }
}
