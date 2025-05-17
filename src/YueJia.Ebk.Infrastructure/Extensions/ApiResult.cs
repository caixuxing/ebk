using Microsoft.AspNetCore.Http;
using YueJia.Ebk.Domain.Shared.Response;

namespace YueJia.Ebk.Infrastructure.Extensions
{
    public static class ApiResult
    {
        // 处理成功结果 
        public static IResult Success<T>(T data)
        {
            return Results.Ok(ServiceResult<T>.Success(data));
        }

        // 处理失败结果
        public static IResult Failure(string message, int statusCode = StatusCodes.Status400BadRequest)
        {
            return statusCode switch
            {
                StatusCodes.Status404NotFound => Results.NotFound(ServiceResult.Failure(message)),
                _ => Results.BadRequest(ServiceResult.Failure(message))
            };
        }

        // 处理可能为null的结果
        public static IResult HandleResult<T>(T result, string notFoundMessage = "资源不存在")
        {
            return result switch
            {
                null => Failure(notFoundMessage, StatusCodes.Status404NotFound),
                _ => Success(result)
            };
        }

        // 处理布尔结果 
        public static IResult HandleBoolResult(bool result, string failureMessage = "操作失败")
        {
            return result ? Success(result) : Failure(failureMessage);
        }

        public static IResult HandleLongResult(long result, string successUri = "", string failureMessage = "操作失败") => result switch
        {
            > 0 => Success(result),
            _ => Failure(failureMessage)
        };

    }
}
