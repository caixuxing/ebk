namespace YueJia.Ebk.Application.Contracts.HotelApp.Commands;

/// <summary>
/// 保存库存和价格模型
/// </summary>
public record SaveInventoryAndPriceCmd
{
    /// <summary>
    /// 库存
    /// </summary>
    public List<InventoryOb> Inventorys { get; set; } = new();
    /// <summary>
    /// 价格列表
    /// </summary>
    public List<PriceOb> Prices { get; set; } = new();
}

/// <summary>
/// 库存
/// </summary>
public record InventoryOb
{
    /// <summary>
    /// 库存
    /// </summary>
    public string InventoryId { get; set; } = default!;
    /// <summary>
    /// 库存数量
    /// </summary>
    public int InventoryNum { get; set; }
    /// <summary>
    /// 是否启用
    /// </summary>
    public YesOrNoType Status { get; set; }
}

/// <summary>
/// 价格
/// </summary>
public record PriceOb
{
    /// <summary>
    /// 价格ID
    /// </summary>
    public string PriceId { get; set; } = default!;
    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public YesOrNoType Status { get; set; }
}