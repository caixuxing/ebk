using Microsoft.Extensions.DependencyInjection;
using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Shared.Const;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YueJia.Ebk.Application.HotelApp;


[DisableValidation]
public class HotelApp : ApplicationService, IHotelApp
{

    private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();
    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();
    private ISimpleClient<HotelRoomDo> HotelRoomRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<HotelRoomDo>>();
    private ISimpleClient<PricePlanDo> PricePlanRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<PricePlanDo>>();
    private ISqlSugarClient SqlSugarClient => LazyServiceProvider.GetRequiredKeyedService<ISqlSugarClient>(DbConst.YueJiaSysDb);

    private ISimpleClient<HotelPublishDo> HotelPublishRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<HotelPublishDo>>();



    private ISimpleClient<RoomInventoryDo> RoomStockRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<RoomInventoryDo>>();


    public async Task<bool> AddHotelRoomAsync(CreateHotelRoomCmd cmd)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateHotelRoomCmd>>().ValidateAndThrowAsync(cmd);

   
        var entity = HotelRoomDo.Create(cmd.HotelId.ToLong(), 
                                        cmd.HotelCode, 
                                        cmd.RoomType, 
                                        cmd.BedType, 
                                        cmd.MaximumNumberOfPeople, 
                                        cmd.AdultLimit, 
                                        cmd.ChildLimit, 
                                        cmd.StartDate, 
                                        cmd.EndDate,
                                        cmd.HotelRoomTitle);
        
        if (db.Queryable<HotelRoomDo>().Any(vv=> vv.HotelId == SqlFunc.ToInt64(cmd.HotelId) && vv.RoomType == cmd.RoomType  )) {
            throw new InvalidOperationException("房间已存在");
        }

