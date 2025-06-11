using YueJia.Ebk.Application.Contracts.EbkApp;
using YueJia.Ebk.Application.Contracts.EbkApp.Query;
using YueJia.Ebk.Domain.AggRoot;
using YueJia.Ebk.Domain.Hotel;

namespace YueJia.Ebk.Application.EbkApp;



[DisableValidation]
public class EbkApp : ApplicationService, IEbkApp
{


    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();


    public async Task<IEnumerable<HotelPriceDto>> PriceSearch(PriceSearchQry qry)
    {
        //验证
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<PriceSearchQry>>().ValidateAndThrowAsync(qry);

        //连住天数
        int continuousStayDays = (qry.CheckOutDate.Date - qry.CheckInDate.Date).Days;
        //提前天数
        int advanceDays = (qry.CheckOutDate.Date - DateTime.Now.Date).Days;

        var dailyInventoryPriceDetail = await db.Queryable<PricePlanDo, HotelRoomDo, DailyPriceDo, DailyInventoryDo>((t, t1, t2, t3) =>
                   t.HotelRoomId == t1.Id
                   && t2.PricePlanId == t.Id
                   && t3.RoomId == t.HotelRoomId)
            .ClearFilter<ITenantIdFilter>()
            .Where((t, t1, t2, t3) => t.ContinuousStayDays <= continuousStayDays && t.DaysInAdvance <= advanceDays

              && t1.AdultLimit >= qry.AdultNum && t1.ChildLimit >= qry.ChildNum && t1.MaximumNumberOfPeople >= (qry.AdultNum + qry.ChildNum) && t1.HotelCode == qry.HotelCode
              && t2.CurrentDate >= qry.CheckInDate.Date && t2.CurrentDate < qry.CheckOutDate.Date
              && t3.CurrentDate >= qry.CheckInDate.Date && t3.CurrentDate < qry.CheckOutDate.Date && t3.InventoryNum >= 1)
             .GroupBy((t, t1, t2, t3) => new { PricePlanId = t2.Id, PriceId = t2.Id, t2.CurrentDate, roomId = t1.Id, t2.CreatedbyId })
             .Having((t, t1, t2, t3) => SqlFunc.AggregateCount(t2.Id) == continuousStayDays)
             .Select((t, t1, t2, t3) => new
             {
                 PricePlanId = t2.Id,
                 t2.CurrentDate,
                 Price = SqlFunc.AggregateMax(t2.Price),
                 roomId = t1.Id,
                 IsBreakfast = SqlFunc.AggregateMax(t.BreakfastType)
             })
             .ToListAsync();

        var roomIds = dailyInventoryPriceDetail.Select(x => x.roomId).Distinct().ToList();


        var data = await db.Queryable<HotelPublishDo, HotelRoomDo>((t, t1) => t.Id == t1.HotelId)
         .ClearFilter<ITenantIdFilter>()
         .Where((t, t1) => roomIds.Contains(t1.Id) && t.HotelCode == qry.HotelCode)
         .Select((t, t1) => new
         {
             HotelCode = t.HotelCode,
             RoomName = t1.HotelRoomTitle ?? string.Empty,
             RoomCode = t1.RoomType ?? string.Empty,
             roomId = t1.Id
         }).ToListAsync();

        var result = data.GroupBy(x => x.RoomCode)
            .Select(g =>
            {
                var roomOptions = g.Select(x =>
                {
                    var models = dailyInventoryPriceDetail
                        .Where(t => t.roomId == x.roomId)
                        .OrderBy(q => q.CurrentDate)
                        .ToList();
                    return new HotelPriceDto()
                    {
                        HotelCode = x.HotelCode,
                        RoomName = x.RoomName,
                        RoomCode = x.RoomCode,
                        SearchCode = EncryptUtils.MD5Encrypt(string.Join(",", models.Select(y => y.PricePlanId).Distinct())),
                        DayPrice = models.ToDictionary(t => t.CurrentDate.ToString("yyyy-MM-dd"), t => t.Price),
                        TotalPrice = models.Sum(t => t.Price),
                        IsBreakfast = models.First().IsBreakfast.ToDescription(),
                        RoomNameEn = string.Empty,
                    };
                }).ToList();
                return roomOptions.OrderBy(o => o.TotalPrice).First();

            });

        return result;
    }
}
