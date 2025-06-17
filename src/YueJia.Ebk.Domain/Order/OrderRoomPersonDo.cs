namespace YueJia.Ebk.Domain.Order;


/// <summary>
/// 订单房间入住人信息
/// </summary>
[SugarTable("OrderRoomPerson", "订单房间入住人信息")]
public partial record OrderRoomPersonDo : EntityBaseId, IDeletedFilter
{
    public OrderRoomPersonDo() { }
    /// <summary>
    /// 预订号
    /// </summary>
    [SugarColumn(ColumnDescription = "预订号", Length = 50)]
    public string OrderNum { get; set; } = default!;

    /// <summary>
    /// 订单房间Id
    /// </summary>
    [SugarColumn(ColumnDescription = "订单房间Id")]
    public long OrderRoomId { get; set; }

    /// <summary>
    /// 房间序号 从第1间 开始
    /// </summary>
    [SugarColumn(ColumnDescription = "房间序号")]
    public int RoomIndex { get; set; }
    /// <summary>
    ///名拼音
    /// </summary>
    [SugarColumn(ColumnDescription = "名拼音", Length = 30)]
    public string FirstName { get; set; } = default!;
    /// <summary>
    /// 姓拼音
    /// </summary>
    [SugarColumn(ColumnDescription = "名拼音", Length = 20)]
    public string LastName { get; set; } = default!;
    /// <summary>
    /// 类型 
    /// </summary>
    [SugarColumn(ColumnDescription = "类型")]
    public PersonTypeEnum Type { get; set; }
    /// <summary>
    /// 年龄(儿童年龄必须要)
    /// </summary>
    [SugarColumn(ColumnDescription = "年龄")]
    public int Age { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [SugarColumn(ColumnDescription = "是否删除")]
    public bool IsDelete { get; set; }
}

public partial record OrderRoomPersonDo
{

    public OrderRoomPersonDo(string orderNum, long orderRoomId, int roomIndex, string firstName, string lastName, PersonTypeEnum type, int age)
    {
        OrderNum = orderNum;
        OrderRoomId = orderRoomId;
        RoomIndex = roomIndex;
        FirstName = firstName;
        LastName = lastName;
        Type = type;
        Age = age;
    }

    public static OrderRoomPersonDo Create(string orderNum, long orderRoomId, int roomIndex, string firstName, string lastName, PersonTypeEnum type, int age)
    {
        return new OrderRoomPersonDo(orderNum, orderRoomId, roomIndex, firstName, lastName, type, age);
    }
}