using MongoDB.Driver;
using YueJia.Ebk.Application.Contracts.EbkApp;
using YueJia.Ebk.Application.Contracts.EbkApp.Dto;
using YueJia.Ebk.Application.Contracts.EbkApp.Query;
using YueJia.Ebk.Domain.AggRoot;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Shared.Const;
using YueJia.Ebk.Infrastructure.Uilts;

namespace YueJia.Ebk.Application.EbkApp;



[DisableValidation]
public class EbkApp : ApplicationService, IEbkApp
{


    private IMongoDatabase MongoDb => LazyServiceProvider.LazyGetRequiredService<IMongoDatabase>();

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();

    public async Task<HotelPriceDto> PriceCheckQry(PriceCheckQry qry)
    {

        //验证
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<PriceSearchQry>>().ValidateAndThrowAsync(qry);

        //连住天数
        int continuousStayDays = (qry.CheckOutDate.Date - qry.CheckInDate.Date).Days;
        //提前天数
        int advanceDays = (qry.CheckOutDate.Date - DateTime.Now.Date).Days;

        //解密查价唯一值
        var searchCodeStr = CompressedEncryptor.Decrypt(qry.SearchCode, SecretKeyConst.key, SecretKeyConst.iv);
        if (string.IsNullOrWhiteSpace(searchCodeStr) || !searchCodeStr.Contains("|")) throw new ArgumentException("查价唯一值解密失败！");
        var splitArray = searchCodeStr.Split('|');
        var dailyPriceIds = splitArray[0].Split(',').Select(long.Parse).ToList();
        var dailyInventoryIds = (splitArray[1]).Split(',').Select(long.Parse).ToList();

        //按库存ID擦查询库存集合
        var dailyInventoryDos = await db.Queryable<DailyInventoryDo>().ClearFilter<ITenantIdFilter>().Where(t => dailyInventoryIds.Contains(t.Id)).ToListAsync();
        //按条件筛选：库存数、日期范围
        var FilterData = dailyInventoryDos.Where(t => t.InventoryNum >= qry.RoomNum && t.CurrentDate >= qry.CheckInDate.Date && t.CurrentDate < qry.CheckOutDate.Date).ToList();
        //验证库存数是否足够
        bool areEqual = dailyInventoryIds.Count == FilterData.Count && dailyInventoryIds.All(id => dailyInventoryDos.Any(e => e.Id == id));
        if (!areEqual) throw new ArgumentException("库存数不足！");


        var dailyPriceDos = await db.Queryable<DailyPriceDo>().ClearFilter<ITenantIdFilter>().Where(t => dailyPriceIds.Contains(t.Id)).ToListAsync();
        //收集价格计划
        var pricePlanIds = dailyPriceDos.Select(t => t.PricePlanId).Distinct().ToList();

        var hotel = await db.Queryable<HotelRoomDo>()
             .InnerJoin<PricePlanDo>((r, p) => r.Id == p.HotelRoomId)
             .Where((r, p) => r.HotelCode == qry.HotelCode && r.Id == p.HotelRoomId && pricePlanIds.Contains(p.Id))
             .Where((r, p) => r.MaximumNumberOfPeople >= (qry.AdultNum + qry.ChildNum) && r.AdultLimit >= qry.AdultNum && r.ChildLimit >= qry.ChildNum)
             .Where((r, p) => p.ContinuousStayDays >= continuousStayDays && p.DaysInAdvance <= advanceDays)
             .Select((r, p) => new
             {
                 RoomCode = r.RoomType,
                 RoomName = r.HotelRoomTitle ?? string.Empty,
                 HotelCode = r.HotelCode,
                 BreakfastType = p.BreakfastType,
                 PricePlanId = p.Id.ToString(),
             })
             .SingleAsync() ?? throw new ArgumentException("未找到符合条件的房间！");
        return new HotelPriceDto()
        {
            HotelCode = hotel.HotelCode,
            RoomCode = hotel.RoomCode,
            RoomName = hotel.RoomName,
            IsBreakfast = hotel.BreakfastType.ToDescription(),
            PricePlanId = hotel.PricePlanId,
            DayPrice = dailyPriceDos.ToDictionary(t => t.CurrentDate.ToString("yyyy-MM-dd"), t => t.Price),
            TotalPrice = dailyPriceDos.Sum(t => t.Price),
            SearchCode = CompressedEncryptor.Encrypt($@"{string.Join(",", dailyPriceDos.Select(t => t.Id).ToList())}|{string.Join(",", dailyInventoryDos.Select(t => t.Id).ToList())}", SecretKeyConst.key, SecretKeyConst.iv)
        };
    }

