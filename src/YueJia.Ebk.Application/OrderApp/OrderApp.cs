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
        //var (isSuccess, searchCodeStr) = CompressedEncryptor.Decrypt(cmd.SearchCode, SecretKeyConst.key, SecretKeyConst.iv);
        //if (!isSuccess || string.IsNullOrWhiteSpace(searchCodeStr) || !searchCodeStr.Contains("|")) throw new InvalidOperationException("查价唯一值解密失败！");
        //var splitArray = searchCodeStr.Split('|');
        //var dailyPriceIds = splitArray[0].Split(',').Select(long.Parse).ToList();
        //var dailyInventoryIds = (splitArray[1]).Split(',').Select(long.Parse).ToList();



        //解密查价唯一值
        var (isSuccess, searchCodeStr) = CompressedEncryptor.Decrypt(cmd.SearchCode, SecretKeyConst.key, SecretKeyConst.iv);
        if (!isSuccess || string.IsNullOrWhiteSpace(searchCodeStr)) throw new InvalidOperationException("请勿篡改查价唯一码!");
        //解析查价唯一码
        var searchCode = JsonUtils.AnalysisSearchCode(searchCodeStr);
        if (searchCode is null || searchCode.DailyInventoryIds.Count == 0 || searchCode.DailyPriceIds.Count == 0) throw new InvalidOperationException("查价唯一码无效!");




        var LockKey = await db.Queryable<DailyInventoryDo>()
            .InnerJoin<HotelRoomDo>((t, r) => t.RoomId == r.Id)
            .ClearFilter<ITenantIdFilter>()
            .With(SqlWith.NoLock)
            .Where((t, r) => t.Id == searchCode.DailyInventoryIds.FirstOrDefault())
            .Select((t, r) => $"{r.HotelId}_{r.HotelCode}_{r.RoomType}").SingleAsync() ?? throw new InvalidOperationException("生成锁Key失败！");




        await DistributedLockService.LockAsync(LockKey, async () =>
        {

            //按库存ID擦查询库存集合
            var dailyInventoryDos = await db.Queryable<DailyInventoryDo>().ClearFilter<ITenantIdFilter>().Where(t => searchCode.DailyInventoryIds.Contains(t.Id) && t.IsEnable == YesOrNoType.Yes).ToListAsync();

            //按条件筛选：库存数、日期范围
            var FilterData = dailyInventoryDos.Where(t => t.InventoryNum >= cmd.RoomNum && t.CurrentDate >= cmd.CheckInDate.Date && t.CurrentDate < cmd.CheckOutDate.Date).ToList();
            //验证库存数是否足够
            bool areEqual = searchCode.DailyInventoryIds.Count == FilterData.Count && searchCode.DailyInventoryIds.All(id => dailyInventoryDos.Any(e => e.Id == id));
            if (!areEqual) throw new InvalidOperationException("库存数不足！");

            var dailyPriceDos = await db.Queryable<DailyPriceDo>().ClearFilter<ITenantIdFilter>().Where(t => searchCode.DailyPriceIds.Contains(t.Id) && t.IsEnable == YesOrNoType.Yes).ToListAsync();
            //收集价格计划
            var pricePlanIds = dailyPriceDos.Select(t => t.PricePlanId).Distinct().ToList();


            var hotel = await db.Queryable<HotelRoomDo>()
                 .InnerJoin<PricePlanDo>((r, p) => r.Id == p.HotelRoomId)
                 .Where((r, p) => r.Id == p.HotelRoomId && pricePlanIds.Contains(p.Id))
                 //.Where((r, p) => r.HotelCode == cmd.HotelCode && r.Id == p.HotelRoomId && pricePlanIds.Contains(p.Id))
                 //.Where((r, p) => r.MaximumNumberOfPeople >= (qry.AdultNumber + qry.ChildNumber) && r.AdultLimit >= qry.AdultNumber && r.ChildLimit >= qry.ChildNumber)
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
                     p.CreatedbyName,
                     r.MaximumNumberOfPeople,
                     r.AdultLimit,
                     r.ChildLimit,
                 })
                 .SingleAsync() ?? throw new InvalidOperationException("未找到符合条件的房间！");

            //验证 客户下单数据、与服务端数据是否一致
            if (cmd.RoomNum != cmd.RoomList.Count) throw new InvalidOperationException("下单房间数不匹配,请勿非法操作！");

            if (cmd.HotelCode.Trim().ToLower() != hotel.HotelCode.Trim().ToLower() || cmd.OtaRoomCode.Trim().ToLower() != hotel.RoomCode.Trim().ToLower()) throw new InvalidOperationException("下单酒店或房间不匹配,请勿非法操作！");

            //if (cmd.SalePrice != 9000) throw new InvalidOperationException("价格已发生变化,请重新下单！");

            if (cmd.IsBreakfast != (hotel.BreakfastType == BreakfastTypeEnum.Breakfast ? true : false)) throw new InvalidOperationException("餐食已发生变化,请重新下单！");





            var customerName = cmd.RoomList.Where(room => room.PersonList != null && room.PersonList.Any())
            .Select(room => $"{room.PersonList.Where(t => t.Type == PersonTypeEnum.Adult).First().FirstName}/{room.PersonList.Where(t => t.Type == PersonTypeEnum.Adult).First().LastName}").ToList();

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
                                     customerName: string.Join(",", customerName),
                                           remark: string.Empty,
                                      createdbyId: hotel.CreatedbyId!,
                                    createdbyName: hotel.CreatedbyName!,
                                         tenantId: hotel.TenantId ?? 0);



            List<OrderRoomDo> orderRoomDos = new List<OrderRoomDo>();
            List<OrderRoomPersonDo> orderRoomPersonDos = new List<OrderRoomPersonDo>();
            List<OrderRoomDailyPriceDetailDo> orderRoomDailyPriceDetailDos = new List<OrderRoomDailyPriceDetailDo>();
            foreach (var item in cmd.RoomList)
            {


                //验证房间人数上限要求
                if (hotel.MaximumNumberOfPeople < (item.AdultNumber + item.ChildNumber) || hotel.AdultLimit < item.AdultNumber || hotel.ChildLimit < item.ChildNumber
                         ) throw new InvalidOperationException($"房间最大人数上限:{hotel.MaximumNumberOfPeople},成人:{hotel.AdultLimit},儿童:{hotel.ChildLimit},人数超出限制！");




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
         timeout: TimeSpan.FromSeconds(3),
         timeoutMethod: () => throw new InvalidOperationException("下单超时，请重试!"));

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
