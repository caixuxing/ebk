using YueJia.Ebk.Application.Contracts.OrderApp;
using YueJia.Ebk.Application.Contracts.OrderApp.Commands;
using YueJia.Ebk.Application.Contracts.OrderApp.Dto;
using YueJia.Ebk.Application.Contracts.OrderApp.Qry;
using YueJia.Ebk.Domain.AggRoot;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Order;
using YueJia.Ebk.Domain.Shared.Const;
using YueJia.Ebk.Infrastructure.DistributedLock;

namespace YueJia.Ebk.Application.OrderApp;


[DisableValidation]
public class OrderApp : ApplicationService, IOrderApp
{

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();
    private ISimpleClient<OrderDo> OrderRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<OrderDo>>();


    private IDistributedLockService DistributedLockService => LazyServiceProvider.LazyGetRequiredService<IDistributedLockService>();


    public async Task<bool> CreateOrderAsync(CreateOrderCmd cmd)
    {
        //验证
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrderCmd>>().ValidateAndThrowAsync(cmd);

        //连住天数
        int continuousStayDays = (cmd.CheckOutDate.Date - cmd.CheckInDate.Date).Days;
        //提前天数
        int advanceDays = (cmd.CheckOutDate.Date - DateTime.Now.Date).Days;

        //解密查价唯一值
        var (isSuccess, searchCodeStr) = CompressedEncryptor.Decrypt(cmd.SearchCode, SecretKeyConst.key, SecretKeyConst.iv);
        if (!isSuccess || string.IsNullOrWhiteSpace(searchCodeStr) || !searchCodeStr.Contains("|")) throw new InvalidOperationException("查价唯一值解密失败！");
        var splitArray = searchCodeStr.Split('|');
        var dailyPriceIds = splitArray[0].Split(',').Select(long.Parse).ToList();
        var dailyInventoryIds = (splitArray[1]).Split(',').Select(long.Parse).ToList();

        //按库存ID擦查询库存集合
        var dailyInventoryDos = await db.Queryable<DailyInventoryDo>().ClearFilter<ITenantIdFilter>().Where(t => dailyInventoryIds.Contains(t.Id)).ToListAsync();


        //按条件筛选：库存数、日期范围
        var FilterData = dailyInventoryDos.Where(t => t.InventoryNum >= cmd.RoomNum && t.CurrentDate >= cmd.CheckInDate.Date && t.CurrentDate < cmd.CheckOutDate.Date).ToList();
        //验证库存数是否足够
        bool areEqual = dailyInventoryIds.Count == FilterData.Count && dailyInventoryIds.All(id => dailyInventoryDos.Any(e => e.Id == id));
        if (!areEqual) throw new InvalidOperationException("库存数不足！");


        var dailyPriceDos = await db.Queryable<DailyPriceDo>().ClearFilter<ITenantIdFilter>().Where(t => dailyPriceIds.Contains(t.Id)).ToListAsync();
        //收集价格计划
        var pricePlanIds = dailyPriceDos.Select(t => t.PricePlanId).Distinct().ToList();



        var qry = cmd.RoomList.FirstOrDefault() ?? throw new InvalidOperationException("房间列表不能为空！");




        var hotel = await db.Queryable<HotelRoomDo>()
             .InnerJoin<PricePlanDo>((r, p) => r.Id == p.HotelRoomId)
             .Where((r, p) => r.HotelCode == cmd.HotelCode && r.Id == p.HotelRoomId && pricePlanIds.Contains(p.Id))
             .Where((r, p) => r.MaximumNumberOfPeople >= (qry.AdultNumber + qry.ChildNumber) && r.AdultLimit >= qry.AdultNumber && r.ChildLimit >= qry.ChildNumber)
             .Where((r, p) => p.ContinuousStayDays <= continuousStayDays && p.DaysInAdvance <= advanceDays)
             .Select((r, p) => new
             {
                 r.HotelId,
                 RoomCode = r.RoomType,
                 RoomName = r.HotelRoomTitle ?? string.Empty,
                 r.BedType,
                 HotelCode = r.HotelCode,
                 BreakfastType = p.BreakfastType,
                 PricePlanId = p.Id,
                 p.PricePlanTitle,
                 p.TenantId,
                 p.CreatedbyId,
                 p.CreatedbyName
             })
             .SingleAsync() ?? throw new InvalidOperationException("未找到符合条件的房间！");



        string lockKey = $"{hotel.PricePlanId.ToString()}";

        await DistributedLockService.LockAsync(lockKey,
            async () =>
        {

            //验证 客户下单数据、与服务端数据是否一致

            //创建订单
            var orderDo = OrderDo.Create(orderNum: cmd.OrderCode,
                                      bookingDate: DateTime.Now,
                                            state: BookingStateTypeEnum.ToBeConfirmed,
                                        hotelCode: cmd.HotelCode,
                                      userHotelId: hotel.HotelId,
                                         roomCode: cmd.OtaRoomCode,
                                         roomName: hotel.RoomName,
                                          bedCode: hotel.BedType.GetHashCode().ToString(),
                                          bedName: $"{hotel.BedType.ToDescription()} {hotel.BedType.ToString()}",
                                      checkInDate: cmd.CheckInDate,
                                     checkOutDate: cmd.CheckOutDate,
                                      totalAmount: cmd.SalePrice,
                                  hotelConfirmNum: string.Empty,
                              numberOfRoomsBooked: cmd.RoomNum,
                                    howManyNights: continuousStayDays,
                                     customerName: string.Empty,
                                           remark: string.Empty,
                                      createdbyId: hotel.CreatedbyId!,
                                    createdbyName: hotel.CreatedbyName!,
                                         tenantId: hotel.TenantId ?? 0);


            //创建订单房间、创建订单房间入住人信息

            List<OrderRoomDo> orderRoomDos = new List<OrderRoomDo>();
            List<OrderRoomPersonDo> orderRoomPersonDos = new List<OrderRoomPersonDo>();
            List<OrderRoomDailyPriceDetailDo> orderRoomDailyPriceDetailDos = new List<OrderRoomDailyPriceDetailDo>();
            foreach (var item in cmd.RoomList)
            {
                var orderRoomDo = OrderRoomDo.Create(orderNum: cmd.OrderCode,
                                                     roomName: hotel.RoomName,
                                                     roomCode: cmd.OtaRoomCode,
                                                      bedCode: hotel.BedType.GetHashCode().ToString(),
                                                      bedName: $"{hotel.BedType.ToDescription()} {hotel.BedType.ToString()}",
                                                  pricePlanId: hotel.PricePlanId,
                                                pricePlanName: hotel.PricePlanTitle!,
                                                breakfastType: hotel.BreakfastType);

                item.PersonList.ForEach(child => orderRoomPersonDos.Add(OrderRoomPersonDo.Create(orderNum: cmd.OrderCode,
                                                                                              orderRoomId: orderRoomDo.Id,
                                                                                                roomIndex: child.RoomIndex,
                                                                                                firstName: child.FirstName,
                                                                                                 lastName: child.LastName,
                                                                                                     type: child.Type,
                                                                                                      age: child.Age)));
                //创建订单房间每天价格明细
                orderRoomDailyPriceDetailDos.AddRange(dailyPriceDos.Select(x => new OrderRoomDailyPriceDetailDo
                {
                    OrderNum = cmd.OrderCode,
                    OrderRoomId = orderRoomDo.Id,
                    CurrentDate = x.CurrentDate,
                    DayPrice = x.Price,
                }).ToList());

                orderRoomDos.Add(orderRoomDo);
            }


            await DbTransaction.ExecuteInTransactionAsync(db, async () =>
            {
                //插入订单
                await db.Insertable(orderDo).ExecuteReturnSnowflakeIdListAsync();
                //插入订单房间
                await db.Insertable(orderRoomDos).ExecuteCommandAsync();

                //订单房间入住人信息
                await db.Insertable(orderRoomPersonDos).ExecuteCommandAsync();

                //订单房间每天价格明细
                await db.Insertable(orderRoomDailyPriceDetailDos).ExecuteCommandAsync();
                return true;
            });

        },
         timeout: default,
         timeoutMethod: () => throw new Exception("操作超时，请重试!"));


        return true;

    }

