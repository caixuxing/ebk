using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueJia.Ebk.Application.Contracts.DeptApp.Dto
{
    public class DepartmentChannelMapModel
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public long DeptId { get; init; }

        /// <summary>
        /// 渠道集合
        /// </summary>
        public List<string> SalePlatCodeList { get; init; }
    }
}
