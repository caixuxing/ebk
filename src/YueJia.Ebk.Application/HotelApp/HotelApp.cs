using Microsoft.Extensions.DependencyInjection;
using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
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

    private ISimpleClient<HotelPublishDo> HotelPublishRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<HotelPublishDo>>();

    private ISimpleClient<DailyInventoryDo> DailyInventoryRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<DailyInventoryDo>>();
    private ISimpleClient<DailyPriceDo> DailyPriceRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<DailyPriceDo>>();


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

        if (db.Queryable<HotelRoomDo>().Any(vv => vv.HotelId == SqlFunc.ToInt64(cmd.HotelId) && vv.RoomType == cmd.RoomType))
        {
            throw new InvalidOperationException("房间已存在");
        }

        List<DailyInventoryDo> dailyInventoryDoList = new List<DailyInventoryDo>();
        for (DateTime date = cmd.StartDate; date <= cmd.EndDate; date = date.AddDays(1))
        {
            int stockNum = 0;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday: stockNum = cmd.Monday; break;
                case DayOfWeek.Tuesday: stockNum = cmd.Tuesday; break;
                case DayOfWeek.Wednesday: stockNum = cmd.Wednesday; break;
                case DayOfWeek.Thursday: stockNum = cmd.Thursday; break;
                case DayOfWeek.Friday: stockNum = cmd.Friday; break;
                case DayOfWeek.Saturday: stockNum = cmd.Saturday; break;
                case DayOfWeek.Sunday: stockNum = cmd.Sunday; break;
            }

            dailyInventoryDoList.Add(DailyInventoryDo.Create(entity.Id, date, stockNum, YesOrNoType.Yes));
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

        entity.PricePlanTitle = $@"{hotelRoomObj.HotelRoomTitle}<{(cmd.BreakfastType == BreakfastTypeEnum.Breakfast ? "含早" : "无早")}><提前{cmd.DaysInAdvance}天><连住{cmd.ContinuousStayDays}天><{(cmd.IsReservedRoom == YesOrNoType.Yes ? "保留房" : "非保留房")}>";

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
            dailyPriceDoList.Add(new DailyPriceDo()
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
        if (entity == null)
        {
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
                 Id = x.Id.ToString(),
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
        var items = PricePlanRepo.AsQueryable().Where(i => hotelRoomIds.Contains(SqlFunc.ToString(i.HotelRoomId)))
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
    public async Task<bool> UpdatePricePlanStateAsync(long id)
    {
        var entity = await PricePlanRepo.GetByIdAsync(id);
        if (entity == null)
        {
            throw new InvalidOperationException("价格计划不存在！");
        }
        entity.SetIsEnable(entity.IsEnable == YesOrNoType.Yes ? YesOrNoType.No : YesOrNoType.Yes);
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

        var RoomEntitys = await HotelRoomRepo.GetListAsync(x => x.HotelId == long.Parse(qry.HotelId));

        var roomIds = RoomEntitys.Select(x => x.Id).ToList();

        var pricePlanEntities = await PricePlanRepo.GetFirstAsync(x => x.HotelRoomId == long.Parse(qry.RoomId));


        var hotelCode = RoomEntitys.FirstOrDefault()?.HotelCode ?? throw new InvalidOperationException("酒店编码不存在！");

        var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
                              .Where(q => q.pfcode == "D" && q.hotelcode == hotelCode)
                              .Select(t => new { t.roomcode, t.roomname })
                              .ToListAsync();
        var RoomStockEntitys = await RoomStockRepo.GetListAsync(x => x.CurrentDate >= qry.StartDate && x.CurrentDate <= qry.StartDate.AddDays(qry.Days) && roomIds.Contains(long.Parse(qry.RoomId)));

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

            RoomTypes = roomEntity.Select(x => new SelectDataDto<string>()
            {
                Label = $"{x.Id} {currentHotelRoomTypeDate.FirstOrDefault(y => y.roomcode == int.Parse(x.RoomType))?.roomname ?? string.Empty},{x.BedType.ToDescription()}",
                Value = x.Id.ToString()
            }).ToList(),
            HotelRoomPricePlanAll = pricePlanEntity
        };
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="userHotelId"></param>
    /// <returns></returns>
    public async Task<List<HotelRoomListDto>> GetEbkOtaRoomList(long userHotelId)
    {

        return await HotelRoomRepo.AsQueryable().Where(x => x.HotelId == userHotelId)
                                                     .Select(x => new HotelRoomListDto()
                                                     {
                                                         Id = x.Id.ToString(),
                                                         RoomType = x.RoomType,
                                                         BedType = x.BedType,
                                                         AdultLimit = x.AdultLimit,
                                                         ChildLimit = x.ChildLimit,
                                                         IsEnabled = x.IsEnabled,
                                                         HotelRoomTitle = x.HotelRoomTitle
                                                     })
                                                    .ToListAsync();
    }



    public async Task<List<PricePlanListDto>> GetEbkPricePlanList(long userHotelId)
    {

        var dataList = await db.Queryable<HotelRoomDo>()
                                  .InnerJoin<PricePlanDo>((x1, x2) => x1.Id == x2.HotelRoomId)
                                  .Where((x1, x2) => x1.HotelId == userHotelId)
                                  .Select((x1, x2) => new PricePlanListDto()
                                  {
                                      Id = SqlFunc.ToString(x2.Id),
                                      PricePlanTitle = x2.PricePlanTitle
                                  }).ToListAsync();
        return dataList;

    }

    public async Task<List<DailyInventoryModel>> GetInventoryList(long userRoomId, int dateYear, int dateMonth)
    {
        var sDate = new DateTime(dateYear, dateMonth, 1);
        var eDate = sDate.AddMonths(1);

        var userRoomObj = db.Queryable<HotelRoomDo>().Where(vv => vv.Id == userRoomId).ToList().First();


        var dataList = await db.Queryable<DailyInventoryDo>().Where(vv => vv.RoomId == userRoomId
                                                          && vv.CurrentDate >= sDate
                                                          && vv.CurrentDate < eDate)
                                               .Select(vv => new DailyInventoryModel
                                               {
                                                   CurrentDate = vv.CurrentDate,
                                                   InventoryNum = vv.InventoryNum,
                                                   StatusBool = vv.IsEnable == YesOrNoType.Yes
                                               }).ToListAsync();

        List<DailyInventoryModel> Result = new List<DailyInventoryModel>();
        for (DateTime date = sDate; date < eDate; date = date.AddDays(1))
        {
            if (date < userRoomObj.StartDate || date > userRoomObj.EndDate)
            {
                continue;
            }
            Result.Add(new DailyInventoryModel() { CurrentDate = date, InventoryNum = 0, StatusBool = false });
            if (dataList.Any(vv => vv.CurrentDate == date))
            {
                Result.Last().InventoryNum = dataList.First(vv => vv.CurrentDate == date).InventoryNum;
                Result.Last().StatusBool = dataList.First(vv => vv.CurrentDate == date).StatusBool;
            }
        }
        return Result;
    }


    public async Task<List<DailyPriceModel>> GetPriceList(long userPlanId, int dateYear, int dateMonth)
    {
        var sDate = new DateTime(dateYear, dateMonth, 1);
        var eDate = sDate.AddMonths(1);

        var userRoomObj = db.Queryable<HotelRoomDo>()
                                  .InnerJoin<PricePlanDo>((x1, x2) => x1.Id == x2.HotelRoomId)
                                  .Where((x1, x2) => x2.Id == userPlanId)
                                  .Select((x1, x2) => new HotelRoomListDto()
                                  {
                                      Id = SqlFunc.ToString(x1.Id),
                                      StartDate = x1.StartDate,
                                      EndDate = x1.EndDate,
                                  }).ToListAsync().Result.First();



        var dataList = await db.Queryable<DailyPriceDo>().Where(vv => vv.RoomId == SqlFunc.ToInt64(userRoomObj.Id)
                                                          && vv.CurrentDate >= sDate
                                                          && vv.CurrentDate < eDate)
                                               .Select(vv => new DailyPriceModel
                                               {
                                                   Price = vv.Price,
                                                   CurrentDate = vv.CurrentDate,
                                                   StatusBool = vv.IsEnable == YesOrNoType.Yes,
                                               }).ToListAsync();

        List<DailyPriceModel> Result = new List<DailyPriceModel>();
        for (DateTime date = sDate; date < eDate; date = date.AddDays(1))
        {
            if (date < userRoomObj.StartDate || date > userRoomObj.EndDate)
            {
                continue;
            }
            Result.Add(new DailyPriceModel() { CurrentDate = date, Price = 0, StatusBool = false });
            if (dataList.Any(vv => vv.CurrentDate == date))
            {
                Result.Last().Price = dataList.First(vv => vv.CurrentDate == date).Price;
                Result.Last().StatusBool = dataList.First(vv => vv.CurrentDate == date).StatusBool;
            }
        }
        return Result;
    }



    public async Task<InventoryAndPriceDto> InventoryAndPriceViewAsync(InventoryAndPriceDetailsQry qry)
    {
        var model = new InventoryAndPriceDto();
        var room = await HotelRoomRepo.GetByIdAsync(qry.RoomId);


        var yyx = await PricePlanRepo.GetListAsync(x => x.HotelRoomId == long.Parse(qry.RoomId));

        var pricePlan = (await PricePlanRepo.AsQueryable()
                .LeftJoin<DailyPriceDo>((o, d) => o.Id == d.PricePlanId)
                .Where((o, d) => o.HotelRoomId == long.Parse(qry.RoomId) && d.CurrentDate >= qry.StartDate && d.CurrentDate <= qry.StartDate.AddDays(qry.Days - 1).Date)
                .Select((o, d) => new
                {
                    pricePlan = o,
                    dailyPrice = d
                })
                .ToListAsync())
                .GroupBy(x => x.pricePlan)
                .Select(g => new PricePlanItemDto()
                {
                    DailyPrices = g.Select(x => new DailyPriceDto()
                    {
                        PriceId = x.dailyPrice.Id.ToString(),
                        Price = x.dailyPrice.Price,
                        MonthDay = x.dailyPrice.CurrentDate.ToString("MM-dd"),
                        PricePlanId = x.dailyPrice.PricePlanId.ToString(),
                        RoomId = x.dailyPrice.RoomId.ToString(),
                        Status = (x.dailyPrice.IsEnable == YesOrNoType.Yes ? true : false),
                    }).ToList(),
                    PricePlanId = g.Key.Id.ToString(),
                    PricePlanName = g.Key.PricePlanTitle ?? $"{g.Key.Id.ToString()} {g.Key.IsEnable}",
                    RoomId = g.Key.HotelRoomId.ToString(),
                    Status = g.Key.IsEnable,
                }).ToList();








        model.ShowDays = qry.Days;
        model.StartDate = qry.StartDate;
        model.RoomTypeInfo = new RoomTypeInfoDto()
        {

            RoomId = room?.Id.ToString() ?? string.Empty,
            Status = room?.IsEnabled ?? YesOrNoType.No,
            HotelRoomTitle = room?.HotelRoomTitle,
            DailyInventory = await DailyInventoryRepo.AsQueryable()
            .Where(x => x.RoomId == long.Parse(qry.RoomId) && x.CurrentDate >= qry.StartDate && x.CurrentDate <= qry.StartDate.AddDays(qry.Days - 1))
            .Select(x => new DailyInventoryDto()
            {
                CurrentDate = x.CurrentDate,
                InventoryId = x.Id.ToString(),
                InventoryNum = x.InventoryNum,
                Status = (x.IsEnable == YesOrNoType.Yes ? true : false),
            }).ToListAsync(),
            PricePlan = pricePlan,
        };
        return model;
    }

    public async Task<bool> SaveInventory(HotelRoomListDto ebkRoom, List<DailyInventoryModel> dailyInventoryList)
    {
        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            var dataList = await db.Queryable<DailyInventoryDo>().Where(vv => vv.RoomId == SqlFunc.ToInt64(ebkRoom.Id) &&
                                                         vv.CurrentDate >= dailyInventoryList.Min(vv => vv.CurrentDate) &&
                                                          vv.CurrentDate <= dailyInventoryList.Max(vv => vv.CurrentDate)).ToArrayAsync();

            foreach (var item in dailyInventoryList)
            {
                var updateObj = dataList.Where(vv => vv.CurrentDate == item.CurrentDate).ToList().FirstOrDefault();
                updateObj.SetInventoryNum(item.InventoryNum);
                updateObj.SetIsEnable(item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No);

                db.Updateable<DailyInventoryDo>(updateObj).ExecuteCommand();
            }
            return true;
        });

    }

    public async Task<bool> SavePrice(string userPlanId, List<DailyPriceModel> priceList)
    {
        var userRoomObj = db.Queryable<HotelRoomDo>()
                            .InnerJoin<PricePlanDo>((x1, x2) => x1.Id == x2.HotelRoomId)
                            .Where((x1, x2) => x2.Id == SqlFunc.ToInt64(userPlanId))
                            .Select((x1, x2) => new HotelRoomListDto()
                            {
                                Id = x1.Id.ToString(),
                                StartDate = x1.StartDate,
                                EndDate = x1.EndDate,
                            }).ToListAsync().Result.First();

        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            var dataList = await db.Queryable<DailyPriceDo>().Where(vv => vv.RoomId == SqlFunc.ToInt64(userRoomObj.Id) &&
                                                         vv.CurrentDate >= priceList.Min(vv => vv.CurrentDate) &&
                                                          vv.CurrentDate <= priceList.Max(vv => vv.CurrentDate)).ToArrayAsync();



            foreach (var item in priceList)
            {
                var updateObj = dataList.Where(vv => vv.CurrentDate == item.CurrentDate).ToList().FirstOrDefault();
                updateObj.SetPrice(item.Price);
                updateObj.SetIsEnable(item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No);

                db.Updateable<DailyPriceDo>(updateObj).ExecuteCommand();
            }
            return true;
        });
    }

    public async Task<bool> SaveInventoryAndPriceAsync(SaveInventoryAndPriceCmd cmd)
    {

        var inventoryIds = cmd.Inventorys.Select(t => long.Parse(t.InventoryId)).ToList();
        var oldInventory = await DailyInventoryRepo.GetListAsync(x => inventoryIds.Contains(x.Id));
        oldInventory.ForEach(item =>
        {
            var model = cmd.Inventorys.Single(t => t.InventoryId == item.Id.ToString());
            item.SetInventoryNum(model.InventoryNum);
            item.SetIsEnable(model.Status);
        });
        var priceIds = cmd.Prices.Select(t => long.Parse(t.PriceId)).ToList();
        var oldPrice = await DailyPriceRepo.GetListAsync(x => priceIds.Contains(x.Id));
        oldPrice.ForEach(item =>
        {
            var model = cmd.Prices.Single(t => t.PriceId == item.Id.ToString());
            item.SetPrice(model.Price);
            item.SetIsEnable(model.Status);
        });
        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            await db.Updateable(oldInventory).ExecuteCommandAsync();
            await db.Updateable(oldPrice).ExecuteCommandAsync();
            return true;
        });
    }

    public async Task<List<RoomPricingPlanListDto>> PricePlanListDataByRoomIdAsync(long roomId)
    {
        return await PricePlanRepo.AsQueryable()
             .Where(x => x.HotelRoomId == roomId)
             .Select(x => new RoomPricingPlanListDto()
             {
                 IsEnabled = x.IsEnable,
                 PricePlanId = x.Id.ToString(),
                 PricePlanTitle = x.PricePlanTitle ?? string.Empty,
                 RoomId = x.HotelRoomId.ToString(),
             }).ToListAsync();
    }

    /// <summary>
    /// 保存加载库存和价格数据
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public async Task<bool> SaveLoadingInventoryAndPricesAsync(SaveLoadingInventoryAndPricesCmd cmd)
    {
        DateTime startDate = DateTime.Now;

        var roomIds = cmd.Rooms.Select(t => long.Parse(t.RoomId)).ToList();
        //查询房间日期范围内的库存数据
        var oldInventory = await DailyInventoryRepo.AsQueryable()
             .Where(x => x.CurrentDate >= startDate.Date && x.CurrentDate <= cmd.EndDate.Date && roomIds.Contains(x.RoomId))
             .ToListAsync();


        //查询房间日期范围内的价格数据
        var oldPrice = await DailyPriceRepo.AsQueryable()
            .Where(x => x.CurrentDate >= startDate.Date && x.CurrentDate <= cmd.EndDate.Date && roomIds.Contains(x.RoomId))
            .ToListAsync();

        List<DailyInventoryDo> insertDailyInventoryDos = new();
        List<DailyInventoryDo> updateDailyInventoryDos = new();
        List<DailyPriceDo> insertDailyPriceDos = new();
        List<DailyPriceDo> updateDailyPriceDos = new();
        foreach (var item in cmd.Rooms)
        {
            long roomId = item.RoomId.ToLong();

            for (DateTime date = startDate; date <= cmd.EndDate.Date; date = date.AddDays(1).Date)
            {
                //创建库存数据
                DayOfWeek dayOfWeek = date.DayOfWeek;
                int inventoryNum = cmd.Rooms.Single(t => t.RoomId == item.RoomId).Inventory.GetValueOrDefault(dayOfWeek);
                var oldInventoryModel = oldInventory.SingleOrDefault(x => x.CurrentDate == date && x.RoomId == roomId);
                if (oldInventoryModel is null) insertDailyInventoryDos.Add(DailyInventoryDo.Create(item.RoomId.ToLong(), date, inventoryNum, inventoryNum > 0 ? YesOrNoType.Yes : YesOrNoType.No).CreateByInfo(CurrentUserApp.TenantId.ToLong(), CurrentUserApp.Id, CurrentUserApp.UserName));
                else
                {
                    oldInventoryModel.SetInventoryNum(inventoryNum);
                    oldInventoryModel.SetIsEnable(inventoryNum > 0 ? YesOrNoType.Yes : YesOrNoType.No);
                    oldInventoryModel.UpdateByInfo(CurrentUserApp.Id, CurrentUserApp.UserName);
                    updateDailyInventoryDos.Add(oldInventoryModel);
                }

                //创建价格数据
                var pricePlan = cmd.Rooms.Single(t => t.RoomId == item.RoomId).Prices;
                pricePlan.ForEach(p =>
                {
                    decimal price = p.DailyPrices.GetValueOrDefault(dayOfWeek);
                    var oldPriceModel = oldPrice.SingleOrDefault(x => x.CurrentDate == date && x.RoomId == roomId && x.PricePlanId == p.PricePlanId.ToLong());
                    if (oldPriceModel is null) insertDailyPriceDos.Add(DailyPriceDo.Create(item.RoomId.ToLong(), p.PricePlanId.ToLong(), date, price, price > 0 ? YesOrNoType.Yes : YesOrNoType.No).CreateByInfo(CurrentUserApp.TenantId.ToLong(), CurrentUserApp.Id, CurrentUserApp.UserName));
                    else
                    {
                        oldPriceModel.SetPrice(price);
                        oldPriceModel.SetIsEnable(price > 0 ? YesOrNoType.Yes : YesOrNoType.No);
                        oldPriceModel.UpdateByInfo(CurrentUserApp.Id, CurrentUserApp.UserName);
                        updateDailyPriceDos.Add(oldPriceModel);
                    }
                });
            }
        }

        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            //var insertInventory = dailyInventoryDos.Where(x => x.Id == 0).ToList();
            //var updateInventory = dailyInventoryDos.Where(x => x.Id > 0).ToList();
            var ttx = await db.Fastest<DailyInventoryDo>().PageSize(1000).BulkCopyAsync(insertDailyInventoryDos);
            await db.Fastest<DailyInventoryDo>().PageSize(1000).BulkUpdateAsync(updateDailyInventoryDos);

            //var insertPrice = dailyPriceDos.Where(x => x.Id == 0).ToList();
            //var updatePrice = dailyPriceDos.Where(x => x.Id > 0).ToList();
            await db.Fastest<DailyPriceDo>().PageSize(1000).BulkCopyAsync(insertDailyPriceDos);
            await db.Fastest<DailyPriceDo>().PageSize(1000).BulkUpdateAsync(updateDailyPriceDos);
            return true;
        });
    }


}
