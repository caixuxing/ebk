using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using YueJia.Ebk.Application.Contracts.OrderApp;
using YueJia.Ebk.Application.Contracts.OrderApp.Commands;
using YueJia.Ebk.Application.Contracts.OrderApp.Dto;
using YueJia.Ebk.Application.Contracts.OrderApp.Qry;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.AggRoot;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Order;
using YueJia.Ebk.Domain.Shared.Const;
using YueJia.Ebk.Domain.Other;
using YueJia.Ebk.Domain.Shared.Dto;
using YueJia.Ebk.Domain.SysUser;
using YueJia.Ebk.Infrastructure.DistributedLock;
using ZstdSharp.Unsafe;

namespace YueJia.Ebk.Application.OrderApp;


[DisableValidation]
public class OrderApp : ApplicationService, IOrderApp
{

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();
    private ISimpleClient<OrderDo> OrderRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<OrderDo>>();

    private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();

    private ISimpleClient<SysUserDo> SysUserRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<SysUserDo>>();


    private IDistributedLockService DistributedLockService => LazyServiceProvider.LazyGetRequiredService<IDistributedLockService>();

    private IMongoDatabase MongoDb => LazyServiceProvider.LazyGetRequiredService<IMongoDatabase>();

    private ISqlSugarClient SqlSugarClient => LazyServiceProvider.GetRequiredKeyedService<ISqlSugarClient>(DbConst.YueJiaSysDb);


    public async Task<bool> CreateOrderAsync(CreateOrderCmd cmd)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrderCmd>>().ValidateAndThrowAsync(cmd);
        var Ids = Common.AnalysisSearchCode(cmd.SearchCode);
        var hotelQuoteObj = await db.Queryable<HotelQuoteDo>().SingleAsync(vv => vv.Id == Ids.First());
        if (Ids.Count != cmd.NightNumber) throw new InvalidOperationException("参数错误");



        var hotel = await db.Queryable<HotelPublishDo>()
     .InnerJoin<HotelRoomDo>((x1, x2) => x1.Id == x2.HotelId)
     .InnerJoin<SysUserDo>((x1, x2, x3) => SqlFunc.ToInt64(x1.CreatedbyId) == x3.Id)
     .With(SqlWith.NoLock)
     .Where((x1, x2, x3) => x2.Id == hotelQuoteObj.UserRoomId)
     .Select((x1, x2, x3) => new { x1.HotelName, x1.HotelNameEn, x2.HotelRoomTitle, x2.BedType, x3.ContactPhone, x3.Email })
     .SingleAsync();



