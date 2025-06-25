using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Domain.Order
{
    [SugarTable("OrderLog", "订单Log")]
    public partial record OrderLogDo : EntityBaseId
    {
        public OrderLogDo() { }
        /// <summary>
        /// 预订号
        /// </summary>
        [SugarColumn(ColumnDescription = "预订号", Length = 50)]
        public string OrderNum { get; set; } = default!;

        /// <summary>
        ///名拼音
        /// </summary>
        [SugarColumn(ColumnDescription = "描述内容", Length = 500)]
        public string Describe { get; set; } = default!;

        public DateTime CreateTime { get; set; }

    }
    public partial record OrderLogDo
    {

        public OrderLogDo(string orderNum, string describe)
        {
            OrderNum = orderNum;
            Describe = describe;
            CreateTime = DateTime.Now;
        }

        public static OrderLogDo Create(string orderNum, string describe)
        {
            return new OrderLogDo(orderNum, describe);
        }
    }
}