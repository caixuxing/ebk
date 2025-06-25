using LiteDB;
using MongoDB.Driver;
using YueJia.Ebk.Application.Contracts.OrderApp;
using YueJia.Ebk.Application.Contracts.OrderApp.Commands;
using YueJia.Ebk.Application.Contracts.OrderApp.Dto;
using YueJia.Ebk.Application.Contracts.OrderApp.Qry;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.AggRoot;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Order;
using YueJia.Ebk.Domain.SysUser;
using YueJia.Ebk.Infrastructure.DistributedLock;

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


    public async Task<bool> CreateOrderAsync(CreateOrderCmd cmd)
    {
        var Ids = Common.AnalysisSearchCode(cmd.SearchCode);
        var hotelQuoteObj = db.Queryable<HotelQuoteDo>().Where(vv => vv.Id == Ids.First()).ToList().First();
        if (Ids.Count != cmd.NightNumber)
        {
            throw new InvalidOperationException("参数错误");
        }
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
                                                 .Where((x1, x2) => x1.Id == Id)
                                                .Select((x1, x2) => x2).ToListAsync();
                if (_dailyInventoryList.Count != 1)
                {
                    throw new InvalidOperationException("库存未发现");
                }
                var _dailyPriceList = await db.Queryable<HotelQuoteDo>()
                                              .InnerJoin<DailyPriceDo>((x1, x2) => x1.DailyPriceId == x2.Id && x2.IsEnabled == YesOrNoType.Yes)
                                              .Where((x1, x2) => x1.Id == Id)
                                              .Select((x1, x2) => x2).ToListAsync();
                if (_dailyPriceList.Count != 1)
                {
                    throw new InvalidOperationException("报价未发现");
                }
                DailyInventoryList.AddRange(_dailyInventoryList);
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
                                         tenantId: DailyInventoryList.First().TenantId.Value);

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


            //插入订单
            await db.Insertable(orderDo).ExecuteReturnSnowflakeIdListAsync();

            //订单房间入住人信息
            await db.Insertable(orderRoomPersonList).ExecuteCommandAsync();

            //订单房间每天价格明细
            await db.Insertable(orderRoomDailyPriceDetailDos).ExecuteCommandAsync();

            //库存扣减
            foreach (var item in DailyInventoryList)
            {
                var NewInventoryNum = item.InventoryNum - cmd.RoomList.Count;
                var NewIsEnabled = NewInventoryNum > 0 ? YesOrNoType.Yes : YesOrNoType.No;

                await db.Updateable<DailyInventoryDo>()
                         .SetColumns(it => new DailyInventoryDo()
                         {
                             Version = it.Version + 1,
                             LastModifiedTime = DateTime.Now,
                             InventoryNum = NewInventoryNum,
                             IsEnabled = NewIsEnabled,
                         })
                         .Where(it => it.Id == item.Id)
                         .ExecuteCommandAsync();
            }
            return true;
        });
        return true;

    }

    public async Task<PageData<IEnumerable<OrderPageListDto>>> QueryOrderPageAsync(OrderPageListFilterQry qry)
    {
        RefAsync<int> total = 0;
        var query = OrderRepo.AsQueryable().WhereDeptFilter(CurrentUserApp, db)
                             .LeftJoin<HotelPublishDo>((x1, x2) =>  x1.UserHotelId == x2.Id  && x1.TenantId == x2.TenantId )
                             .With(SqlWith.NoLock)
                             .Where((x1, x2) => x1.CreatedbyId == qry.UserId)
                             .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelCode), (x1, x2) => x2.HotelCode == qry.HotelCode)
                             .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelName), (x1, x2) => x2.HotelName.contains(qry.HotelName)  || x2.HotelNameEn.contains(qry.HotelName))
                             .Select((x1, x2) => new OrderPageListDto()
                            {

                                 OrderNum = x1.OrderNum,
                                 CountryName = x2.CountryName,
                                 CityName = x2.CityName,
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


    public async Task<OrderDetailDto> OrderDetailByIdAsync(long id)
    {
        //var order = await db.Queryable<OrderDo>()
        //    .InnerJoin<HotelPublishDo>((t, p) => t.HotelCode == p.HotelCode && t.UserHotelId == p.Id)
        //    .Where((t, p) => t.Id == id)
        //    .Select((t, p) => new OrderDetailDto()
        //    {
        //        HotelName = p.HotelName,
        //        HotelNameEn = p.HotelNameEn,
        //        //RoomName = t.RoomName,
        //        CustRemark = t.Remark,
        //        CheckInDate = t.CheckInDate,
        //        CheckOutDate = t.CheckOutDate,
        //        //BedTypeName = t.BedName,
        //        //BookingDate = t.BookingDate,
        //        //TotalAmount = t.TotalAmount,
        //        Address = $"{p.Address}(${p.AddressEn})",
        //        BreakfastType = t.BreakfastType,
        //        State = t.State,
        //        OrderNum = t.OrderNum,
        //        Contact = p.TelPhone,
        //        Id = t.Id,
        //        Area = $"[{p.CountryIosCode}]{p.CountryName}/{p.CityName}",
        //        HotelConfirmNum = t.HotelConfirmNum

        //    }).SingleAsync() ?? throw new InvalidOperationException("订单不存在！");




        //var roomList = await db.Queryable<OrderRoomDo>()
        //    .InnerJoin<OrderRoomPersonDo>((t, t1) => t.OrderNum == t1.OrderNum && t1.OrderRoomId == t.Id)
        //    .InnerJoin<OrderRoomDailyPriceDetailDo>((t, t1, t2) => t2.OrderNum == t.OrderNum && t2.OrderRoomId == t.Id)
        //    .Where((t, t1, t2) => t.OrderNum == order.OrderNum)
        //    .Select((t, t1, t2) => new
        //    {
        //        t.Id,
        //        t.OrderNum,
        //        t.RoomName,
        //        t.BedName,
        //        t.PricePlanName,
        //        Name = $"{t1.LastName}/{t1.FirstName}",
        //        t1.Type,
        //        t2.CurrentDate,
        //        t2.DayPrice
        //    })
        //    .ToListAsync();
        //order.HotelRoomInfo = roomList.GroupBy(t => t.Id)
        //     .Select(g => new HotelRoomInfoOB()
        //     {
        //         PricePlanTitle = g.FirstOrDefault()?.PricePlanName ?? string.Empty,
        //         Adult = g.Where(t => t.Type == PersonTypeEnum.Adult).Select(t => t.Name).ToList(),
        //         Child = g.Where(t => t.Type == PersonTypeEnum.Child).Select(t => t.Name).ToList(),
        //         DailyPrice = g.ToDictionary(t => t.CurrentDate.ToString("yyyy-MM-dd"), t => t.DayPrice)

        //     }).ToList();
        return null;
    }

    public async Task<bool> SaveOrderConfirmNumAsync(long id, string confirmNum)
    {
        var entity = await OrderRepo.GetByIdAsync(id) ?? throw new InvalidOperationException("订单不存在！");
        entity.HotelConfirmNum = confirmNum;
        await OrderRepo.AsUpdateable(entity).UpdateColumns(it => new { it.HotelConfirmNum, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
            .EnableDiffLogEvent()
            .ExecuteCommandWithOptLockAsync();
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