        await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {




            List<DailyInventoryDo> DailyInventoryList = new List<DailyInventoryDo>();
            List<DailyPriceDo> DailyPriceList = new List<DailyPriceDo>();

            foreach (var Id in Ids)
            {
                var _dailyInventoryList = await db.Queryable<HotelQuoteDo>()
                                                 .InnerJoin<DailyInventoryDo>((x1, x2) => x1.DailyInventoryId == x2.Id &&
                                                                                          x2.InventoryNum >= cmd.RoomList.Count &&
                                                                                          x2.IsEnabled == YesOrNoType.Yes)

                                                 .With($"WITH(ROWLOCK, UPDLOCK, HOLDLOCK)")
                                                 .Where((x1, x2) => x1.Id == Id)
                                                .Select((x1, x2) => x2).SingleAsync() ?? throw new InvalidOperationException("库存未发现");

                _dailyInventoryList.InventoryNum = _dailyInventoryList.InventoryNum - cmd.RoomList.Count;
                _dailyInventoryList.IsEnabled = _dailyInventoryList.InventoryNum > 0 ? YesOrNoType.Yes : YesOrNoType.No;
                _dailyInventoryList.Version = _dailyInventoryList.Version + 1;
                _dailyInventoryList.LastModifiedTime = DateTime.Now;


                var _dailyPriceList = await db.Queryable<HotelQuoteDo>()
                                              .InnerJoin<DailyPriceDo>((x1, x2) => x1.DailyPriceId == x2.Id && x2.IsEnabled == YesOrNoType.Yes)
                                              .Where((x1, x2) => x1.Id == Id)
                                              .Select((x1, x2) => x2).ToListAsync();
                if (_dailyPriceList.Count != 1)
                {
                    throw new InvalidOperationException("报价未发现");
                }
                DailyInventoryList.Add(_dailyInventoryList);
                DailyPriceList.AddRange(_dailyPriceList);
            }
            var costAmount = DailyPriceList.Sum(vv => vv.Price * cmd.RoomList.Count);
            if (cmd.SalePrice < costAmount && (costAmount - cmd.SalePrice) > 50)
            {
                throw new InvalidOperationException($@"价格变化 {cmd.SalePrice}<===>{costAmount}");
            }

            //1):订单主表
            var orderDo = OrderDo.Create(orderNum: cmd.OrderCode,
                                            state: BookingStateTypeEnum.BookConfirmed,
                                      userHotelId: hotelQuoteObj.UserHotelId,
                                         roomCode: hotelQuoteObj.RoomCode,
                                    breakfastType: hotelQuoteObj.BreakfastType,
                                      checkInDate: Convert.ToDateTime(cmd.CheckInDate),
                                     checkOutDate: Convert.ToDateTime(cmd.CheckOutDate),
                                      saleAmount: cmd.SalePrice,
                                      costAmount: costAmount,
                                  hotelConfirmNum: string.Empty,
                              roomNumber: cmd.RoomList.Count,
                                    howManyNights: (Convert.ToDateTime(cmd.CheckOutDate) - Convert.ToDateTime(cmd.CheckInDate)).Days,
                                           remark: cmd.SpecialRemark ?? string.Empty,
                                      createdbyId: hotelQuoteObj.UserId,
                                         tenantId: DailyInventoryList.FirstOrDefault()?.TenantId ?? 0);

            List<OrderPersonDo> orderRoomPersonList = new List<OrderPersonDo>();

            foreach (var ele in cmd.RoomList)
            {
                int roomIndex = cmd.RoomList.IndexOf(ele) + 1;
                orderRoomPersonList.AddRange(ele.PersonList.Select(vv => OrderPersonDo.Create(orderNum: cmd.OrderCode,
                                                                                                roomIndex: roomIndex,
                                                                                                firstName: vv.FirstName,
                                                                                                 lastName: vv.LastName,
                                                                                                     type: vv.Type,
                                                                                                      age: vv.Age)).ToList());
            }
            List<OrderDailyPriceDo> orderRoomDailyPriceDetailDos = DailyPriceList.Select(x => new OrderDailyPriceDo
            {
                OrderNum = cmd.OrderCode,
                CurrentDate = x.CurrentDate,
                DayPrice = x.Price,
            }).ToList();


            var taskPublishData = new TaskPublishDto
            {
                OrderCode = cmd.OrderCode,
                HotelName = hotel.HotelName,
                HotelNameEn = hotel.HotelNameEn,
                RecipientAccount = hotel.Email ?? string.Empty,
                RoomNmae = hotel.HotelRoomTitle ?? string.Empty,
                BedType = hotel.BedType.ToDescription(),
                CheckInDate = cmd.CheckInDate,
                CheckOutDate = cmd.CheckOutDate,
                AdultNumber = cmd.RoomList.FirstOrDefault()?.AdultNumber ?? 0,
                ChildNumber = cmd.RoomList.FirstOrDefault()?.ChildNumber ?? 0,
                CostAmount = costAmount,
                RoomNumber = cmd.RoomList.Count,
                PersonName = cmd.RoomList.SelectMany(t => t.PersonList)
             .Where(t => t.Type == PersonTypeEnum.Adult)
             .Select(t => $"{t.LastName}/{t.FirstName}")
             .ToList()
            };


            //推送任数据包装

            //创建推送任务
            var taskPublishDo = new List<TaskPublishDo>() {
             TaskPublishDo.Create(
                 PushTypeEnum.Email,
                 cmd.OrderCode,
                 System.Text.Json.JsonSerializer.Serialize(taskPublishData, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) }),
                 0,
                 null,
                 TaskPushStatusTypeEnum.Pending,
                 DateTime.Now),
             TaskPublishDo.Create(
                 PushTypeEnum.SMS,
                 cmd.OrderCode,
                 System.Text.Json.JsonSerializer.Serialize(taskPublishData with{ RecipientAccount=hotel.ContactPhone??string.Empty }, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) }),
                 0,
                 null,
                 TaskPushStatusTypeEnum.Pending,
                 DateTime.Now)
            };


            //插入订单
            await db.Insertable(orderDo).ExecuteReturnSnowflakeIdListAsync();

            //订单房间入住人信息
            await db.Insertable(orderRoomPersonList).ExecuteCommandAsync();

            //订单房间每天价格明细
            await db.Insertable(orderRoomDailyPriceDetailDos).ExecuteCommandAsync();

            //库存扣减
            await db.Updateable(DailyInventoryList).UpdateColumns(it => new { it.InventoryNum, it.IsEnabled, it.LastModifiedTime, it.Version })
           .ExecuteCommandAsync();

            //插入推送任务
            await db.Insertable(taskPublishDo).ExecuteCommandAsync();
            return true;
        });
        return true;

    }

    public async Task<PageData<IEnumerable<OrderPageListDto>>> QueryOrderPageAsync(OrderPageListFilterQry qry)
    {
        RefAsync<int> total = 0;

        string CountryIosCode = "";
        if (qry.CountryId != null)
        {
            CountryIosCode = SqlSugarClient.Queryable<BAreaEntity>().Single(vv => vv.Id == qry.CountryId).CountryIosCode ?? "";
        }


        var query = OrderRepo.AsQueryable().WhereDeptFilter(CurrentUserApp, db)
                             .LeftJoin<HotelPublishDo>((x1, x2) => x1.UserHotelId == x2.Id && x1.TenantId == x2.TenantId)
                             .With(SqlWith.NoLock)
                             .Where((x1, x2) => x1.CreatedbyId == qry.UserId)

                             .WhereIF(!string.IsNullOrEmpty(qry.OrderNum), (x1, x2) => x1.OrderNum ==qry.OrderNum)
                             .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelCode), (x1, x2) => x2.HotelCode == qry.HotelCode)
                             .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelName), (x1, x2) => x2.HotelName.Contains(qry.HotelName)  || x2.HotelNameEn.Contains(qry.HotelName))
                             .WhereIF(!string.IsNullOrEmpty(CountryIosCode), (x1, x2) => x2.CountryIosCode == CountryIosCode)
                             .WhereIF(!string.IsNullOrEmpty(qry.CityName), (x1, x2) => x2.CityName.Contains(qry.CityName))

                             .WhereIF(qry.DateType=="A", (x1, x2) =>  x1.CreateTime>= SqlFunc.ToDate(qry.StartDate) && x1.CreateTime<= SqlFunc.ToDate(qry.EndDate).AddDays(1))
                             .WhereIF(qry.DateType == "B", (x1, x2) => x1.CheckInDate >= SqlFunc.ToDate(qry.StartDate) && x1.CheckInDate <= SqlFunc.ToDate(qry.EndDate))
                             .WhereIF(qry.DateType == "C", (x1, x2) => x1.CheckOutDate >= SqlFunc.ToDate(qry.StartDate) && x1.CheckOutDate <= SqlFunc.ToDate(qry.EndDate))


                             .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelName), (x1, x2) => x2.HotelName.Contains(qry.HotelName) || x2.HotelNameEn.Contains(qry.HotelName))
                             .Select((x1, x2) => new OrderPageListDto()
                             {

                                 OrderNum = x1.OrderNum,
                                 CountryName = x2.CountryName,
                                 CityName = x2.CityName,
                                 HotelCode = x2.HotelCode,
                                 HotelName = x2.HotelName,
                                 HotelNameEn = x2.HotelNameEn,
                                 CheckInDate = x1.CheckInDate,
                                 CheckOutDate = x1.CheckOutDate,
                                 HowManyNights = x1.HowManyNights,
                                 RoomNumber = x1.RoomNumber,
                                 CostAmount = x1.CostAmount,
                                 CreateTime = x1.CreateTime,
                                 State = x1.State,
                                 Id = x1.Id,
                                 BreakfastType = x1.BreakfastType
                             });
        var data = await query.ToPageListAsync(qry.PageIndex, qry.PageSize, total);
        return new PageData<IEnumerable<OrderPageListDto>>(total, qry.PageSize, qry.PageIndex, data);
    }


    public async Task<OrderDetailDto> OrderDetailByIdAsync(long orderId)
    {
        return await db.Queryable<OrderDo>()
                        .InnerJoin<HotelPublishDo>((x1, x2) => x1.UserHotelId == x2.Id && x1.TenantId == x2.TenantId)
                        .InnerJoin<HotelRoomDo>((x1, x2, x3) => x1.RoomCode == x3.RoomType && x1.UserHotelId == x3.HotelId && x1.TenantId == x3.TenantId)
                        .Where((x1, x2, x3) => x1.Id == orderId)
                        .Select((x1, x2, x3) => new OrderDetailDto()
                        {
                            OrderNum = x1.OrderNum,
                            HotelCode = x2.HotelCode,
                            HotelName = x2.HotelName,
                            HotelNameEn = x2.HotelNameEn,
                            TelPhone = x2.TelPhone,
                            CheckInDate = x1.CheckInDate,
                            CheckOutDate = x1.CheckOutDate,
                            HotelRoomTitle = x3.HotelRoomTitle,
                            BedType = ((int)x3.BedType).ToString(),
                            BreakfastType = x1.BreakfastType,
                            HowManyNights = x1.HowManyNights,
                            RoomNumber = x1.RoomNumber,

                            Address = x2.Address,
                            CityName = x2.CityName,
                            CountryName = x2.CountryName,
                            CostAmount = x1.CostAmount,
                            Id = x1.Id,
                            Remark = x1.Remark,
                            HotelConfirmNum = x1.HotelConfirmNum,
                            State = x1.State,
                            CreateTime = x1.CreateTime,
                        }).SingleAsync();
    }


    public async Task<List<OrderPersonDto>> GetOrderPersonList(string orderNum) {
        return await db.Queryable<OrderPersonDo>()
                       .Where(vv=> vv.OrderNum == orderNum).Select(vv => new OrderPersonDto()
                        {
                            FirstName = vv.FirstName,
                            LastName = vv.LastName,
                            Age = vv.Age,
                            OrderNum = vv.OrderNum,
                            RoomIndex = vv.RoomIndex, 
                            TypeString = vv.Type == PersonTypeEnum.Adult ? "成人" : "儿童"
                        }).ToListAsync();
    
    }


    public async Task<List<OrderDailyPriceDto>> GetOrderDailyPriceList(string orderNum)
    {
        return await db.Queryable<OrderDailyPriceDo>()
                       .Where(vv => vv.OrderNum == orderNum).Select(vv => new OrderDailyPriceDto()
                       {
                            CurrentDate = vv.CurrentDate,
                            DayPrice = vv.DayPrice,
                       }).ToListAsync();

    }

    public async Task<List<OrderLogDto>> GetOrderLogList(string orderNum)
    {
        return await db.Queryable<OrderLogDo>()
                       .Where(vv => vv.OrderNum == orderNum)
                       .OrderByDescending(vv => vv.CreateTime)
                       .Select(vv => new OrderLogDto()
                       {
                          CreateTime = vv.CreateTime,
                          Describe = vv.Describe,
                       })
                   .ToListAsync();

    }

    public async Task<bool> SetInputRemark(string orderNum, string inputRemark)
    {
        db.Insertable<OrderLogDo>(new OrderLogDo()
        {
            OrderNum = orderNum,
            CreateTime = DateTime.Now,
            Describe = inputRemark
        }).ExecuteCommand();
        return true;
    }










    public async Task<bool> SaveOrderConfirmNumAsync(long id, string confirmNum)
    {
        var entity = await OrderRepo.GetByIdAsync(id);
        entity.HotelConfirmNum = confirmNum;
        await OrderRepo.AsUpdateable(entity).UpdateColumns(it => new { it.HotelConfirmNum, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
            .EnableDiffLogEvent()
            .ExecuteCommandWithOptLockAsync();


        db.Insertable<OrderLogDo>(new OrderLogDo() { 
           OrderNum = entity.OrderNum,
            CreateTime =DateTime.Now,
             Describe = $@"更新确认号：{confirmNum}"
        }).ExecuteCommand();

        return true;

    }
}

internal static partial class MapExt
{

    /// <summary>
    /// MapUserDtoList
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    internal static ISugarQueryable<T> WhereDeptFilter<T>(this ISugarQueryable<T> query, ICurrentUserApp CurrentUserApp, ISqlSugarClient db) where T : EntityBase
    {
        if (new List<AccountTypeEnum>() { AccountTypeEnum.SysAdmin, AccountTypeEnum.SuperAdmin }.ToList().Contains(CurrentUserApp.AccountType!.Value))
        {
            return query;
        }
        if (CurrentUserApp.IsDeptAdmin)
        {
            var deptUserIds = db.Queryable<SysUserDo>().Where(x => x.DeptId == CurrentUserApp.Dept.DeptId)
                .Select(x => x.Id.ToString())
                .ToList();
            deptUserIds.Insert(0, CurrentUserApp.Id.ToString());
            return query.Where(x => deptUserIds.Contains(x.CreatedbyId!));
        }
        return query.Where(x => x.CreatedbyId == CurrentUserApp.Id);
    }
}
