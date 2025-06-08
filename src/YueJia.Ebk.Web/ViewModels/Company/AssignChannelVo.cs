using System.Text.Json.Serialization;

namespace YueJia.Ebk.Web.ViewModels.Company
{
    public class AssignChannelVo
    {
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public List<string> SelectedChannelList { get; set; }



    }
}
