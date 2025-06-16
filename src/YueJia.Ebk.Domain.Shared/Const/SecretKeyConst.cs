using System.Text;

namespace YueJia.Ebk.Domain.Shared.Const
{
    /// <summary>
    /// 加密密钥常量
    /// </summary>
    public class SecretKeyConst
    {
        /// <summary>
        /// 加密密钥
        /// </summary>
        public static byte[] key = Encoding.UTF8.GetBytes("woxsfkwlslawlxlsoewlawlqxledadre");

        /// <summary>
        /// 加密向量
        /// </summary>
        public static byte[] iv = Encoding.UTF8.GetBytes("qwedsdewhtyibgfd");
    }
}
