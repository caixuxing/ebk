using System.Reflection;

namespace YueJia.Ebk.Domain.Shared
{
    public static class App
    {
        /// <summary>
        /// 有效枚举类型
        /// </summary>
        public static readonly List<Type> EffectiveEnumTypes;

        /// <summary>
        /// 构造函数
        /// </summary>
        static App()
        {
            // 获取有效的类型集合
            EffectiveEnumTypes = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "YueJia.Ebk.*.Shared.dll")
                .Select(x => Assembly.LoadFrom(x).GetTypes())
                .SelectMany(x => x)
                .ToList();

        }
    }
}
