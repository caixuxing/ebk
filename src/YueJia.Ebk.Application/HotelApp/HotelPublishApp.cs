using YueJia.Ebk.Application.Contracts.HotelApp;
using YueJia.Ebk.Application.Contracts.HotelApp.Commands;
using YueJia.Ebk.Application.Contracts.HotelApp.Dto;
using YueJia.Ebk.Application.Contracts.HotelApp.Query;
using YueJia.Ebk.Application.Contracts.SysUserApp;
using YueJia.Ebk.Domain.Hotel;
using YueJia.Ebk.Domain.Shared.Enums;

namespace YueJia.Ebk.Application.HotelApp;

[DisableValidation]
public class HotelPublishApp : ApplicationService, IHotelPublishApp
{

    private ISimpleClient<HotelPublishDo> HotelPublishRepo => LazyServiceProvider.LazyGetRequiredService<ISimpleClient<HotelPublishDo>>();

    private ISqlSugarClient db => LazyServiceProvider.LazyGetRequiredService<ISqlSugarClient>();


    private ICurrentUserApp CurrentUserApp => LazyServiceProvider.LazyGetRequiredService<ICurrentUserApp>();

    public async Task<HotelPublishDetailDto> GetHotelPublishDetailAsync(long id)
    {
        var entity = await HotelPublishRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"酒店ID:{id}资源不存在！");
        return new HotelPublishDetailDto()
        {
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
        RefAsync<int> total = 0;

        var query = HotelPublishRepo.AsQueryable()
              .WhereIF(CurrentUserApp.AccountType == AccountTypeEnum.NormalUser, x => x.CreatedbyId == CurrentUserApp.Id)
              .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelName), x => SqlFunc.Like(x.HotelName, $"{qry.HotelName}%") || SqlFunc.Like(x.HotelNameEn, $"{qry.HotelName}%"))
              .WhereIF(!string.IsNullOrWhiteSpace(qry.HotelCode), x => x.HotelCode == qry.HotelCode)
              .WhereIF(qry.Status.HasValue, x => x.Status == qry.Status)
              .Select(x => new HotelPublishPageListDto()
              {
                  Id = x.Id,
                  HotelCode = x.HotelCode,
                  HotelName = x.HotelName,
                  HotelNameEn = x.HotelNameEn,
                  Address = x.Address,
                  AddressEn = x.AddressEn,
                  Status = x.Status,
                  CreateTime = x.CreateTime,
                  LowestPrice = x.LowestPrice,
                  TelPhone = x.TelPhone
              });

        var data = await query.ToPageListAsync(qry.PageIndex, qry.PageSize, total);

        return new PageData<IEnumerable<HotelPublishPageListDto>>(total, qry.PageSize, qry.PageIndex, data);
    }

    public async Task<long> PublishHotelAsync(CreateOrUpHotelPublishCmd cmd)
    {
        await LazyServiceProvider.LazyGetRequiredService<FluentValidation.IValidator<CreateOrUpHotelPublishCmd>>().ValidateAndThrowAsync(cmd);

        if (await HotelPublishRepo.IsAnyAsync(x => x.HotelCode == cmd.HotelCode && x.CreatedbyId == CurrentUserApp.Id)) throw new InvalidOperationException($"酒店【{cmd.HotelName}({cmd.HotelCode})】已存在,无须重复添加！");

        var entity = HotelPublishDo.Create(cmd.HotelCode, cmd.HotelName, cmd.HotelNameEn, HotelSaleTypeMnum.Stop, cmd.Address, cmd.AddressEn, cmd.TelPhone, cmd.LowestPrice) ?? throw new InvalidOperationException("创建酒店失败！");

        return await HotelPublishRepo.InsertReturnSnowflakeIdAsync(entity);
    }

    public async Task<bool> UpdatePublishHotelAsync(CreateOrUpHotelPublishCmd cmd, long id)
    {
        var entity = await HotelPublishRepo.GetByIdAsync(id) ?? throw new InvalidOperationException($"酒店ID:{id}资源不存在！");
        entity.SetStatus(cmd.Status).SetLowestPrice(cmd.LowestPrice);
        return await HotelPublishRepo.UpdateAsync(entity);
    }
}