        List<DailyInventoryDo> dailyInventoryDoList = new List<DailyInventoryDo>();
        for (DateTime date = cmd.StartDate; date <= cmd.EndDate; date = date.AddDays(1))
        {
            int stockNum = 0;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday: stockNum = cmd.Monday ; break;
                case DayOfWeek.Tuesday: stockNum = cmd.Tuesday; break;
                case DayOfWeek.Wednesday: stockNum = cmd.Wednesday; break;
                case DayOfWeek.Thursday: stockNum = cmd.Thursday; break;
                case DayOfWeek.Friday: stockNum = cmd.Friday; break;
                case DayOfWeek.Saturday: stockNum = cmd.Saturday; break;
                case DayOfWeek.Sunday: stockNum = cmd.Sunday; break;
            }
            dailyInventoryDoList.Add(new DailyInventoryDo() { 
                 RoomId = entity.Id,
                 InventoryNum = stockNum,
                 CurrentDate = date,
            });
        }
          await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            await db.Insertable<DailyInventoryDo>(dailyInventoryDoList).ExecuteCommandAsync();
            await db.Insertable(entity).ExecuteCommandAsync();
            return entity.Id;
        });
        return true;
    }



    public async Task<bool> CreatePricePlanAsync(CreateOrUpdatePricePlanCmd cmd)
    {

        var hotelRoomObj = db.Queryable<HotelRoomDo>().Where(vv => vv.Id == SqlFunc.ToInt64(cmd.HotelRoomId)).ToList().First();
        var entity = PricePlanDo.Create(cmd.HotelRoomId.ToLong(), string.Empty, cmd.BreakfastType, cmd.DaysInAdvance, cmd.ContinuousStayDays, cmd.IsReservedRoom, cmd.IsEnable);

        entity.PricePlanTitle = $@"{hotelRoomObj.HotelRoomTitle}<{(cmd.BreakfastType== BreakfastTypeEnum.Breakfast?"含早":"无早")}><提前{cmd.DaysInAdvance}天><连住{cmd.ContinuousStayDays}天><{(cmd.IsReservedRoom == YesOrNoType.Yes? "保留房" : "非保留房")}>";

        List<DailyPriceDo> dailyPriceDoList = new List<DailyPriceDo>();
        for (DateTime date = hotelRoomObj.StartDate; date <= hotelRoomObj.EndDate; date = date.AddDays(1))
        {
            decimal price = Convert.ToDecimal(0);
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday: price = cmd.Monday; break;
                case DayOfWeek.Tuesday: price = cmd.Tuesday; break;
                case DayOfWeek.Wednesday: price = cmd.Wednesday; break;
                case DayOfWeek.Thursday: price = cmd.Thursday; break;
                case DayOfWeek.Friday: price = cmd.Friday; break;
                case DayOfWeek.Saturday: price = cmd.Saturday; break;
                case DayOfWeek.Sunday: price = cmd.Sunday; break;
            }
            dailyPriceDoList.Add(new  DailyPriceDo()
            {
                RoomId = hotelRoomObj.Id,
                PricePlanId = entity.Id,
                Price = price,
                CurrentDate = date, 
            });
        }


        await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            await db.Insertable<DailyPriceDo>(dailyPriceDoList).ExecuteCommandAsync();
            await db.Insertable<PricePlanDo>(entity).ExecuteCommandAsync();
            return entity.Id;
        });

        return true;
    }

    
    public async Task<bool> DeleteHotelRoomAsync(long id)
    {
        //房间
        var entity = HotelRoomRepo.GetById(id);
        entity.IsDelete = true;

        //库存
        var roomInventoryList = db.Queryable<DailyInventoryDo>().Where(vv => vv.RoomId == entity.Id).ToList();
        roomInventoryList.ForEach(x => x.IsDelete = true);

        //价格计划
        var pricePlanEntity = PricePlanRepo.AsQueryable().Where(x => x.HotelRoomId == id).ToList();
        pricePlanEntity.ForEach(x => x.IsDelete = true);

   
        //价格计划价格
        var dailyPriceDoList = db.Queryable<DailyPriceDo>().Where(vv => vv.RoomId == entity.Id).ToList();
        dailyPriceDoList.ForEach(x => x.IsDelete = true);


        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            await db.Updateable(entity)
                    .PublicSetColumns(it => it.Version, it => it.Version + 1)
                    .UpdateColumns(it => new { it.IsDelete, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
                    .ExecuteCommandAsync();

            await db.Updateable(roomInventoryList)
                      .PublicSetColumns(it => it.Version, it => it.Version + 1)
                      .UpdateColumns(it => new { it.IsDelete, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
                      .ExecuteCommandAsync();

            await db.Updateable(pricePlanEntity)
                    .PublicSetColumns(it => it.Version, it => it.Version + 1)
                    .UpdateColumns(it => new { it.IsDelete, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
                    .ExecuteCommandAsync();

            await db.Updateable(dailyPriceDoList)
                    .PublicSetColumns(it => it.Version, it => it.Version + 1)
                    .UpdateColumns(it => new { it.IsDelete, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
                    .ExecuteCommandAsync();

            return true;
        });
    }

    public async Task<bool> UpdateRoomStateAsync(long id)
    {
        //房间
        var entity = HotelRoomRepo.GetById(id);
        if (entity == null)
        {
            throw new InvalidOperationException("数据不存在！");
        }
        entity.SetIsEnabled(entity.IsEnabled == YesOrNoType.Yes ? YesOrNoType.No : YesOrNoType.Yes);


        return await db.Updateable<HotelRoomDo>(entity).ExecuteCommandWithOptLockAsync(true) > 0;
    }

    

    public async Task<bool> DeletePricePlanAsync(long id)
    {
        var entity = await PricePlanRepo.GetByIdAsync(id);
        if (entity==null) {
            throw new InvalidOperationException("价格计划不存在！");
        }
        entity.IsDelete = true;

        //价格计划价格
        var dailyPriceDoList = db.Queryable<DailyPriceDo>().Where(vv => vv.RoomId == entity.HotelRoomId).ToList();
        dailyPriceDoList.ForEach(x => x.IsDelete = true);


        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            await db.Updateable(entity)
                    .PublicSetColumns(it => it.Version, it => it.Version + 1)
                    .UpdateColumns(it => new { it.IsDelete, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
                    .ExecuteCommandAsync();

            await db.Updateable(dailyPriceDoList)
                    .PublicSetColumns(it => it.Version, it => it.Version + 1)
                    .UpdateColumns(it => new { it.IsDelete, it.LastModifiedbyId, it.LastModifiedbyName, it.LastModifiedTime, it.Version })
                    .ExecuteCommandAsync();

            return true;
        });

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
                 IsEnabled = x.IsEnabled,
                 StartDate = x.StartDate,
                 EndDate = x.EndDate
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
                HotelRoomId = x.HotelRoomId.ToString(),
                PricePlanTitle = x.PricePlanTitle
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

    /// <summary>
    /// 切换价格计划状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> UpdatePricePlanStateAsync( long id)
    {
        var entity = await PricePlanRepo.GetByIdAsync(id);
        if(entity==null)
        {
            throw new InvalidOperationException("价格计划不存在！");
        } 
        entity.SetIsEnable(entity.IsEnable == YesOrNoType.Yes? YesOrNoType.No: YesOrNoType.Yes);
        return await PricePlanRepo.AsUpdateable(entity).ExecuteCommandWithOptLockAsync(true) > 0;
    }


    /// <summary>
    /// 读取库存和价格详情
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<InventoryAndPriceDetailsDto> GetInventoryAndPriceDetailsByFilterAsync(InventoryAndPriceDetailsQry qry)
    {

        var RoomEntitys = await HotelRoomRepo.GetListAsync(x => x.HotelId == qry.HotelId);

        var roomIds = RoomEntitys.Select(x => x.Id).ToList();

        var pricePlanEntities = await PricePlanRepo.GetFirstAsync(x => x.HotelRoomId == qry.RoomId);


        var hotelCode = RoomEntitys.FirstOrDefault()?.HotelCode ?? throw new InvalidOperationException("酒店编码不存在！");

        var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
                              .Where(q => q.pfcode == "D" && q.hotelcode == hotelCode)
                              .Select(t => new { t.roomcode, t.roomname })
                              .ToListAsync();
        var RoomStockEntitys = await RoomStockRepo.GetListAsync(x => x.CurrentDate >= qry.StartDate && x.CurrentDate <= qry.StartDate.AddDays(qry.Days) && roomIds.Contains(qry.RoomId));

        return new()
        {


            RoomTypeDropdownList = RoomEntitys.Select(x => new SelectDataDto<string>() { Value = x.RoomType, Label = currentHotelRoomTypeDate.FirstOrDefault(y => y.roomcode == int.Parse(x.RoomType))?.roomname ?? string.Empty }).ToList(),
            Room = RoomEntitys.Where(x => x.Id == 5).Select(x => new Room()
            {
                Id = x.Id.ToString(),
                RoomName = currentHotelRoomTypeDate.FirstOrDefault(y => y.roomcode == int.Parse(x.RoomType))?.roomname ?? string.Empty,
                Status = x.IsEnabled,
                Inventories = RoomStockEntitys.Select(t => new Inventory()
                {
                    Id = t.Id.ToString(),
                    MonthDay = t.CurrentDate.ToString("MM-dd"),
                    InventoryNum = t.StockNum,
                    Status = t.IsEnabled,
                    DayOfWeek = t.CurrentDate.DayOfWeek.ToString(),

                }).ToList()

            }).FirstOrDefault() ?? new(),
            PricePlan = new PricePlan
            {
                Id = pricePlanEntities?.Id.ToString() ?? string.Empty,
                Name = "价格计划名称Remark",
                Status = pricePlanEntities?.IsEnable ?? YesOrNoType.No,
                Prices = RoomStockEntitys.Select(x => new PriceItem()
                {
                    Id = x.Id.ToString(),
                    Price = x.Price,
                    Status = x.IsEnabled,

                }).ToList()
            }
        };
    }

    public async Task<List<TreeSelectDataDto<string>>> GetHoteTreeSelectDataByHotelIdAsync(long hotelId)
    {
        var result = await HotelRoomRepo.GetListAsync(x => x.HotelId == hotelId);
        var hotelCode = result.FirstOrDefault()?.HotelCode ?? string.Empty;
        var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
                            .Where(q => q.pfcode == "D" && q.hotelcode == hotelCode)
                            .Select(t => new { t.roomcode, t.roomname })
                            .ToListAsync();

        return new List<TreeSelectDataDto<string>>() { new TreeSelectDataDto<string>()
           {
               Label = "全选",
               Value = "all",
               Children = result.Select(x => new SelectDataDto<string>()
               {
                    Label = $"{x.Id} {currentHotelRoomTypeDate.FirstOrDefault(y => y.roomcode == int.Parse(x.RoomType))?.roomname ?? string.Empty},{x.BedType.ToDescription()}",
                    Value = x.Id.ToString()
               }).ToList()
           }
        };


    }

    public async Task<LoadingInventoryAndPricesDto> LoadingInventoryAndPricesViewAsync(long hotelId)
    {

        var hotelEntity = await HotelPublishRepo.GetByIdAsync(hotelId) ?? throw new InvalidOperationException("酒店发布信息不存在！");

        var roomEntity = await HotelRoomRepo.GetListAsync(x => x.HotelId == hotelId);

        var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
                            .Where(q => q.pfcode == "D" && q.hotelcode == hotelEntity.HotelCode)
                            .Select(t => new { t.roomcode, t.roomname })
                            .ToListAsync();


        var roomIds = roomEntity.Select(x => x.Id).ToList();

        var pricePlanEntity = await PricePlanRepo.AsQueryable()
            .Where(x => roomIds.Contains(x.HotelRoomId))
            .Select(t => new PricePlanItemDto()
            {

                PricePlanId = t.Id.ToString(),
                PricePlanName = t.PricePlanTitle ?? $"{t.Id.ToString()} {t.IsEnable}",
                RoomId = t.HotelRoomId.ToString(),
                Status = t.IsEnable
            })
            .ToListAsync();

        return new()
        {
            HotelId = hotelEntity.Id.ToString(),
            HotelName = hotelEntity.HotelName,
            HotelNameEn = hotelEntity.HotelNameEn,
            HotelCode = hotelEntity.HotelCode,

            RoomTypes = new List<TreeSelectDataDto<string>>() { new TreeSelectDataDto<string>()
               {
                   Label = "全选",
                   Value = "all",
                   Children = roomEntity.Select(x => new SelectDataDto<string>()
                   {
                        Label = $"{x.Id} {currentHotelRoomTypeDate.FirstOrDefault(y => y.roomcode == int.Parse(x.RoomType))?.roomname ?? string.Empty},{x.BedType.ToDescription()}",
                        Value = x.Id.ToString()
                   }).ToList()
               } },
            HotelRoomPricePlanAll = pricePlanEntity
        };
    }
}
