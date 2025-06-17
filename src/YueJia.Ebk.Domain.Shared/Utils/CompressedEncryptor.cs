using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace YueJia.Ebk.Domain.Shared.Utils;

/// <summary>
/// 提供文本压缩和加密功能的工具类
/// 先压缩文本再加密，可以生成更短的密文
/// </summary>
public class CompressedEncryptor
{
    /// <summary>
    /// 加密并压缩文本
    /// </summary>
    /// <param name="plainText">要加密的原始文本</param>
    /// <param name="key">加密密钥(32字节，256位)</param>
    /// <param name="iv">初始化向量(16字节，128位)</param>
    /// <returns>Base64编码的加密结果</returns>
    public static (bool, string) Encrypt(string plainText, byte[] key, byte[] iv)
    {
        try
        {
            // 1. 将原始文本转换为UTF-8编码的字节数组
            byte[] compressed = Compress(Encoding.UTF8.GetBytes(plainText));

            // 2. 使用AES加密压缩后的数据
            byte[] encrypted = EncryptBytes(compressed, key, iv);

            // 3. 将加密结果转换为Base64字符串以便安全传输/存储
            return (true, Convert.ToBase64String(encrypted));
        }
        catch (Exception ex)
        {
            return (false, $"加密失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 解密并解压文本
    /// </summary>
    /// <param name="cipherText">Base64编码的加密文本</param>
    /// <param name="key">加密时使用的密钥(32字节)</param>
    /// <param name="iv">加密时使用的初始化向量(16字节)</param>
    /// <returns>解密后的原始文本</returns>
    public static (bool, string) Decrypt(string cipherText, byte[] key, byte[] iv)
    {
        try
        {
            // 1. 将Base64字符串转换回字节数组
            byte[] encrypted = Convert.FromBase64String(cipherText);

            // 2. 使用AES解密数据
            byte[] decrypted = DecryptBytes(encrypted, key, iv);

            // 3. 解压解密后的数据
            byte[] decompressed = Decompress(decrypted);

            // 4. 将字节数组转换回UTF-8字符串
            return (true, Encoding.UTF8.GetString(decompressed));
        }

        catch (Exception ex)
        {
            return (false, $"解密失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 使用GZip压缩字节数组
    /// </summary>
    /// <param name="data">要压缩的原始数据</param>
    /// <returns>压缩后的字节数组</returns>
    private static byte[] Compress(byte[] data)
    {
        using (var memoryStream = new MemoryStream())
        {
            // 使用GZipStream进行压缩，设置最优压缩级别
            using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
            {
                // 写入原始数据，GZipStream会自动压缩
                gzipStream.Write(data, 0, data.Length);
            }
            // 返回压缩后的内存流内容
            return memoryStream.ToArray();
        }
    }

    /// <summary>
    /// 解压GZip压缩的字节数组
    /// </summary>
    /// <param name="data">压缩后的字节数组</param>
    /// <returns>解压后的原始字节数组</returns>
    private static byte[] Decompress(byte[] data)
    {
        // 使用压缩数据创建内存流
        using (var memoryStream = new MemoryStream(data))
        // 准备输出流存储解压结果
        using (var outputStream = new MemoryStream())
        {
            // 使用GZipStream解压数据
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                // 将解压后的数据复制到输出流
                gzipStream.CopyTo(outputStream);
            }
            // 返回解压后的字节数组
            return outputStream.ToArray();
        }
    }

    /// <summary>
    /// 使用AES算法加密字节数组
    /// </summary>
    /// <param name="data">要加密的原始数据</param>
    /// <param name="key">加密密钥(32字节)</param>
    /// <param name="iv">初始化向量(16字节)</param>
    /// <returns>加密后的字节数组</returns>
    private static byte[] EncryptBytes(byte[] data, byte[] key, byte[] iv)
    {
        // 创建AES加密算法实例
        using (Aes aes = Aes.Create())
        {
            // 设置加密密钥和初始化向量
            aes.Key = key;
            aes.IV = iv;

            // 创建加密器
            using (var encryptor = aes.CreateEncryptor())
            // 准备内存流存储加密结果
            using (var ms = new MemoryStream())
            {
                // 创建加密流
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    // 写入原始数据，加密流会自动加密
                    cs.Write(data, 0, data.Length);
                    // 确保所有数据都被处理
                    cs.FlushFinalBlock();
                }
                // 返回加密后的字节数组
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// 使用AES算法解密字节数组
    /// </summary>
    /// <param name="data">要解密的加密数据</param>
    /// <param name="key">加密时使用的密钥(32字节)</param>
    /// <param name="iv">加密时使用的初始化向量(16字节)</param>
    /// <returns>解密后的原始字节数组</returns>
    private static byte[] DecryptBytes(byte[] data, byte[] key, byte[] iv)
    {
        // 创建AES加密算法实例
        using (Aes aes = Aes.Create())
        {
            // 设置解密密钥和初始化向量(必须与加密时相同)
            aes.Key = key;
            aes.IV = iv;

            // 创建解密器
            using (var decryptor = aes.CreateDecryptor())
            // 使用加密数据创建内存流
            using (var ms = new MemoryStream(data))
            // 创建解密流
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            // 准备输出流存储解密结果
            using (var output = new MemoryStream())
            {
                // 将解密后的数据复制到输出流
                cs.CopyTo(output);
                // 返回解密后的字节数组
                return output.ToArray();
            }
        }
    }
}