    public async Task<PageData<IEnumerable<OrderPageListDto>>> QueryOrderPageAsync(OrderPageListFilterQry qry)
    {
        RefAsync<int> total = 0;
        var query = OrderRepo.AsQueryable()
            .LeftJoin<HotelPublishDo>((t, h) => t.HotelCode == h.HotelCode && t.UserHotelId == h.Id)
            .With(SqlWith.NoLock)
            .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelCode), t => t.HotelCode == qry.HotelCode)
            .Select((t, h) => new OrderPageListDto()
            {
                BedName = t.BedName,
                BookingDate = t.BookingDate,
                CheckInDate = t.CheckInDate,
                CheckOutDate = t.CheckOutDate,
                CustomerName = t.CustomerName,
                HotelConfirmNum = t.HotelConfirmNum,
                HotelName = h.HotelName,
                HotelNameEn = h.HotelNameEn,
                HowManyNights = t.HowManyNights,
                NumberOfRoomsBooked = t.NumberOfRoomsBooked,
                OrderNum = t.OrderNum,
                RoomName = t.RoomName,
                State = t.State,
                TotalAmount = t.TotalAmount,
                Id = t.Id
            });
        var data = await query.ToPageListAsync(qry.PageIndex, qry.PageSize, total);
        return new PageData<IEnumerable<OrderPageListDto>>(total, qry.PageSize, qry.PageIndex, data);
    }
}
