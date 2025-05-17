using System.Security.Cryptography;
using System.Text;

namespace YueJia.Ebk.Domain.Shared.Utils
{
    public static class EncryptUtils
    {
        /// <summary>
        /// MD5加密 指定编码格式= Encoding.Default
        /// 将字符串转换成32位小写md5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string str)
        {
            using MD5 md5 = MD5.Create();
            StringBuilder sBuilder = new StringBuilder();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(str));
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
