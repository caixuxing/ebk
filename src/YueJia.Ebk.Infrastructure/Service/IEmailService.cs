namespace YueJia.Ebk.Infrastructure.Service
{
    public interface IEmailService
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns></returns>
        Task<bool> DoSendAsync(string to, string subject, string htmlContent);
    }
}
