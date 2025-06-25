namespace YueJia.Ebk.Domain.Shared.Dto;

public record TaskPublishDto
{
    public string OrderCode { get; set; }

    public string HotelName { get; set; }



    public string HotelNameEn { get; set; }

    public string RoomNmae { get; set; }

    public string BedType { get; set; }

    public string CheckInDate { get; set; }

    public string CheckOutDate { get; set; }

    public int AdultNumber { get; set; }

    public int ChildNumber { get; set; }

    public decimal CostAmount { get; set; }

    public int RoomNumber { get; set; }

    public List<string> PersonName { get; set; } = new();

    /// <summary>
    /// 接收者账户
    /// </summary>
    public string RecipientAccount { get; set; }
}
