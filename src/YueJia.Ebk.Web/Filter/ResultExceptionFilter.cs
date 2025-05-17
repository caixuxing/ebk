using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http;
using YueJia.Ebk.Domain.Shared.Response;

namespace YueJia.Ebk.Web.Filter
{
    /// <summary>
    /// 全局异常统一格式返回
    /// </summary>
    public class ResultExceptionFilter : IAsyncExceptionFilter, ITransientDependency
    {
        private ILogger<ResultExceptionFilter> Logger;
        public ResultExceptionFilter(
            ILogger<ResultExceptionFilter> logger)
        {
            Logger = NullLogger<ResultExceptionFilter>.Instance;
            Logger = logger;
        }
        public async Task OnExceptionAsync(ExceptionContext context)
        {

            await HandleAndWrapException(context);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual async Task HandleAndWrapException(ExceptionContext context)
        {

            string errorMsg = string.Empty;
            if (context.Exception is YueJiaBusinessException yueJiaBusinessException)
            {
                errorMsg = yueJiaBusinessException.Message;
                context.HttpContext.Response.StatusCode = 400;
            }
            else if (context.Exception is InvalidOperationException invalidOperationException)
            {
                errorMsg = invalidOperationException.Message;
                context.HttpContext.Response.StatusCode = 400;
            }
            else
            {
                errorMsg = "服务器好像出了点问题，请联系系统管理员...";
                context.HttpContext.Response.StatusCode = 500;
            }
            var apiResult = ServiceResult<string>.Failure(errorMsg);
            var remoteServiceErrorInfoBuilder = new StringBuilder();
            remoteServiceErrorInfoBuilder.AppendLine($"---------- {nameof(RemoteServiceErrorInfo)} ----------");
            remoteServiceErrorInfoBuilder.AppendLine(apiResult.ToString());
            var logLevel = context.Exception.GetLogLevel();
            Logger.LogWithLevel(logLevel, remoteServiceErrorInfoBuilder.ToString());
            Logger.LogWithLevel(logLevel, "---------- 错误明细 ----------");
            Logger.LogException(context.Exception, logLevel);
            context.ExceptionHandled = true;
            context.HttpContext.Response.ContentType = "text/json;charset=utf-8";
            await context.HttpContext.Response.WriteAsJsonAsync(apiResult);
        }

    }
}
