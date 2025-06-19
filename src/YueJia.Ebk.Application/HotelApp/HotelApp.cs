using Dm.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.AggRoot;
using YueJia.Ebk.Domain.Company;
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


    //private ISimpleClient<RoomInventoryDo> RoomStockRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<RoomInventoryDo>>();

    private IMongoDatabase MongoDb => LazyServiceProvider.LazyGetRequiredService<IMongoDatabase>();




    public async Task<bool> AddHotelRoomAsync(CreateHotelRoomCmd cmd)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateHotelRoomCmd>>().ValidateAndThrowAsync(cmd);
        if (db.Queryable<HotelRoomDo>().Any(vv => vv.HotelId == SqlFunc.ToInt64(cmd.HotelId) && vv.RoomType == cmd.RoomType && vv.CreatedbyId == CurrentUserApp.Id))
        {
            throw new InvalidOperationException("数据存在，请勿重复创建");
        }
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

        var hotelRoomObj = db.Queryable<HotelRoomDo>().Where(vv => vv.Id == SqlFunc.ToInt64(cmd.HotelRoomId) && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList().First();
        var entity = PricePlanDo.Create(cmd.HotelRoomId.ToLong(), string.Empty, cmd.BreakfastType, cmd.DaysInAdvance, cmd.ContinuousStayDays, cmd.IsReservedRoom, cmd.IsEnable);
        entity.PricePlanTitle = $@"{hotelRoomObj.HotelRoomTitle}<{(cmd.BreakfastType == BreakfastTypeEnum.Breakfast ? "含早" : "无早")}><提前{cmd.DaysInAdvance}天><连住{cmd.ContinuousStayDays}天><{(cmd.IsReservedRoom == YesOrNoType.Yes ? "保留房" : "非保留房")}>";

        var LowestPrice = db.Queryable<HotelPublishDo>()
                            .InnerJoin<HotelRoomDo>((x1, x2) => x1.Id == x2.HotelId && x1.TenantId == x2.TenantId)
                            .Where((x1, x2) => x2.Id == SqlFunc.ToInt64(cmd.HotelRoomId) && x2.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId))
                            .ToList().First().LowestPrice;

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
            if (price > 0 && price < LowestPrice)
            {
                throw new InvalidOperationException($@"酒店最低价格为{LowestPrice}！");
            }

            dailyPriceDoList.Add(new DailyPriceDo()
            {
                RoomId = hotelRoomObj.Id,
                PricePlanId = entity.Id,
                Price = price,
                IsEnable = YesOrNoType.Yes,
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
        var roomInventoryList = db.Queryable<DailyInventoryDo>().Where(vv => vv.RoomId == entity.Id && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList();
        roomInventoryList.ForEach(x => x.IsDelete = true);

        //价格计划
        var pricePlanEntity = PricePlanRepo.AsQueryable().Where(x => x.HotelRoomId == entity.Id && x.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList();
        pricePlanEntity.ForEach(x => x.IsDelete = true);


        //价格计划价格
        var dailyPriceDoList = db.Queryable<DailyPriceDo>().Where(vv => vv.RoomId == entity.Id && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList();
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
        var dailyPriceDoList = db.Queryable<DailyPriceDo>().Where(vv => vv.RoomId == entity.HotelRoomId && vv.PricePlanId == entity.Id && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList();
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">用户酒店Id</param>
    /// <returns></returns>
    public async Task<List<HotelRoomListDto>> GetHotelRoomListByIdAsync(long id)
    {
        var userRoomList = await HotelRoomRepo.AsQueryable().Where(x => x.HotelId == id && x.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId))
                                                    .Select(x => new HotelRoomListDto()
                                                    {
                                                        Id = x.Id.ToString(),
                                                        RoomType = x.RoomType,
                                                        HotelRoomTitle = x.HotelRoomTitle,
                                                        BedType = x.BedType,
                                                        MaximumNumberOfPeople = x.MaximumNumberOfPeople,
                                                        AdultLimit = x.AdultLimit,
                                                        ChildLimit = x.ChildLimit,
                                                        HotelCode = x.HotelCode,
                                                        IsEnabled = x.IsEnabled,
                                                        StartDate = x.StartDate,
                                                        EndDate = x.EndDate
                                                    }).ToListAsync();



        var userPlanList = await PricePlanRepo.AsQueryable()
                                        .InnerJoin<HotelRoomDo>((x1, x2) => x1.HotelRoomId == x2.Id && x1.TenantId == x2.TenantId)
                                        .Where((x1, x2) => x2.HotelId == id && x2.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId))
                                        .Select((x1, x2) => new PricePlanListDto()
                                        {
                                            Id = x1.Id.ToString(),
                                            BreakfastType = x1.BreakfastType,
                                            DaysInAdvance = x1.DaysInAdvance,
                                            ContinuousStayDays = x1.ContinuousStayDays,
                                            IsReservedRoom = x1.IsReservedRoom,
                                            IsEnable = x1.IsEnable,
                                            HotelRoomId = x1.HotelRoomId.ToString(),
                                            PricePlanTitle = x1.PricePlanTitle
                                        }).ToListAsync();


        return userRoomList.Select(vv =>
        {
            vv.PlanList = userPlanList.Where(pp => pp.HotelRoomId == vv.Id).ToList();
            return vv;
        }).ToList();

        //var itemGroups = items.GroupBy(i => i.HotelRoomId).ToDictionary(g => g.Key, g => g.ToList());

        //string hotelCode = userRoomList?.FirstOrDefault()?.HotelCode ?? string.Empty;

        //var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
        //    .Where(q => q.pfcode == "D" && q.hotelcode == hotelCode)
        //    .Select(t => new { t.roomcode, t.roomname })
        //    .ToListAsync();

        //var result = userRoomList?.Select(item =>
        // {
        //     item.RoomTypeName = currentHotelRoomTypeDate.SingleOrDefault(x => x.roomcode == int.Parse(item.RoomType))?.roomname ?? string.Empty;

        //     item.PricePlans =    //itemGroups.TryGetValue(item.Id.ToString(), out var itemList) ? itemList : new();


        //     return item;
        // }).ToList();

        //return userRoomList ?? new();
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
                HotelNameEn = t2.HotelNameEn,
                PricePlanTitle = t.PricePlanTitle,
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

        return null;

        //var RoomEntitys = await HotelRoomRepo.GetListAsync(x => x.HotelId == long.Parse(qry.UserHotelId));

        //var roomIds = RoomEntitys.Select(x => x.Id).ToList();

        //var pricePlanEntities = await PricePlanRepo.GetFirstAsync(x => x.HotelRoomId == long.Parse(qry.UserRoomId));


        //var hotelCode = RoomEntitys.FirstOrDefault()?.HotelCode ?? throw new InvalidOperationException("酒店编码不存在！");

        //var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>()
        //                      .Where(q => q.pfcode == "D" && q.hotelcode == hotelCode)
        //                      .Select(t => new { t.roomcode, t.roomname })
        //                      .ToListAsync();
        //var RoomStockEntitys = await RoomStockRepo.GetListAsync(x => x.CurrentDate >= qry.StartDate && x.CurrentDate <= qry.StartDate.AddDays(qry.DataNumber) && roomIds.Contains(long.Parse(qry.UserRoomId)));

        //return new()
        //{


        //    RoomTypeDropdownList = RoomEntitys.Select(x => new SelectDataDto<string>() { Value = x.RoomType, Label = currentHotelRoomTypeDate.FirstOrDefault(y => y.roomcode == int.Parse(x.RoomType))?.roomname ?? string.Empty }).ToList(),
        //    Room = RoomEntitys.Where(x => x.Id == 5).Select(x => new Room()
        //    {
        //        Id = x.Id.ToString(),
        //        RoomName = currentHotelRoomTypeDate.FirstOrDefault(y => y.roomcode == int.Parse(x.RoomType))?.roomname ?? string.Empty,
        //        Status = x.IsEnabled,
        //        Inventories = RoomStockEntitys.Select(t => new Inventory()
        //        {
        //            Id = t.Id.ToString(),
        //            MonthDay = t.CurrentDate.ToString("MM-dd"),
        //            InventoryNum = t.StockNum,
        //            Status = t.IsEnabled,
        //            DayOfWeek = t.CurrentDate.DayOfWeek.ToString(),

        //        }).ToList()

        //    }).FirstOrDefault() ?? new(),
        //    PricePlan = new PricePlan
        //    {
        //        Id = pricePlanEntities?.Id.ToString() ?? string.Empty,
        //        Name = "价格计划名称Remark",
        //        Status = pricePlanEntities?.IsEnable ?? YesOrNoType.No,
        //        Prices = RoomStockEntitys.Select(x => new PriceItem()
        //        {
        //            Id = x.Id.ToString(),
        //            Price = x.Price,
        //            Status = x.IsEnabled,

        //        }).ToList()
        //    }
        //};
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
                                  .InnerJoin<PricePlanDo>((x1, x2) => x1.Id == x2.HotelRoomId && x1.TenantId == x2.TenantId)
                                  .Where((x1, x2) => x2.Id == userPlanId && x2.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId))
                                  .Select((x1, x2) => new HotelRoomListDto()
                                  {
                                      Id = SqlFunc.ToString(x1.Id),
                                      StartDate = x1.StartDate,
                                      EndDate = x1.EndDate,
                                  }).ToListAsync().Result.First();



        var dataList = await db.Queryable<DailyPriceDo>().Where(vv => vv.PricePlanId == userPlanId
                                                          && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)
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
        var result = new InventoryAndPriceDto();
        var userRoomObj = await HotelRoomRepo.GetByIdAsync(qry.UserRoomId);
        DateTime endDate = qry.StartDate.AddDays(qry.DataNumber);

        var tempList = (await PricePlanRepo.AsQueryable()
                                                .LeftJoin<DailyPriceDo>((o, d) => o.Id == d.PricePlanId && o.TenantId == d.TenantId)
                                                .Where((o, d) => o.HotelRoomId == long.Parse(qry.UserRoomId) && d.CurrentDate >= qry.StartDate && d.CurrentDate < endDate)
                                                .Select((o, d) => new
                                                {
                                                    plan = o,
                                                    dailyPrice = d
                                                })
                                                .ToListAsync());


        var userPlanList = tempList.GroupBy(vv => vv.plan).Select(vv => new PricePlanItemDto()
        {
            DailyPriceList = tempList.Where(pp => pp.dailyPrice.PricePlanId == vv.Key.Id).Select(pp => new DailyPriceModel()
            {
                Price = pp.dailyPrice.Price,
                CurrentDate = pp.dailyPrice.CurrentDate,
                StatusBool = (pp.dailyPrice.IsEnable == YesOrNoType.Yes ? true : false),
            }).ToList(),
            PricePlanId = vv.Key.Id.ToString(),
            PricePlanName = vv.Key.PricePlanTitle,
            RoomId = vv.Key.HotelRoomId.ToString(),
            Status = vv.Key.IsEnable,
        }).ToList();

        return new InventoryAndPriceDto()
        {
            UserHotelId = qry.UserHotelId,
            UserRoomId = userRoomObj.Id.ToString(),
            UserRoomTitle = userRoomObj.HotelRoomTitle,
            StartDateString = userRoomObj.StartDate.ToString("yyyy-MM-dd"),
            EndDateString = userRoomObj.EndDate.ToString("yyyy-MM-dd"),

            PlanList = userPlanList,
            DailyInventoryList = db.Queryable<DailyInventoryDo>().Where(vv => vv.RoomId == SqlFunc.ToInt64(qry.UserRoomId) &&
                                                                             vv.CurrentDate >= qry.StartDate &&
                                                                             vv.CurrentDate < endDate)
                                                                      .Select(vv => new DailyInventoryModel()
                                                                      {
                                                                          CurrentDate = vv.CurrentDate,
                                                                          InventoryNum = vv.InventoryNum,
                                                                          StatusBool = vv.IsEnable == YesOrNoType.Yes
                                                                      }).ToList()
        };
    }

    /// <summary>
    /// 保存==> 库存日历
    /// </summary>
    /// <param name="ebkRoom"></param>
    /// <param name="dailyInventoryList"></param>
    /// <returns></returns>
    public async Task<bool> SaveInventory(HotelRoomListDto ebkRoom, List<DailyInventoryModel> dailyInventoryList)
    {
        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            var dataList = await db.Queryable<DailyInventoryDo>().Where(vv => vv.RoomId == SqlFunc.ToInt64(ebkRoom.Id) &&
                                                                              vv.CurrentDate >= dailyInventoryList.Min(vv => vv.CurrentDate) &&
                                                                              vv.CurrentDate <= dailyInventoryList.Max(vv => vv.CurrentDate)).ToListAsync();

            foreach (var item in dailyInventoryList)
            {
                var updateObj = dataList.Where(vv => vv.CurrentDate == item.CurrentDate).ToList().FirstOrDefault();

                if (item.InventoryNum == updateObj.InventoryNum && updateObj.IsEnable == (item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No))
                {
                    dataList.Remove(updateObj);
                    continue;
                }

                updateObj.SetInventoryNum(item.InventoryNum);
                updateObj.SetIsEnable(item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No);
            }
            db.Updateable<DailyInventoryDo>(dataList).ExecuteCommand();
            return true;
        });

    }

    public async Task<bool> SavePrice(string userPlanId, List<DailyPriceModel> priceList)
    {
        var LowestPrice = db.Queryable<HotelPublishDo>()
                            .InnerJoin<HotelRoomDo>((x1, x2) => x1.Id == x2.HotelId && x1.TenantId == x2.TenantId)
                            .InnerJoin<PricePlanDo>((x1, x2, x3) => x3.HotelRoomId == x2.Id && x3.TenantId == x2.TenantId)
                            .Where((x1, x2, x3) => x3.Id == SqlFunc.ToInt64(userPlanId) && x3.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId))
                            .ToList().First().LowestPrice;

        if (LowestPrice > 0 && priceList.Any(vv => vv.Price > 0 && vv.Price < LowestPrice))
        {
            throw new InvalidOperationException($@"酒店最低价格为{LowestPrice}！");
        }
        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            var dataList = await db.Queryable<DailyPriceDo>().Where(vv => vv.PricePlanId == SqlFunc.ToInt64(userPlanId) &&
                                                                          vv.CurrentDate >= priceList.Min(vv => vv.CurrentDate) &&
                                                                          vv.CurrentDate <= priceList.Max(vv => vv.CurrentDate)).ToListAsync();

            foreach (var item in priceList)
            {
                var updateObj = dataList.Where(vv => vv.CurrentDate == item.CurrentDate).ToList().FirstOrDefault();
                if (item.Price == updateObj.Price && updateObj.IsEnable == (item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No))
                {
                    dataList.Remove(updateObj);
                    continue;
                }
                updateObj.SetPrice(item.Price);
                updateObj.SetIsEnable(item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No);
            }
            if (dataList.Count > 0)
            {
                db.Updateable<DailyPriceDo>(dataList).ExecuteCommand();
            }
            return true;
        });
    }


    public async Task<bool> SaveInventoryAndPriceAsync(InventoryAndPriceDto cmd)
    {
        var LowestPrice = db.Queryable<HotelPublishDo>().Where(vv => vv.Id == SqlFunc.ToInt64(cmd.UserHotelId)).ToList().First().LowestPrice;

        if (LowestPrice > 0 && cmd.PlanList.SelectMany(vv => vv.DailyPriceList).ToList().Any(vv => vv.Price > 0 && vv.Price < LowestPrice))
        {
            throw new InvalidOperationException($@"酒店最低价格为{LowestPrice}！");
        }


        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            List<DailyInventoryDo> waitHandleIinventoryDataList = new List<DailyInventoryDo>();
            List<DailyPriceDo> waitHandlePriceDataList = new List<DailyPriceDo>();

            //更新库存
            var inventoryDataList = await db.Queryable<DailyInventoryDo>().Where(vv => vv.RoomId == SqlFunc.ToInt64(cmd.UserRoomId) &&
                                                                             vv.CurrentDate >= cmd.DailyInventoryList.Min(vv => vv.CurrentDate) &&
                                                                             vv.CurrentDate <= cmd.DailyInventoryList.Max(vv => vv.CurrentDate)).ToListAsync();

            foreach (var item in cmd.DailyInventoryList)
            {
                var updateObj = inventoryDataList.Where(vv => vv.CurrentDate == item.CurrentDate).ToList().FirstOrDefault();

                if (item.InventoryNum == updateObj.InventoryNum && updateObj.IsEnable == (item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No))
                {
                    continue;
                }

                updateObj.SetInventoryNum(item.InventoryNum);
                updateObj.SetIsEnable(item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No);

                waitHandleIinventoryDataList.add(updateObj);
            }


            //更新价格
            foreach (var ele in cmd.PlanList)
            {
                var dataList = await db.Queryable<DailyPriceDo>().Where(vv => vv.PricePlanId == SqlFunc.ToInt64(ele.PricePlanId) &&
                                                                       vv.CurrentDate >= ele.DailyPriceList.Min(vv => vv.CurrentDate) &&
                                                                       vv.CurrentDate <= ele.DailyPriceList.Max(vv => vv.CurrentDate)).ToListAsync();

                foreach (var item in ele.DailyPriceList)
                {
                    var updateObj = dataList.Where(vv => vv.CurrentDate == item.CurrentDate).ToList().FirstOrDefault();
                    if (item.Price == updateObj.Price && updateObj.IsEnable == (item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No))
                    {
                        dataList.Remove(updateObj);
                        continue;
                    }
                    updateObj.SetPrice(item.Price);
                    updateObj.SetIsEnable(item.StatusBool ? YesOrNoType.Yes : YesOrNoType.No);

                    waitHandlePriceDataList.add(updateObj);
                }
            }

            db.Updateable<DailyInventoryDo>(waitHandleIinventoryDataList).ExecuteCommand();
            db.Updateable<DailyPriceDo>(waitHandlePriceDataList).ExecuteCommand();
            return true;
        });


        //var inventoryIds = cmd.Inventorys.Select(t => long.Parse(t.InventoryId)).ToList();
        //var oldInventory = await DailyInventoryRepo.GetListAsync(x => inventoryIds.Contains(x.Id));
        //oldInventory.ForEach(item =>
        //{
        //    var model = cmd.Inventorys.Single(t => t.InventoryId == item.Id.ToString());
        //    item.SetInventoryNum(model.InventoryNum);
        //    item.SetIsEnable(model.Status = (model.InventoryNum <= 0 ? YesOrNoType.No : model.Status));
        //});
        //var priceIds = cmd.Prices.Select(t => long.Parse(t.PriceId)).ToList();
        //var oldPrice = await DailyPriceRepo.GetListAsync(x => priceIds.Contains(x.Id));
        //oldPrice.ForEach(item =>
        //    {
        //        var model = cmd.Prices.Single(t => t.PriceId == item.Id.ToString());
        //        item.SetPrice(model.Price);
        //        item.SetIsEnable(model.Status = (model.Price <= 0 ? YesOrNoType.No : model.Status));
        //    });
        //return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        //{
        //    await db.Updateable(oldInventory).ExecuteCommandAsync();
        //    await db.Updateable(oldPrice).ExecuteCommandAsync();
        //    return true;
        //});
    }

    /// <summary>
    /// 批量编辑库存与价格=> 简易模式
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    public async Task<bool> BatchSaveInventoryAndPricesSimple(BatchEditInventoryAndPricesModel qry)
    {
        var startDate = Convert.ToDateTime(qry.startDate);
        var endDate = Convert.ToDateTime(qry.endDate);

        var LowestPrice = db.Queryable<HotelPublishDo>().Where(vv => vv.Id == SqlFunc.ToInt64(qry.userHotelId) && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList().First().LowestPrice;

        List<DailyInventoryDo> updateDailyInventoryList = new List<DailyInventoryDo>();
        List<DailyPriceDo> updateDailyPriceList = new List<DailyPriceDo>();
        //处理库存
        foreach (var userRoomId in qry.userRoomIdList)
        {
            var dailyInventoryList = await db.Queryable<DailyInventoryDo>().Where(vv => vv.RoomId == SqlFunc.ToInt64(userRoomId) && vv.CurrentDate >= startDate && vv.CurrentDate <= endDate).ToListAsync();
            foreach (var dailyInventoryObj in dailyInventoryList)
            {
                if (qry.weekIndexList.Where(weekIndex => weekIndex == ((int)dailyInventoryObj.CurrentDate.DayOfWeek)).Count() == 0)
                {
                    continue;
                }

                var InventoryNum = dailyInventoryObj.InventoryNum;
                var IsEnable = dailyInventoryObj.IsEnable;

                if (qry.inventoryNumFlag)
                {
                    var newInventoryNum = qry.inventoryNum;
                    if (qry.inventoryNumExecType == "2")
                    {
                        newInventoryNum = qry.inventoryNum + dailyInventoryObj.InventoryNum;
                    }
                    dailyInventoryObj.SetInventoryNum(newInventoryNum);
                }

                if (qry.inventoryStateFlag)
                {
                    dailyInventoryObj.SetIsEnable(qry.inventoryState ? YesOrNoType.Yes : YesOrNoType.No);
                }


                if (InventoryNum != dailyInventoryObj.InventoryNum || IsEnable != dailyInventoryObj.IsEnable)
                {
                    updateDailyInventoryList.Add(dailyInventoryObj);
                }
            }
        }

        //处理价格
        foreach (var userPlanId in qry.userPlanIdList)
        {
            var dailyPriceList = await db.Queryable<DailyPriceDo>().Where(vv => vv.PricePlanId == SqlFunc.ToInt64(userPlanId) && vv.CurrentDate >= startDate && vv.CurrentDate <= endDate).ToListAsync();
            foreach (var dailyPriceObj in dailyPriceList)
            {
                if (qry.weekIndexList.Where(weekIndex => weekIndex == ((int)dailyPriceObj.CurrentDate.DayOfWeek)).Count() == 0)
                {
                    continue;
                }

                var Price = dailyPriceObj.Price;
                var IsEnable = dailyPriceObj.IsEnable;

                if (qry.planPriceFlag)
                {
                    if (qry.planPriceExecType == "1")
                    {
                        dailyPriceObj.SetPrice(qry.planPrice);

                        if (qry.planPrice > 0 && qry.planPrice < LowestPrice)
                        {
                            throw new InvalidOperationException($@"酒店最低价格为{LowestPrice}！");
                        }
                    }
                    if (qry.planPriceExecType == "2" && dailyPriceObj.Price > 0)
                    {
                        dailyPriceObj.SetPrice(qry.planPrice + dailyPriceObj.Price);
                    }
                    if (qry.planPriceExecType == "3")
                    {
                        dailyPriceObj.SetPrice(dailyPriceObj.Price + (dailyPriceObj.Price * (qry.planPrice / 100)));
                    }
                }

                if (qry.planPriceStateFlag)
                {
                    dailyPriceObj.SetIsEnable(qry.planPriceState ? YesOrNoType.Yes : YesOrNoType.No);
                }

                if (Price != dailyPriceObj.Price || IsEnable != dailyPriceObj.IsEnable)
                {
                    updateDailyPriceList.Add(dailyPriceObj);
                }
            }
        }

        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            db.Updateable<DailyInventoryDo>(updateDailyInventoryList).ExecuteCommand();
            db.Updateable<DailyPriceDo>(updateDailyPriceList).ExecuteCommand();
            return true;
        });

    }

    public async Task<bool> BatchSaveInventoryAndPricesSenior([FromBody] BatchEditInventoryAndPricesSeniorModel qry)
    {

        var startDate = Convert.ToDateTime(qry.startDate);
        var endDate = Convert.ToDateTime(qry.endDate);
        var LowestPrice = db.Queryable<HotelPublishDo>().Where(vv => vv.Id == SqlFunc.ToInt64(qry.userHotelId) && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList().First().LowestPrice;


        List<DailyInventoryDo> updateDailyInventoryList = new List<DailyInventoryDo>();
        List<DailyPriceDo> updateDailyPriceList = new List<DailyPriceDo>();

        #region 房间数量
        foreach (var ele in qry.userRoomList)
        {
            var dailyInventoryList = await db.Queryable<DailyInventoryDo>().Where(vv => vv.RoomId == SqlFunc.ToInt64(ele.Id) &&
                                                                                        vv.CurrentDate >= startDate &&
                                                                                        vv.CurrentDate <= endDate &&
                                                                                        vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToListAsync();

            foreach (var dailyInventoryObj in dailyInventoryList)
            {
                if (qry.weekIndexList.Where(weekIndex => weekIndex == ((int)dailyInventoryObj.CurrentDate.DayOfWeek)).Count() == 0)
                {
                    continue;
                }
                var InventoryNum = dailyInventoryObj.InventoryNum;
                var IsEnable = dailyInventoryObj.IsEnable;

                if (ele.inventoryNumExecType == "1")
                {
                    dailyInventoryObj.SetInventoryNum(ele.inventoryNum);
                }
                else if (ele.inventoryNumExecType == "2")
                {
                    dailyInventoryObj.SetInventoryNum(ele.inventoryNum + dailyInventoryObj.InventoryNum);
                }


                if (ele.inventoryStateType == "1")
                {
                    dailyInventoryObj.SetIsEnable(YesOrNoType.Yes);
                }
                else if (ele.inventoryStateType == "2")
                {
                    dailyInventoryObj.SetIsEnable(YesOrNoType.No);
                }

                if (InventoryNum != dailyInventoryObj.InventoryNum || IsEnable != dailyInventoryObj.IsEnable)
                {
                    updateDailyInventoryList.Add(dailyInventoryObj);
                }
            }
        }
        #endregion


        //处理价格
        foreach (var ele in qry.userPlanList)
        {
            var dailyPriceList = await db.Queryable<DailyPriceDo>().Where(vv => vv.PricePlanId == SqlFunc.ToInt64(ele.Id) &&
                                                                                vv.CurrentDate >= startDate &&
                                                                                vv.CurrentDate <= endDate &&
                                                                                vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToArrayAsync();
            foreach (var dailyPriceObj in dailyPriceList)
            {

                if (qry.weekIndexList.Where(weekIndex => weekIndex == ((int)dailyPriceObj.CurrentDate.DayOfWeek)).Count() == 0)
                {
                    continue;
                }

                var Price = dailyPriceObj.Price;
                var IsEnable = dailyPriceObj.IsEnable;


                if (ele.planPriceExecType == "1")
                {
                    dailyPriceObj.SetPrice(ele.planPrice);

                    if (ele.planPrice > 0 && ele.planPrice < LowestPrice)
                    {
                        throw new InvalidOperationException($@"酒店最低价格为{LowestPrice}！");
                    }
                }
                else if (ele.planPriceExecType == "2")
                {
                    dailyPriceObj.SetPrice(ele.planPrice + dailyPriceObj.Price);
                }

                if (ele.planPriceStateType == "1")
                {
                    dailyPriceObj.SetIsEnable(YesOrNoType.Yes);
                }
                else if (ele.planPriceStateType == "2")
                {
                    dailyPriceObj.SetIsEnable(YesOrNoType.No);
                }


                if (Price != dailyPriceObj.Price || IsEnable != dailyPriceObj.IsEnable)
                {
                    updateDailyPriceList.Add(dailyPriceObj);
                }
            }
        }

        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            db.Updateable<DailyInventoryDo>(updateDailyInventoryList).ExecuteCommand();
            db.Updateable<DailyPriceDo>(updateDailyPriceList).ExecuteCommand();
            return true;
        });

    }

    public async Task<LoadingInventoryAndPriceModel> PricePlanListDataByRoomIdAsync(string UserRoomId)
    {

        var userRoom = await HotelRoomRepo.GetByIdAsync(Convert.ToInt64(UserRoomId));

        var planList = await PricePlanRepo.AsQueryable().Where(vv => vv.HotelRoomId == SqlFunc.ToInt64(UserRoomId) &&
                                                                     vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId))
                                                        .Select(vv => new LoadingInventoryAndPricePlanModel
                                                        {
                                                            UserPlanId = SqlFunc.ToString(vv.Id),
                                                            UserPlanTitel = vv.PricePlanTitle,
                                                            UserPlanStatusBool = vv.IsEnable == YesOrNoType.Yes,
                                                        }).ToListAsync();

        return new LoadingInventoryAndPriceModel
        {
            UserRoomId = UserRoomId,
            StartDateString = userRoom.StartDate.ToString("yyyy-MM-dd"),
            EndDateString = userRoom.EndDate.ToString("yyyy-MM-dd"),
            PlanList = planList,
        };
    }

    /// <summary>
    /// 保存加载库存和价格数据
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public async Task<bool> SaveLoadingInventoryAndPricesAsync(LoadingInventoryAndPriceModel cmd)
    {

        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            //第一步验证库存有效期
            var userRoom = await HotelRoomRepo.GetByIdAsync(Convert.ToInt64(cmd.UserRoomId));
            if (userRoom.EndDate >= Convert.ToDateTime(cmd.NewEndDateString))
            {
                throw new InvalidOperationException($@"新截至时间必须大于{userRoom.EndDate.ToString("yyyy-MM-dd")}！");
            }

            var LowestPrice = db.Queryable<HotelPublishDo>().Where(vv => vv.Id == SqlFunc.ToInt64(cmd.UserHotelId)).ToList().First().LowestPrice;
            if (LowestPrice > 0 && cmd.PlanList.Any(vv => (vv.Monday > 0 && vv.Monday < LowestPrice) ||
                                                          (vv.Tuesday > 0 && vv.Tuesday < LowestPrice) ||
                                                          (vv.Wednesday > 0 && vv.Wednesday < LowestPrice) ||
                                                          (vv.Thursday > 0 && vv.Thursday < LowestPrice) ||
                                                          (vv.Friday > 0 && vv.Friday < LowestPrice) ||
                                                          (vv.Saturday > 0 && vv.Saturday < LowestPrice) ||
                                                          (vv.Sunday > 0 && vv.Sunday < LowestPrice)))
            {
                throw new InvalidOperationException($@"酒店最低价格为{LowestPrice}！");
            }



            //库存处理
            List<DailyInventoryDo> dailyInventoryDoList = new List<DailyInventoryDo>();
            for (DateTime date = userRoom.EndDate.AddDays(1); date <= Convert.ToDateTime(cmd.NewEndDateString); date = date.AddDays(1))
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
                dailyInventoryDoList.Add(DailyInventoryDo.Create(userRoom.Id, date, stockNum, (stockNum > 0 ? YesOrNoType.Yes : YesOrNoType.No)));
            }

            List<DailyPriceDo> dailyPriceDoList = new List<DailyPriceDo>();
            foreach (var userPlan in cmd.PlanList)
            {

                for (DateTime date = userRoom.EndDate.AddDays(1); date <= Convert.ToDateTime(cmd.NewEndDateString); date = date.AddDays(1))
                {
                    decimal price = Convert.ToDecimal(0);
                    switch (date.DayOfWeek)
                    {
                        case DayOfWeek.Monday: price = userPlan.Monday; break;
                        case DayOfWeek.Tuesday: price = userPlan.Tuesday; break;
                        case DayOfWeek.Wednesday: price = userPlan.Wednesday; break;
                        case DayOfWeek.Thursday: price = userPlan.Thursday; break;
                        case DayOfWeek.Friday: price = userPlan.Friday; break;
                        case DayOfWeek.Saturday: price = userPlan.Saturday; break;
                        case DayOfWeek.Sunday: price = userPlan.Sunday; break;
                    }

                    dailyPriceDoList.Add(new DailyPriceDo()
                    {
                        RoomId = userRoom.Id,
                        PricePlanId = Convert.ToInt64(userPlan.UserPlanId),
                        Price = price,
                        IsEnable = price > 0 ? YesOrNoType.Yes : YesOrNoType.No,
                        CurrentDate = date,
                    });
                }
            }
            userRoom.EndDate = Convert.ToDateTime(cmd.NewEndDateString);
            await db.Updateable<HotelRoomDo>(userRoom).ExecuteCommandAsync();
            await db.Insertable<DailyInventoryDo>(dailyInventoryDoList).ExecuteCommandAsync();
            await db.Insertable<DailyPriceDo>(dailyPriceDoList).ExecuteCommandAsync();

            return true;
        });
    }

    public async Task<bool> UpdateHotelState(string userHotelId)
    {
        //房间
        var entity = HotelPublishRepo.GetById(userHotelId.ToLong());
        if (entity == null)
        {
            throw new InvalidOperationException("数据不存在！");
        }
        if (entity.Status == HotelSaleTypeEnum.Down)
        {
            entity.SetStatus(HotelSaleTypeEnum.Up);
        }
        else if (entity.Status == HotelSaleTypeEnum.Up)
        {
            entity.SetStatus(HotelSaleTypeEnum.Down);
        }

        return await db.Updateable<HotelPublishDo>(entity).ExecuteCommandWithOptLockAsync(true) > 0;
    }


    public async Task<bool> BatchUpdateHotelState(List<string> userHotelIds, HotelSaleTypeEnum newSaleType)
    {

        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {
            foreach (var userHotelId in userHotelIds)
            {

                var model = db.Queryable<HotelPublishDo>().Single(vv => vv.Id == SqlFunc.ToInt64(userHotelId) && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId));
                if (model.Status == newSaleType)
                {
                    continue;
                }
                model.SetStatus(newSaleType);
                db.Updateable<HotelPublishDo>(model).ExecuteCommand();
            }
            return true;
        });
    }

    public async Task<bool> UserHotelDelete(string userHotelId)
    {
        //房间
        var entity = db.Queryable<HotelPublishDo>().Where(vv => vv.Id == SqlFunc.ToInt64(userHotelId) && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList().FirstOrDefault();
        if (entity == null)
        {
            throw new InvalidOperationException("数据不存在！");
        }
        entity.IsDelete = true;
        await db.Updateable<HotelPublishDo>(entity).ExecuteCommandAsync();
        return true;

    }

    public async Task<bool> CopeUserPlan(CopeUserPlanModel cmd)
    {





        var LowestPrice = db.Queryable<HotelPublishDo>()
                        .InnerJoin<HotelRoomDo>((x1, x2) => x1.Id == x2.HotelId && x1.TenantId == x2.TenantId)
                        .InnerJoin<PricePlanDo>((x1, x2, x3) => x3.HotelRoomId == x2.Id && x3.TenantId == x2.TenantId)
                        .Where((x1, x2, x3) => x3.Id == SqlFunc.ToInt64(cmd.CopeUserPlanId) && x3.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId))
                        .ToList().First().LowestPrice;

        return await DbTransaction.ExecuteInTransactionAsync(db, async () =>
        {

            var userPricePlan = db.Queryable<PricePlanDo>().Where(vv => vv.Id == SqlFunc.ToInt64(cmd.CopeUserPlanId) && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList().First();
            //只能自己复制自己
            if (userPricePlan.CreatedbyId != CurrentUserApp.Id.ToString())
            {
                throw new InvalidOperationException($@"复制错误！");
            }
            var hotelRoomObj = db.Queryable<HotelRoomDo>().Where(vv => vv.Id == userPricePlan.HotelRoomId && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList().First();

            var entity = PricePlanDo.Create(userPricePlan.HotelRoomId,
                                            string.Empty,
                                            cmd.BreakfastType,
                                            cmd.DaysInAdvance,
                                            cmd.ContinuousStayDays,
                                            cmd.IsReservedRoom,
                                            cmd.IsEnable);
            entity.PricePlanTitle = $@"{hotelRoomObj.HotelRoomTitle}<{(cmd.BreakfastType == BreakfastTypeEnum.Breakfast ? "含早" : "无早")}><提前{cmd.DaysInAdvance}天><连住{cmd.ContinuousStayDays}天><{(cmd.IsReservedRoom == YesOrNoType.Yes ? "保留房" : "非保留房")}>";


            List<DailyPriceDo> dailyPriceDoList = new List<DailyPriceDo>();

            var copeDailyPriceList = db.Queryable<DailyPriceDo>().Where(vv => vv.PricePlanId == SqlFunc.ToInt64(cmd.CopeUserPlanId) && vv.TenantId == SqlFunc.ToInt64(CurrentUserApp.TenantId)).ToList();
            foreach (var ele in copeDailyPriceList)
            {
                var price = ele.Price;
                if (price > 0)
                {
                    price = price + cmd.AddPrice;
                }
                if (price > 0 && price < LowestPrice)
                {
                    throw new InvalidOperationException($@"酒店最低价格为{LowestPrice}！");
                }

                dailyPriceDoList.Add(new DailyPriceDo()
                {
                    RoomId = entity.HotelRoomId,
                    PricePlanId = entity.Id,
                    Price = Math.Max(price, 0),
                    IsEnable = YesOrNoType.Yes,
                    CurrentDate = ele.CurrentDate,
                });
            }

            await db.Insertable<DailyPriceDo>(dailyPriceDoList).ExecuteCommandAsync();
            await db.Insertable<PricePlanDo>(entity).ExecuteCommandAsync();
            return true;
        });


    }



    public async Task<HotelPriceDto> PriceCheckQry(PriceCheckQry qry)
    {

        //验证
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<PriceSearchQry>>().ValidateAndThrowAsync(qry);

        //连住天数
        int continuousStayDays = (qry.CheckOutDate.Date - qry.CheckInDate.Date).Days;
        //提前天数
        int advanceDays = (qry.CheckOutDate.Date - DateTime.Now.Date).Days;

        //解密查价唯一值
        var (isSuccess, searchCodeStr) = CompressedEncryptor.Decrypt(qry.SearchCode, SecretKeyConst.key, SecretKeyConst.iv);
        if (!isSuccess || string.IsNullOrWhiteSpace(searchCodeStr)) throw new InvalidOperationException("请勿篡改查价唯一码!");
        //解析查价唯一码
        var searchCode = JsonUtils.AnalysisSearchCode(searchCodeStr);
        if (searchCode is null || searchCode.DailyInventoryIds.Count == 0 || searchCode.DailyPriceIds.Count == 0) throw new InvalidOperationException("查价唯一码无效!");

        //按库存ID擦查询库存集合
        var dailyInventoryDos = await db.Queryable<DailyInventoryDo>()
            .ClearFilter<ITenantIdFilter>()
            .Where(t => searchCode.DailyInventoryIds.Contains(t.Id) && t.IsEnable == YesOrNoType.Yes)
            .ToListAsync();
        //按条件筛选：库存数、日期范围
        var FilterData = dailyInventoryDos.Where(t => t.InventoryNum >= qry.RoomNum && t.CurrentDate >= qry.CheckInDate.Date && t.CurrentDate < qry.CheckOutDate.Date).ToList();
        //验证库存数是否足够
        bool areEqual = searchCode.DailyInventoryIds.Count == FilterData.Count && searchCode.DailyInventoryIds.All(id => dailyInventoryDos.Any(e => e.Id == id));
        if (!areEqual) throw new InvalidOperationException("库存数不足！");
        if (dailyInventoryDos.GroupBy(t => t.CreatedbyId).Count() > 1) throw new InvalidOperationException("当前查价唯一码非同一用户库存！");


        var dailyPriceDos = await db.Queryable<DailyPriceDo>().ClearFilter<ITenantIdFilter>().Where(t => searchCode.DailyPriceIds.Contains(t.Id) && t.IsEnable == YesOrNoType.Yes).ToListAsync();
        if (!dailyPriceDos.Any()) throw new InvalidOperationException("暂无报价数据！");
        if (dailyPriceDos.GroupBy(t => t.CreatedbyId).Count() > 1) throw new InvalidOperationException("当前查价唯一码非同一用户报价！");



        //收集价格计划
        var pricePlanIds = dailyPriceDos.Select(t => t.PricePlanId).Distinct().ToList();

        var hotel = await db.Queryable<HotelRoomDo>()
             .InnerJoin<PricePlanDo>((r, p) => r.Id == p.HotelRoomId)
             .Where((r, p) => r.HotelCode == qry.HotelCode && r.Id == p.HotelRoomId && pricePlanIds.Contains(p.Id) && p.IsEnable == YesOrNoType.Yes && r.IsEnabled == YesOrNoType.Yes)
             .Where((r, p) => r.MaximumNumberOfPeople >= (qry.AdultNum + qry.ChildNum) && r.AdultLimit >= qry.AdultNum && r.ChildLimit >= qry.ChildNum)
             .Where((r, p) => p.ContinuousStayDays <= continuousStayDays && p.DaysInAdvance <= advanceDays)
             .Select((r, p) => new
             {
                 RoomCode = r.RoomType,
                 RoomName = r.HotelRoomTitle ?? string.Empty,
                 HotelCode = r.HotelCode,
                 BreakfastType = p.BreakfastType,
                 PricePlanId = p.Id.ToString(),
                 r.TenantId
             })
             .SingleAsync() ?? throw new InvalidOperationException("未找到符合条件的房间！");






        //查询公司价格调整规则
        var priceAdjustData = await db.Queryable<CompanyDO>()
            .ClearFilter<ITenantIdFilter>()
            .Where(t => t.Status == YesOrNoType.Yes)
            .Select(t => new { t.TenantId, t.AdjustmentPriceType, t.AdjustmentPriceValue })
            .ToListAsync();


        var tempTotalPrice = dailyPriceDos.Sum(t => t.Price);

        var adjustmentPrice = priceAdjustData.FirstOrDefault(t => t.TenantId == hotel.TenantId);
        if (adjustmentPrice is not null)
        {

            decimal adjustDailyPrice = adjustmentPrice.AdjustmentPriceType switch
            {
                AdjustmentPriceTypeEnum.FixedValueIncrease => Math.Ceiling((tempTotalPrice + (adjustmentPrice.AdjustmentPriceValue ?? 0)) / continuousStayDays),
                AdjustmentPriceTypeEnum.PercentageIncrease => Math.Ceiling(tempTotalPrice * (decimal)((double)(adjustmentPrice.AdjustmentPriceValue ?? 0) / 100) / continuousStayDays),
                _ => 0
            };

            foreach (var c in dailyPriceDos)
            {
                c.Price = adjustmentPrice.AdjustmentPriceType switch
                {
                    AdjustmentPriceTypeEnum.FixedValueIncrease => adjustDailyPrice,
                    AdjustmentPriceTypeEnum.PercentageIncrease => c.Price + adjustDailyPrice,
                    _ => c.Price
                };
            }
        }


        return new HotelPriceDto()
        {
            HotelCode = hotel.HotelCode,
            RoomCode = hotel.RoomCode,
            RoomName = hotel.RoomName,
            IsBreakfast = hotel.BreakfastType.ToDescription(),
            PricePlanId = hotel.PricePlanId,
            DayPrice = dailyPriceDos.ToDictionary(t => t.CurrentDate.ToString("yyyy-MM-dd"), t => t.Price),
            TotalPrice = dailyPriceDos.Sum(t => t.Price),
            SearchCode = qry.SearchCode
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


        var filter = Builders<HotelRoomDo>.Filter.And(
                         Builders<HotelRoomDo>.Filter.Eq(x => x.HotelCode, qry.HotelCode),
                         Builders<HotelRoomDo>.Filter.Gte(x => x.MaximumNumberOfPeople, (qry.AdultNum + qry.ChildNum)),  //大于等于
                         Builders<HotelRoomDo>.Filter.Gte(x => x.AdultLimit, qry.AdultNum),
                         Builders<HotelRoomDo>.Filter.Gte(x => x.ChildLimit, qry.ChildNum));

        var roomData = await MongoDb.GetCollection<HotelRoomDo>(nameof(HotelRoomDo)).Find(filter).ToListAsync();

        if (roomData is null || roomData.Count == 0) return default!;

        //收集房间id
        var roomids = roomData!.Select(t => t.Id).ToList();


        //查询库存
        var inventoryData = await MongoDb.GetCollection<DailyInventoryDo>(nameof(DailyInventoryDo))
            .Find(Builders<DailyInventoryDo>.Filter.And(
                    Builders<DailyInventoryDo>.Filter.In(x => x.RoomId, roomids),
                    Builders<DailyInventoryDo>.Filter.Gte(x => x.InventoryNum, qry.RoomNum),
                    Builders<DailyInventoryDo>.Filter.Gte(x => x.CurrentDate, qry.CheckInDate.Date),
                    Builders<DailyInventoryDo>.Filter.Lt(x => x.CurrentDate, qry.CheckOutDate.Date))).ToListAsync();

        //按CreatedbyId 分组 计算每组的记录行数
        var inventoryGroup = inventoryData!.GroupBy(x => new { x.CreatedbyId, x.RoomId })
            .Select(g => new { CreatedbyId = g.Key.CreatedbyId, RoomId = g.Key.RoomId, Count = g.Count(), item = g.ToList() })
            .ToList();
        if (!inventoryGroup.Any(t => t.Count == continuousStayDays)) return default!;


        //查询价格计划
        var pricePlanData = await MongoDb.GetCollection<PricePlanDo>(nameof(PricePlanDo)).Find(Builders<PricePlanDo>.Filter.And(
        Builders<PricePlanDo>.Filter.In(x => x.HotelRoomId, roomids),
        Builders<PricePlanDo>.Filter.Lte(x => x.ContinuousStayDays, continuousStayDays),
        Builders<PricePlanDo>.Filter.Lte(x => x.DaysInAdvance, advanceDays))).ToListAsync();
        if (pricePlanData is null || pricePlanData.Count == 0) return default!;

        //收集价格计划id
        var pricePlanIds = pricePlanData!.Select(t => t.Id).ToList();

        //查询每日价格
        var dailyPriceData = await MongoDb.GetCollection<DailyPriceDo>(nameof(DailyPriceDo)).Find(Builders<DailyPriceDo>.Filter.And(
        Builders<DailyPriceDo>.Filter.In(x => x.PricePlanId, pricePlanIds),
        Builders<DailyPriceDo>.Filter.Gte(x => x.CurrentDate, qry.CheckInDate.Date),
        Builders<DailyPriceDo>.Filter.Lt(x => x.CurrentDate, qry.CheckOutDate.Date))).ToListAsync();

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



        //查询公司价格调整规则
        var priceAdjustData = db.Queryable<CompanyDO>()
            .ClearFilter<ITenantIdFilter>()
            .Select(t => new { t.TenantId, t.AdjustmentPriceType, t.AdjustmentPriceValue })
            .ToListAsync();



        foreach (var roomCodeGroup in RoomCodeGroupData)
        {
            List<HotelPriceDto> roomOptions = new List<HotelPriceDto>();
            foreach (var room in roomCodeGroup.item)
            {
                //每日库存
                var dailyInventory = inventoryGroup.FirstOrDefault(t => room.Id == t.RoomId && t.Count == continuousStayDays);
                if (dailyInventory is null || dailyInventory?.Count <= 0) continue;

                //价格计划   
                var pricePlan = pricePlanData.Where(x => x.HotelRoomId == room.Id && x.ContinuousStayDays <= continuousStayDays && x.DaysInAdvance <= advanceDays).ToList();
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

                    var (isSuccess, searchCode) = CompressedEncryptor.Encrypt(System.Text.Json.JsonSerializer.Serialize(new { DailyPriceIds = models.item.Select(t => t.Id).ToList(), DailyInventoryIds = dailyInventory!.item.Select(t => t.Id).ToList() }), SecretKeyConst.key, SecretKeyConst.iv);
                    if (!isSuccess) throw new InvalidOperationException("查价唯一值加密失败！");

                    var tempTotalPrice = models.item.Sum(t => t.Price);

                    var adjustmentPrice = (await priceAdjustData).FirstOrDefault(t => t.TenantId == item.TenantId);
                    if (adjustmentPrice is not null)
                    {

                        decimal adjustDailyPrice = adjustmentPrice.AdjustmentPriceType switch
                        {
                            AdjustmentPriceTypeEnum.FixedValueIncrease => Math.Ceiling((tempTotalPrice + (adjustmentPrice.AdjustmentPriceValue ?? 0)) / continuousStayDays),
                            AdjustmentPriceTypeEnum.PercentageIncrease => Math.Ceiling(tempTotalPrice * (decimal)((double)(adjustmentPrice.AdjustmentPriceValue ?? 0) / 100) / continuousStayDays),
                            _ => 0
                        };

                        foreach (var c in models.item)
                        {
                            c.Price = adjustmentPrice.AdjustmentPriceType switch
                            {
                                AdjustmentPriceTypeEnum.FixedValueIncrease => adjustDailyPrice,
                                AdjustmentPriceTypeEnum.PercentageIncrease => c.Price + adjustDailyPrice,
                                _ => c.Price
                            };
                        }

                    }
                    roomOptions.Add(new HotelPriceDto()
                    {
                        HotelCode = room.HotelCode,
                        RoomCode = room.RoomType,
                        RoomName = room.HotelRoomTitle ?? string.Empty,
                        SearchCode = searchCode,
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
    }
}
