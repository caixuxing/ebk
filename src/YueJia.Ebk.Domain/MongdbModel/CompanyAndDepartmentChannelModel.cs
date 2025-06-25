using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain;

namespace YueJia.Ebk.Domain.MongdbModel
{

    [BsonIgnoreExtraElements]
    public class CompanyAndDepartmentChannelModel
    {
        public string TableId { get; set; }

        public string CompanyAndDepartmentId { get; set; }

        public string PFCode { get; set; }

    }
}
