namespace YueJia.Ebk.Domain.Shared.Response
{
    /// <summary>
    /// YueJia业务异常
    /// </summary>
    public class YueJiaBusinessException : Exception
    {

        public new object? Data { get; set; }

        public new string Message { get; private set; } = default!;

        /// <summary>
        /// 义业务异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorInfo"></param>
        /// <param name="data"></param>
        /// <param name="httpStatusCode"></param>
        public YueJiaBusinessException(string message, object? data = null) : base(message)
        {
            this.Message = message;
            this.Data = data;
        }
    }
}
