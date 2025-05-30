using Microsoft.Extensions.DependencyInjection;
using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Shared.Const;

namespace YueJia.Ebk.Application.HotelApp;


[DisableValidation]
public class HotelApp : ApplicationService, IHotelApp
{

    private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();
    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();
    private ISimpleClient<HotelRoomDo> HotelRoomRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<HotelRoomDo>>();
    private ISimpleClient<PricePlanDo> PricePlanRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<PricePlanDo>>();
    private ISqlSugarClient SqlSugarClient => LazyServiceProvider.GetRequiredKeyedService<ISqlSugarClient>(DbConst.YueJiaSysDb);



    public async Task<long> AddHotelRoomAsync(CreateHotelRoomCmd cmd)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateHotelRoomCmd>>().ValidateAndThrowAsync(cmd);

        var entity = HotelRoomDo.Create(cmd.HotelId.ToLong(), cmd.HotelCode, cmd.RoomType, cmd.BedType, cmd.MaximumNumberOfPeople,
            cmd.AdultLimit, cmd.ChildLimit, cmd.StartDate, cmd.EndDate, cmd.StockInitValJosn)
            ?? throw new InvalidOperationException("床间酒店房间信息失败！");


        List<RoomStockDo> roomStockDos = new();
        for (DateTime date = cmd.StartDate; date <= cmd.EndDate; date = date.AddDays(1))
        {
            // 获取当前日期的星期几
            DayOfWeek dayOfWeek = date.DayOfWeek;
            // 如果键不存在，返回 int 的默认值（0）
            int stockNum = cmd.Stock.GetValueOrDefault(dayOfWeek);
            roomStockDos.Add(RoomStockDo.Create(entity.Id, date, stockNum));
        }
        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            await db.Insertable(roomStockDos).ExecuteCommandAsync();
            await db.Insertable(entity).ExecuteCommandAsync();
            return entity.Id;
        });


    }



    public async Task<long> CreatePricePlanAsync(CreateOrUpdatePricePlanCmd cmd)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpdatePricePlanCmd>>().ValidateAndThrowAsync(cmd);

        var entity = PricePlanDo.Create(cmd.HotelRoomId.ToLong(), cmd.BreakfastType, cmd.DaysInAdvance, cmd.ContinuousStayDays, cmd.IsReservedRoom, cmd.IsEnable);

        return await PricePlanRepo.InsertReturnSnowflakeIdAsync(entity);
    }

    public async Task<bool> DeleteHotelRoomAsync(long id)
    {
        var entity = HotelRoomRepo.GetById(id) ?? throw new InvalidOperationException("酒店房间信息不存在！");
        if (entity.IsDelete) return true;
        entity.IsDelete = true;
        var pricePlanEntity = PricePlanRepo.AsQueryable().Where(x => x.HotelRoomId == id).ToList();
        pricePlanEntity.ForEach(x => x.IsDelete = true);


        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            await db.Updateable(entity)
                    .PublicSetColumns(it => it.Version, it => it.Version + 1)
                    .UpdateColumns(it => new { it.IsDelete, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
                    .ExecuteCommandAsync();
            await db.Updateable(pricePlanEntity)
                    .PublicSetColumns(it => it.Version, it => it.Version + 1)
                    .UpdateColumns(it => new { it.IsDelete, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
                    .ExecuteCommandAsync();




            return true;
        });
    }


    public async Task<bool> DeletePricePlanAsync(long id)
    {
        var entity = await PricePlanRepo.GetByIdAsync(id) ?? throw new InvalidOperationException("价格计划不存在！");

        if (entity.IsDelete) return true;
        entity.IsDelete = true;
        return await PricePlanRepo.AsUpdateable(entity)
            .UpdateColumns(it => new { it.IsDelete, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
            .ExecuteCommandWithOptLockAsync(true) > 0;
    }

    public async Task<HotelRoomDetailsDto> GetHotelRoomByIdAsync(long id)
    {
        var entity = await HotelRoomRepo.GetByIdAsync(id) ?? throw new InvalidOperationException("床间酒店房间信息不存在！");
        var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
            .Where(q => q.pfcode == "D" && q.hotelcode == entity.HotelCode)
            .Select(t => new { t.roomcode, t.roomname })
            .ToListAsync();
        return new HotelRoomDetailsDto()
        {
            Id = entity.Id,
            HotelId = entity.HotelId,
            RoomType = entity.RoomType,
            RoomTypeName = currentHotelRoomTypeDate.FirstOrDefault(x => x.roomcode == int.Parse(entity.RoomType))?.roomname ?? string.Empty,
            BedType = entity.BedType,
            MaximumNumberOfPeople = entity.MaximumNumberOfPeople,
            AdultLimit = entity.AdultLimit,
            ChildLimit = entity.ChildLimit
        };
    }

    public async Task<List<HotelRoomListDto>> GetHotelRoomListByIdAsync(long id)
    {
        var data = await HotelRoomRepo.AsQueryable().Where(x => x.HotelId == id)
             .Select(x => new HotelRoomListDto()
             {
                 Id = x.Id,
                 RoomType = x.RoomType,
                 BedType = x.BedType,
                 MaximumNumberOfPeople = x.MaximumNumberOfPeople,
                 AdultLimit = x.AdultLimit,
                 ChildLimit = x.ChildLimit,
                 HotelCode = x.HotelCode,
                 IsEnabled = x.IsEnabled
             })
            .ToListAsync();

        var hotelRoomIds = data.Select(o => o.Id).ToList();
        var items = PricePlanRepo.AsQueryable().Where(i => hotelRoomIds.Contains(i.HotelRoomId))
            .Select(x => new PricePlanListDto()
            {
                Id = x.Id.ToString(),
                BreakfastType = x.BreakfastType,
                DaysInAdvance = x.DaysInAdvance,
                ContinuousStayDays = x.ContinuousStayDays,
                IsReservedRoom = x.IsReservedRoom,
                IsEnable = x.IsEnable,
                HotelRoomId = x.HotelRoomId.ToString()
            })
            .ToList();
        var itemGroups = items.GroupBy(i => i.HotelRoomId).ToDictionary(g => g.Key, g => g.ToList());




        string hotelCode = data?.FirstOrDefault()?.HotelCode ?? string.Empty;

        var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
            .Where(q => q.pfcode == "D" && q.hotelcode == hotelCode)
            .Select(t => new { t.roomcode, t.roomname })
            .ToListAsync();

        var result = data?.Select(item =>
         {
             item.RoomTypeName = currentHotelRoomTypeDate.SingleOrDefault(x => x.roomcode == int.Parse(item.RoomType))?.roomname ?? string.Empty;

             item.PricePlans = itemGroups.TryGetValue(item.Id.ToString(), out var itemList) ? itemList : new();


             return item;
         }).ToList();

        return data ?? new();
    }

    public async Task<PricePlanDetailDto> GetPricePlanDetailsByIdAsync(long id)
    {

        var data = await PricePlanRepo.AsQueryable()
            .LeftJoin<HotelRoomDo>((t, t1) => t.HotelRoomId == t1.Id)
            .LeftJoin<HotelPublishDo>((t, t1, t2) => t1.HotelId == t2.Id)
            .Where((t, t1, t2) => t.Id == id)
            .Select((t, t1, t2) => new PricePlanDetailDto()
            {
                Id = t.Id.ToString(),
                BedType = t1.BedType,
                BreakfastType = t.BreakfastType,
                DaysInAdvance = t.DaysInAdvance,
                ContinuousStayDays = t.ContinuousStayDays,
                IsReservedRoom = t.IsReservedRoom,
                IsEnable = t.IsEnable,
                HotelCode = t2.HotelCode,
                HotelName = t2.HotelName,
                RoomType = t1.RoomType,
                HotelId = t1.HotelId.ToString(),
                HotelNameEn = t2.HotelNameEn
            }).SingleAsync();

        if (data is null) throw new InvalidOperationException("价格计划不存在！");
        var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
                                .Where(q => q.pfcode == "D" && q.hotelcode == data.HotelCode)
                                .Select(t => new { t.roomcode, t.roomname })
                                .ToListAsync();
        data.RoomTypeName = currentHotelRoomTypeDate.SingleOrDefault(x => x.roomcode == int.Parse(data.RoomType))?.roomname ?? string.Empty;

        return data;
    }

    public async Task<bool> UpdatePricePlanAsync(CreateOrUpdatePricePlanCmd cmd, long id)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpdatePricePlanCmd>>().ValidateAndThrowAsync(cmd);
        var entity = await PricePlanRepo.GetByIdAsync(id) ?? throw new InvalidOperationException("价格计划不存在！");
        entity.SetBreakfastType(cmd.BreakfastType)
              .SetContinuousStayDays(cmd.ContinuousStayDays)
              .SetDaysInAdvance(cmd.DaysInAdvance)
              .SetIsEnable(cmd.IsEnable)
              .SetIsReservedRoom(cmd.IsReservedRoom);
        return await PricePlanRepo.AsUpdateable(entity).ExecuteCommandWithOptLockAsync(true) > 0;
    }
}
