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

    private ISimpleClient<HotelRoomDo> HotelRoomRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<HotelRoomDo>>();

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();


    private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();

    private ISqlSugarClient SqlSugarClient => LazyServiceProvider.GetRequiredKeyedService<ISqlSugarClient>(DbConst.YueJiaSysDb);



    public async Task<long> AddHotelRoomAsync(CreateHotelRoomCmd cmd)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateHotelRoomCmd>>().ValidateAndThrowAsync(cmd);

        var entity = HotelRoomDo.Create(cmd.HotelCode, cmd.RoomType, cmd.BedType, cmd.MaximumNumberOfPeople,
            cmd.AdultLimit, cmd.ChildLimit, cmd.StartDate, cmd.EndDate, cmd.StockInitValJosn) ?? throw new InvalidOperationException("床间酒店房间信息失败！");


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

    public async Task<List<HotelRoomListDto>> GetHotelRoomByHotelCodeAsync(string hotelCode)
    {
        var data = await HotelRoomRepo.AsQueryable().Where(x => x.HotelCode == hotelCode)
             .Select(x => new HotelRoomListDto()
             {
                 Id = x.Id,
                 RoomType = x.RoomType,
                 BedType = x.BedType,
                 MaximumNumberOfPeople = x.MaximumNumberOfPeople,
                 AdultLimit = x.AdultLimit,
                 ChildLimit = x.ChildLimit
             })
            .ToListAsync();

        var currentHotelRoomTypeDate = await SqlSugarClient.Queryable<OtaRoomEntity>().Where(q => q.pfcode == "D" && q.hotelcode == hotelCode)
          .Select(t => new { t.roomcode, t.roomname }).ToListAsync();
        data = data.Select(item =>
        {
            item.RoomTypeName = currentHotelRoomTypeDate.SingleOrDefault(x => x.roomcode == int.Parse(item.RoomType))?.roomname ?? string.Empty;
            return item;
        }).ToList();
        return data;
    }
}
