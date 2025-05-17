using System.ComponentModel;
using System.Reflection;

namespace YueJia.Ebk.Domain.Shared.Utils
{
    public static class EnumExtend
    {
        /// <summary>
        /// 根据值转换成枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        //public static T ToEnum<T>(this string name)
        //{
        //    return (T)Enum.Parse(typeof(T), name);
        //}

        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="en">枚举</param>
        /// <returns>返回枚举的描述</returns>
        public static string ToDescription(this System.Enum en)
        {
            Type type = en.GetType();   //获取类型
            MemberInfo[] memberInfos = type.GetMember(en.ToString());   //获取成员
            if (memberInfos != null && memberInfos.Length > 0)
            {
                DescriptionAttribute[]? attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];   //获取描述特性

                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0].Description;    //返回当前描述
                }
            }
            return en.ToString();
        }


        /// <summary>
        /// 获取枚举的Description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDescription(this object value)
        {
            return value.GetType().GetMember(value.ToString() ?? string.Empty).FirstOrDefault()
                ?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
        }


    }
}
