namespace YueJia.Ebk.Application.Contracts.OrderApp.Dto;

/// <summary>
/// 订单详情DTO
/// </summary>
public record OrderDetailDto
{

    /// <summary>
    /// 订单ID
    /// </summary>
    [JsonConverter(typeof(LongToStringConverter))]
    public long Id { get; set; }
    /// <summary>
    /// 预订号
    /// </summary>
    public string OrderNum { get; set; } 

    public string HotelCode { get; set; }

    /// <summary>
    /// 酒店名称
    /// </summary>
    public string HotelName { get; set; } 
    /// <summary>
    /// 酒店名称(英文名)
    /// </summary>

    public string HotelNameEn { get; set; }

    /// <summary>
    ///酒店地址
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    ///酒店电话
    /// </summary>
    public string TelPhone { get; set; }

    /// <summary>
    /// 房型名称
    /// </summary>
    public string HotelRoomTitle { get; set; }

    /// <summary>
    /// 床型
    /// </summary>
    public string BedType { get; set; } 

    public DateTime CheckInDate { get; set; }
    /// <summary>
    /// 离店日期
    /// </summary>
    public DateTime CheckOutDate { get; set; }



    /// <summary>
    /// 订房间数
    /// </summary>
    public int RoomNumber { get; set; }
    /// <summary>
    /// 几晚
    /// </summary>
    public int HowManyNights { get; set; }


    /// <summary>
    /// 预订单状态
    /// </summary>
    public BookingStateTypeEnum State { get; set; } = default!;

    /// <summary>
    /// 预订单状态描述
    /// </summary>

    public string StateName
    {
        get
        {
            return State.ToDescription();
        }
    }
   



    public DateTime CreateTime { get; set; }



    /// <summary>
    /// 餐食类型
    /// </summary>
    public BreakfastTypeEnum BreakfastType { get; set; }

    /// <summary>
    /// 餐食类型描述
    /// </summary>
    public string BreakfastTypeName
    {
        get { return BreakfastType.ToDescription(); }

    }

    /// <summary>
    /// 订单总金额
    /// </summary>
    public decimal CostAmount { get; set; } = default!;

    /// <summary>
    /// 客户备注
    /// </summary>
    public string Remark { get; set; }



    public string CountryName { get; set; }
    public string CityName { get; set; }


    /// <summary>
    /// 酒店确认号
    /// </summary>
    public string HotelConfirmNum { get; set; } = default!;



}

public class OrderPersonDto {
    /// <summary>
    /// 预订号
    /// </summary>
    public string OrderNum { get; set; } = default!;
    /// <summary>
    /// 房间序号 从第1间 开始
    /// </summary>
    public int RoomIndex { get; set; }
    /// <summary>
    ///名拼音
    /// </summary>
    public string FirstName { get; set; } = default!;
    /// <summary>
    /// 姓拼音
    /// </summary>
    public string LastName { get; set; } = default!;
    /// <summary>
    /// 类型 
    /// </summary>
    public string TypeString { get; set; }
    /// <summary>
    /// 年龄(儿童年龄必须要)
    /// </summary>
    public int Age { get; set; }
}



/// <summary>
/// 房间信息
/// </summary>
public class OrderDailyPriceDto
{
    /// <summary>
    /// 当前日期
    /// </summary>
    public DateTime CurrentDate { get; set; }
    /// <summary>
    /// 当前日期
    /// </summary>
    public string CurrentDateString
    {
        get {
            return CurrentDate.ToString("yyyy-MM-dd");
        }
    }

    /// <summary>
    /// 当日价格
    /// </summary>
    public decimal DayPrice { get; set; }
}


public class OrderLogDto {

    public string OrderNum { get; set; } 

    /// <summary>
    /// 当前日期
    /// </summary>
    public DateTime? CreateTime { get; set; }
    /// <summary>
    /// 当前日期
    /// </summary>
    public string CreateTimetring
    {
        get
        {
            string Result = "*";
            if (CreateTime!=null) {
                Result = CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return Result;
        }
    }
    public string Describe { get; set; }
}
