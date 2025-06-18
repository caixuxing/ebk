using YueJia.Ebk.Domain.Shared.Dto;

namespace YueJia.Ebk.Domain.Shared.Utils
{
    public class JsonUtils
    {
        /// <summary>
        /// 解析查价唯一码
        /// </summary>
        /// <param name="searchCodeJsonStr"></param>
        /// <returns></returns>
        public static SearchCodeDto AnalysisSearchCode(string searchCodeJsonStr)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<SearchCodeDto>(searchCodeJsonStr) ?? new();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("查价唯一码解析失败！", ex);
            }

        }
    }
}
