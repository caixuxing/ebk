using YueJia.Ebk.Application.Contracts.SysApp;
using YueJia.Ebk.Application.Contracts.SysApp.Dto;
using YueJia.Ebk.Domain.Shared;
using YueJia.Ebk.Domain.Shared.Utils;

namespace YueJia.Ebk.Application.SysApp;

public class SysEnumApp : ApplicationService, ISysEnumApp
{

    /// <summary>
    /// 通过枚举类型获取枚举值集合
    /// </summary>
    /// <param name="enumName"></param>
    /// <returns></returns>
    public List<EnumDto> GetEnumDataList(string enumName) => App.EffectiveEnumTypes.FirstOrDefault(t => t.IsEnum && t.Name == enumName)?.ToEnumDtoList() ?? new();

}
/// <summary>
/// 映射扩展
/// </summary>
internal static class MapExten
{
    /// <summary>
    /// 将枚举转成枚举信息集合
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<EnumDto> ToEnumDtoList(this Type type)
    {
        if (!type.IsEnum) throw new ArgumentException("Type '" + type.Name + "' is not an enum.");
        var arr = System.Enum.GetNames(type);
        return arr.Select(sl =>
        {
            var item = System.Enum.Parse(type, sl);
            return new EnumDto
            {
                Name = item?.ToString() ?? string.Empty,
                Describe = item?.ToDescription() ?? string.Empty,
                Value = item?.GetHashCode() ?? 0
            };
        }).ToList();
    }
}
