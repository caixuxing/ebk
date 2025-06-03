namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;

public record InventoryAndPriceDetailsDto
{


    public List<SelectDataDto<string>> RoomTypeDropdownList { get; set; }

    public Room Room { get; set; }

    public PricePlan PricePlan { get; set; }
}

/// <summary>
/// 房间信息
/// </summary>
public record Room
{

    public string Id { get; set; }

    public string RoomName { get; set; }

    public YesOrNoType Status { get; set; }

    public string StatusName
    {
        get
        {
            return Status.ToDescription();
        }
    }
    /// <summary>
    /// 库存信息
    /// </summary>
    public List<Inventory> Inventories { get; set; }


}

public record Inventory
{
    public string Id { get; set; }

    /// <summary>
    /// 月-日（01-01）
    /// </summary>
    public string MonthDay { get; set; } = default!;

    /// <summary>
    /// 周天（星期几）
    /// </summary>
    public string DayOfWeek { get; set; } = default!;

    /// <summary>
    /// 库存数
    /// </summary>
    public int InventoryNum { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public YesOrNoType Status { get; set; }

    /// <summary>
    /// 状态名称
    /// </summary>
    public string StatusName
    {
        get
        {
            return Status.ToDescription();
        }
    }
}

/// <summary>
/// 价格信息
/// </summary>
public record PriceDetail
{
    /// <summary>
    /// 
    /// </summary>
    public string Id { get; set; }

    public string Name { get; set; }
    public string StatusName { get; set; }
}



/// <summary>
/// 价格计划
/// </summary>
public record PricePlan
{
    public string Id { get; set; }

    public string Name { get; set; }

    public YesOrNoType Status { get; set; }

    public List<PriceItem> Prices { get; set; }

}


/// <summary>
/// 价格
/// </summary>
public record PriceItem
{
    /// <summary>
    /// 价格ID
    /// </summary>
    public string Id { get; set; } = default!;
    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }
    /// <summary>
    /// 状态
    /// </summary>
    public YesOrNoType Status { get; set; }

}
