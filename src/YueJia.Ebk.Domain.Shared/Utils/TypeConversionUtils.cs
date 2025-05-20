namespace YueJia.Ebk.Domain.Shared.Utils;

public static class TypeConversionUtils
{
    /// <summary>
    /// 字符串转为long类型
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long ToLong(this string value, long defaultValue = 0)
    {
        return long.TryParse(value, out long result) ? result : defaultValue;
    }

}
