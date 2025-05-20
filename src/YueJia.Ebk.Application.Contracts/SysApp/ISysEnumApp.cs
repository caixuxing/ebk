using YueJia.Ebk.Application.Contracts.SysApp.Dto;

namespace YueJia.Ebk.Application.Contracts.SysApp
{

    /// <summary>
    /// 枚举接口服务App
    /// </summary>
    public interface ISysEnumApp
    {
        /// <summary>
        /// 通过枚举类型获取枚举值集合
        /// </summary>
        /// <param name="enumName"></param>
        /// <returns></returns>
        List<EnumDto> GetEnumDataList(string enumName);
    }
}
