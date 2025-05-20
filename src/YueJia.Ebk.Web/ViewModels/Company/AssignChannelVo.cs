using System.Text.Json.Serialization;

namespace YueJia.Ebk.Web.ViewModels.Company
{
    public class AssignChannelVo
    {
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public long CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public string? SelectedChannelJosn { get; set; }

        public string? ChannelDataJosn { get; set; }

    }
}