    public async Task<IEnumerable<HotelPriceDto>> PriceSearch(PriceSearchQry qry)
    {
        //验证
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<PriceSearchQry>>().ValidateAndThrowAsync(qry);

        //连住天数
        int continuousStayDays = (qry.CheckOutDate.Date - qry.CheckInDate.Date).Days;
        //提前天数
        int advanceDays = (qry.CheckOutDate.Date - DateTime.Now.Date).Days;


        //查询符合条件的房间
        //var roomData = await MongoDb.GetCollection<HotelRoomDo>(nameof(HotelRoomDo)).Aggregate()
        //    .Lookup<HotelRoomDo, HotelPublishDo, HotelRoomWithPublishDo>(
        //              foreignCollection: MongoDb.GetCollection<HotelPublishDo>(nameof(HotelPublishDo)),
        //                 localField: r => r.HotelId,
        //                 foreignField: h => h.Id,
        //                 @as: result => result.HotelPublishe)
        //         .Unwind<HotelRoomWithPublishDo, HotelRoomWithPublishDo>(s => s.HotelPublishe)
        //         .Match(s => s.HotelCode == qry.HotelCode && s.MaximumNumberOfPeople >= (qry.AdultNum + qry.ChildNum) && s.AdultLimit >= qry.AdultNum && s.ChildLimit >= qry.ChildNum)
        //         .Project(h => new
        //         {
        //             RoomId = h.Id,
        //             h.HotelCode,
        //             h.RoomType,
        //             h.HotelRoomTitle
        //         }).ToListAsync();



        var roomFilter = Builders<HotelRoomDo>.Filter.And(
                        Builders<HotelRoomDo>.Filter.Eq(x => x.HotelCode, qry.HotelCode),
                        Builders<HotelRoomDo>.Filter.Gte(x => x.MaximumNumberOfPeople, (qry.AdultNum + qry.ChildNum)),  //大于等于
                        Builders<HotelRoomDo>.Filter.Gte(x => x.AdultLimit, qry.AdultNum),
                        Builders<HotelRoomDo>.Filter.Gte(x => x.ChildLimit, qry.ChildNum)
                        );
        var roomData = await MongoDb.GetCollection<HotelRoomDo>(nameof(HotelRoomDo)).Find(roomFilter).ToListAsync();

        if (roomData is null || roomData.Count == 0) return default!;

        //收集房间id
        var roomids = roomData!.Select(t => t.Id).ToList();


        //查询库存
        var filter = Builders<DailyInventoryDo>.Filter.And(
        Builders<DailyInventoryDo>.Filter.In(x => x.RoomId, roomids),
        Builders<DailyInventoryDo>.Filter.Gte(x => x.InventoryNum, qry.RoomNum),
        Builders<DailyInventoryDo>.Filter.Gte(x => x.CurrentDate, qry.CheckInDate.Date),
        Builders<DailyInventoryDo>.Filter.Lt(x => x.CurrentDate, qry.CheckOutDate.Date));
        var inventoryData = await MongoDb.GetCollection<DailyInventoryDo>(nameof(DailyInventoryDo)).Find(filter).ToListAsync();

        //按CreatedbyId 分组 计算每组的记录行数
        var inventoryGroup = inventoryData!.GroupBy(x => new { x.CreatedbyId, x.RoomId })
            .Select(g => new { CreatedbyId = g.Key.CreatedbyId, RoomId = g.Key.RoomId, Count = g.Count(), item = g.ToList() })
            .ToList();
        if (!inventoryGroup.Any(t => t.Count == continuousStayDays)) return default!;





        //查询价格计划
        var pricePlanFilter = Builders<PricePlanDo>.Filter.And(
        Builders<PricePlanDo>.Filter.In(x => x.HotelRoomId, roomids),
        Builders<PricePlanDo>.Filter.Gte(x => x.ContinuousStayDays, continuousStayDays),
        Builders<PricePlanDo>.Filter.Lte(x => x.DaysInAdvance, advanceDays));
        var pricePlanData = await MongoDb.GetCollection<PricePlanDo>(nameof(PricePlanDo)).Find(pricePlanFilter).ToListAsync();
        if (pricePlanData is null || pricePlanData.Count == 0) return default!;

        //收集价格计划id
        var pricePlanIds = pricePlanData!.Select(t => t.Id).ToList();

        //查询每日价格
        var dailyPriceFilter = Builders<DailyPriceDo>.Filter.And(
        Builders<DailyPriceDo>.Filter.In(x => x.PricePlanId, pricePlanIds),
        Builders<DailyPriceDo>.Filter.Gte(x => x.CurrentDate, qry.CheckInDate.Date),
        Builders<DailyPriceDo>.Filter.Lt(x => x.CurrentDate, qry.CheckOutDate.Date));
        var dailyPriceData = await MongoDb.GetCollection<DailyPriceDo>(nameof(DailyPriceDo)).Find(dailyPriceFilter).ToListAsync();

        //按CreatedbyId 分组 计算每组的记录行数
        var dailyPriceDataGroup = dailyPriceData!.GroupBy(x => new { x.CreatedbyId, x.RoomId, x.PricePlanId }).Select(g => new
        {
            CreatedbyId = g.Key.CreatedbyId,
            RoomId = g.Key.RoomId,
            PricePlanId = g.Key.PricePlanId,
            Count = g.Count(),
            item = g.ToList()
        });
        //价格记录行数与连住天数一致
        if (!dailyPriceDataGroup.Any(t => t.Count == continuousStayDays)) return default!;


        //组装查价结果
        List<HotelPriceDto> result = new List<HotelPriceDto>();


        //按房型分组输出房型的最低价
        var RoomCodeGroupData = roomData.GroupBy(t => t.RoomType).Select(t => new { t.Key, item = t.ToList() }).ToList();



        foreach (var roomCodeGroup in RoomCodeGroupData)
        {
            List<HotelPriceDto> roomOptions = new List<HotelPriceDto>();
            foreach (var room in roomCodeGroup.item)
            {
                //每日库存
                var dailyInventory = inventoryGroup.FirstOrDefault(t => room.Id == t.RoomId && t.Count == continuousStayDays);
                if (dailyInventory is null || dailyInventory?.Count <= 0) continue;

                //价格计划   
                var pricePlan = pricePlanData.Where(x => x.HotelRoomId == room.Id && x.ContinuousStayDays >= continuousStayDays && x.DaysInAdvance <= advanceDays).ToList();
                if (pricePlan?.Count <= 0) continue;


                //收集当前房型下的价格计划id
                var roomPricePlanIds = pricePlan!.Select(t => t.Id).ToList();


                //每日价格
                var dailyPrice = dailyPriceDataGroup.Where(t => t.RoomId == room.Id && roomPricePlanIds.Contains(t.PricePlanId) && t.Count == continuousStayDays).ToList();
                if (dailyPrice?.Count <= 0) continue;






                foreach (var item in pricePlan!)
                {
                    var models = dailyPrice!.FirstOrDefault(x => x.PricePlanId == item.Id && x.RoomId == room.Id);
                    if (models is null) continue;

                    roomOptions.Add(new HotelPriceDto()
                    {
                        HotelCode = room.HotelCode,
                        RoomCode = room.RoomType,
                        RoomName = room.HotelRoomTitle ?? string.Empty,
                        SearchCode = CompressedEncryptor.Encrypt($@"{string.Join(",", models.item.Select(t => t.Id).ToList())}|{string.Join(",", dailyInventory!.item.Select(t => t.Id).ToList())}",
                                                                SecretKeyConst.key, SecretKeyConst.iv),
                        PricePlanId = item.Id.ToString(),
                        TotalPrice = models.item.Sum(t => t.Price),
                        DayPrice = models.item!.ToDictionary(t => t.CurrentDate.ToString("yyyy-MM-dd"), t => t.Price),
                        IsBreakfast = item.BreakfastType.ToDescription(),
                    });
                }

            }
            //筛选最低价
            var minPrice = roomOptions.OrderBy(o => o.TotalPrice).FirstOrDefault();
            if (minPrice is not null)
            {
                result.Add(minPrice);
            }

        }

        return result;



        //var dailyInventoryPriceDetail = await db.Queryable<PricePlanDo, HotelRoomDo, DailyPriceDo, DailyInventoryDo>((t, t1, t2, t3) =>
        //               t.HotelRoomId == t1.Id
        //               && t2.PricePlanId == t.Id
        //               && t3.RoomId == t.HotelRoomId)
        //        .ClearFilter<ITenantIdFilter>()
        //        .Where((t, t1, t2, t3) => t.ContinuousStayDays <= continuousStayDays && t.DaysInAdvance <= advanceDays

        //          && t1.AdultLimit >= qry.AdultNum && t1.ChildLimit >= qry.ChildNum && t1.MaximumNumberOfPeople >= (qry.AdultNum + qry.ChildNum) && t1.HotelCode == qry.HotelCode
        //          && t2.CurrentDate >= qry.CheckInDate.Date && t2.CurrentDate < qry.CheckOutDate.Date
        //          && t3.CurrentDate >= qry.CheckInDate.Date && t3.CurrentDate < qry.CheckOutDate.Date && t3.InventoryNum >= 1)
        //         .GroupBy((t, t1, t2, t3) => new { PricePlanId = t2.Id, PriceId = t2.Id, t2.CurrentDate, roomId = t1.Id, t2.CreatedbyId })
        //         .Having((t, t1, t2, t3) => SqlFunc.AggregateCount(t2.Id) == continuousStayDays)
        //         .Select((t, t1, t2, t3) => new
        //         {
        //             PricePlanId = t2.Id,
        //             t2.CurrentDate,
        //             Price = SqlFunc.AggregateMax(t2.Price),
        //             roomId = t1.Id,
        //             IsBreakfast = SqlFunc.AggregateMax(t.BreakfastType)
        //         })
        //         .ToListAsync();

        //var roomIds = dailyInventoryPriceDetail.Select(x => x.roomId).Distinct().ToList();


        //var data = await db.Queryable<HotelPublishDo, HotelRoomDo>((t, t1) => t.Id == t1.HotelId)
        // .ClearFilter<ITenantIdFilter>()
        // .Where((t, t1) => roomIds.Contains(t1.Id) && t.HotelCode == qry.HotelCode)
        // .Select((t, t1) => new
        // {
        //     HotelCode = t.HotelCode,
        //     RoomName = t1.HotelRoomTitle ?? string.Empty,
        //     RoomCode = t1.RoomType ?? string.Empty,
        //     roomId = t1.Id
        // }).ToListAsync();

        //var result = data.GroupBy(x => x.RoomCode)
        //    .Select(g =>
        //    {
        //        var roomOptions = g.Select(x =>
        //        {
        //            var models = dailyInventoryPriceDetail
        //                .Where(t => t.roomId == x.roomId)
        //                .OrderBy(q => q.CurrentDate)
        //                .ToList();
        //            return new HotelPriceDto()
        //            {
        //                HotelCode = x.HotelCode,
        //                RoomName = x.RoomName,
        //                RoomCode = x.RoomCode,
        //                SearchCode = EncryptUtils.MD5Encrypt(string.Join(",", models.Select(y => y.PricePlanId).Distinct())),
        //                DayPrice = models.ToDictionary(t => t.CurrentDate.ToString("yyyy-MM-dd"), t => t.Price),
        //                TotalPrice = models.Sum(t => t.Price),
        //                IsBreakfast = models.First().IsBreakfast.ToDescription(),
        //                RoomNameEn = string.Empty,
        //            };
        //        }).ToList();
        //        return roomOptions.OrderBy(o => o.TotalPrice).First();

        //    });

        //return result;
    }
}