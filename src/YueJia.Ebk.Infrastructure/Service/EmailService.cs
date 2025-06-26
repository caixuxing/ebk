using SqlSugar;
using System.Net;
using System.Net.Mail;
using Volo.Abp.DependencyInjection;

namespace YueJia.Ebk.Infrastructure.Service;

public class EmailService : IEmailService, ITransientDependency
{
    // 全局或静态变量
    private static readonly SmtpClient _smtpClient = new SmtpClient("smtp.exmail.qq.com")
    {
        Port = 587,
        Credentials = new NetworkCredential("develop@yegatrip.com", "p8gBTCuSg7jGjBJR"),
        EnableSsl = true,
    };


    public async Task<bool> DoSendAsync(string to, string subject, string htmlContent)
    {
        try
        {
            // 创建邮件消息对象
            MailMessage mail = new MailMessage();
            // 设置发件人邮箱地址
            mail.From = new MailAddress("develop@yegatrip.com", "YueJia");
            //string to = "11360847@qq.com";
            // 设置收件人邮箱地址，可以添加多个收件人
            to.Split(',').ToList().ForEach(item => mail.To.Add(item));
            // 设置邮件主题
            mail.Subject = subject;
            // 设置邮件正文
            mail.Body = htmlContent;
            mail.IsBodyHtml = true;
            //// 创建 SmtpClient 对象，指定 SMTP 服务器地址和端口
            //SmtpClient smtpClient = new SmtpClient();
            //smtpClient.Host = "smtp.exmail.qq.com"; // 根据服务器要求调整 SMTP 服务器地址
            //smtpClient.Port = 587; // 根据服务器要求调整端口
            //                       // 设置 SMTP 服务器的凭据
            //smtpClient.Credentials = new NetworkCredential("develop@yegatrip.com", "p8gBTCuSg7jGjBJR");
            //// 启用 SSL 加密
            //smtpClient.EnableSsl = true;
            // 发送邮件
            await _smtpClient.SendMailAsync(mail);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("邮件发送失败", ex);
        }
    }

    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
