using Microsoft.Extensions.DependencyInjection;
using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
using YueJia.Ebk.Application.Contracts.OuterServiceApp.Entity;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Shared.Const;
using YueJia.Ebk.Domain.SysUser;


namespace YueJia.Ebk.Application.HotelApp;

[DisableValidation]
public class HotelPublishApp : ApplicationService, IHotelPublishApp
{

    private ISimpleClient<HotelPublishDo> HotelPublishRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<HotelPublishDo>>();

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();


    private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();

    private ISimpleClient<SysUserDo> SysUserRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<SysUserDo>>();

    private ISqlSugarClient SqlSugarClient => LazyServiceProvider.GetRequiredKeyedService<ISqlSugarClient>(DbConst.YueJiaSysDb);


    public async Task<HotelPublishDetailDto> GetHotelPublishDetailAsync(long id)
    {
        var entity = await HotelPublishRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"酒店ID:{id}资源不存在！");
        return new HotelPublishDetailDto()
        {
            Id = entity.Id.ToString(),
            HotelCode = entity.HotelCode,
            HotelName = entity.HotelName,
            HotelNameEn = entity.HotelNameEn,
            Address = entity.Address,
            AddressEn = entity.AddressEn,
            Status = entity.Status,
            TelPhone = entity.TelPhone,
            LowestPrice = entity.LowestPrice,
        };
    }

    public async Task<PageData<IEnumerable<HotelPublishPageListDto>>> GetMyHotelPublishPageListAsync(HotelPublishPageFilterQry qry)
    {
        string CountryIosCode = "";
        if (qry.countryId!=null) {
            CountryIosCode = SqlSugarClient.Queryable<BAreaEntity>().Single(vv => vv.Id == qry.countryId).CountryIosCode??"";
        }

        RefAsync<int> total = 0;

        var query = HotelPublishRepo.AsQueryable()
              .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelName), x => SqlFunc.Like(x.HotelName, $"{qry.HotelName}%") || SqlFunc.Like(x.HotelNameEn, $"{qry.HotelName}%"))
              .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelCode), x => x.HotelCode == qry.HotelCode)
              .WhereIF(!string.IsNullOrEmpty(CountryIosCode) , x=> x.CountryIosCode == CountryIosCode)
              .WhereIF(!string.IsNullOrEmpty(qry.cityName) , x=> x.CityName.Contains(qry.cityName) )
              .WhereIF(qry.Status.HasValue, x => x.Status == qry.Status);
        var queryMap = WhereDeptFilter(query)
                            .LeftJoin<SysUserDo>((x1,x2)=> x1.CreatedbyId == SqlFunc.ToString(x2.Id) && x1.TenantId == x2.TenantId  )
                            .Select((x1,x2) => new HotelPublishPageListDto()
        {
            Id = x1.Id,
            HotelCode = x1.HotelCode,
            HotelName = x1.HotelName,
            HotelNameEn = x1.HotelNameEn,
            Address = x1.Address,
            AddressEn = x1.AddressEn,
            Status = x1.Status,
            CreateTime = x1.CreateTime,
            LowestPrice = x1.LowestPrice,
            TelPhone = x1.TelPhone,
            CountryIosCode = x1.CountryIosCode,
            CountryName = x1.CountryName,
            CityName = x1.CityName,
            RealName = x2.RealName,
        }).OrderByDescending(x1 => x1.Id);
        var data = await queryMap.ToPageListAsync(qry.PageIndex, qry.PageSize, total);
        return new PageData<IEnumerable<HotelPublishPageListDto>>(total, qry.PageSize, qry.PageIndex, data);
    }


    private ISugarQueryable<HotelPublishDo> WhereDeptFilter(ISugarQueryable<HotelPublishDo> query)
    {
        if (new List<AccountTypeEnum>() { AccountTypeEnum.SysAdmin, AccountTypeEnum.SuperAdmin }.ToList().Contains(CurrentUserApp.AccountType!.Value))
        {
            return query;
        }
        if (CurrentUserApp.IsDeptAdmin)
        {
            var deptUserIds = SysUserRepo.GetList(x => x.DeptId == CurrentUserApp.Dept.DeptId)
                .Select(x => x.Id.ToString())
                .ToList();
            deptUserIds.Insert(0, CurrentUserApp.Id.ToString());
            return query.Where(x => deptUserIds.Contains(x.CreatedbyId!));
        }
        return query.Where(x => x.CreatedbyId == CurrentUserApp.Id);
    }



    public async Task<bool> PublishHotelAsync(CreateOrUpHotelPublishCmd cmd)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpHotelPublishCmd>>().ValidateAndThrowAsync(cmd);
        if (await HotelPublishRepo.IsAnyAsync(x => x.HotelCode == cmd.HotelCode && x.CreatedbyId == CurrentUserApp.Id))
        {
            throw new InvalidOperationException($"当前酒店已存在");
        }
        var entity = HotelPublishDo.Create(cmd.HotelCode,
                                           cmd.HotelName,
                                           cmd.HotelNameEn,
                                           HotelSaleTypeEnum.Down,
                                           cmd.Address,
                                           cmd.AddressEn,
                                           cmd.TelPhone,
                                           cmd.LowestPrice);
        entity.CountryIosCode = cmd.CountryIosCode;
        entity.CountryName = cmd.CountryName;
        entity.CityName = cmd.AreaName;
        await HotelPublishRepo.InsertReturnSnowflakeIdAsync(entity);
        return true;
    }

    public async Task<bool> UpdatePublishHotelAsync(CreateOrUpHotelPublishCmd cmd, long id)
    {
        var entity = await HotelPublishRepo.GetByIdAsync(id);
        if (entity==null) {
            throw new InvalidOperationException($"资源不存在！");
        }
        entity.SetStatus(cmd.Status).SetLowestPrice(cmd.LowestPrice);
        await HotelPublishRepo.AsUpdateable(entity).ExecuteCommandAsync();
        return true;
    } 
}
