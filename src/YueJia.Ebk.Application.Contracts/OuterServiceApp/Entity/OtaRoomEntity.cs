namespace YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("ota_room")]
    public partial class OtaRoomEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public OtaRoomEntity()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int roomcode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String pfcode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String roomname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String roomnameen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String hotelcode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 nonsmoking { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String sizemeasurement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 haswindow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 bedtype { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String createuser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.DateTime createtime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String modifyuser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.DateTime modifytime { get; set; }
    }
}
