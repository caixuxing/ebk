using YueJia.Ebk.Application.Contracts.HotelApp.Commands;

namespace YueJia.Ebk.Application.Contracts.HotelApp.Dto;

/// <summary>
/// 
/// </summary>
public record HotelPublishDetailDto : CreateOrUpHotelPublishCmd
{
    public string Id { get; set; }
}
