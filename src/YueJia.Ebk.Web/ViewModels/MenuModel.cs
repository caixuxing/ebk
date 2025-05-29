namespace YueJia.Ebk.Web.ViewModels
{
    public class MenuModel
    {
        public string pageIndex { get; set; }
        public string title { get; set; }
        public string url { get; set; } = "";

        public bool isShow { get; set; }


        public List<MenuModel> children { get; set; } = new List<MenuModel>();
    }
}
