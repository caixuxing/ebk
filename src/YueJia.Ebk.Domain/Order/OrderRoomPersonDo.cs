namespace YueJia.Ebk.Domain.Order;


/// <summary>
/// 订单房间入住人信息
/// </summary>
[SugarTable("OrderPerson", "订单房间入住人信息")]
public partial record OrderPersonDo : EntityBaseId
{
    public OrderPersonDo() { }
    /// <summary>
    /// 预订号
    /// </summary>
    [SugarColumn(ColumnDescription = "预订号", Length = 50)]
    public string OrderNum { get; set; } = default!;

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

 }

public partial record OrderPersonDo
{

    public OrderPersonDo(string orderNum,  int roomIndex, string firstName, string lastName, PersonTypeEnum type, int age)
    {
        OrderNum = orderNum;
        RoomIndex = roomIndex;
        FirstName = firstName;
        LastName = lastName;
        Type = type;
        Age = age;
    }

    public static OrderPersonDo Create(string orderNum,  int roomIndex, string firstName, string lastName, PersonTypeEnum type, int age)
    {
        return new OrderPersonDo(orderNum,  roomIndex, firstName, lastName, type, age);
    }
}