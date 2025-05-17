using Microsoft.AspNetCore.Http;
using YueJia.Ebk.Domain.Shared.Response;

namespace YueJia.Ebk.Infrastructure.Extensions
{
	public static class R
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="result"></param>
		/// <returns></returns>
		public static async Task<IResult> Ok<T>(Task<ServiceResult<T>> result)
		{
			var data = await result;
			return data switch
			{
				{ IsSuccess: true } => Results.Ok(data),
				{ IsSuccess: false } => Results.BadRequest(data)
			};
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="result"></param>
		/// <returns></returns>
		public static IResult Ok<T>(ServiceResult<T> result)
		{

			return result switch
			{
				{ IsSuccess: true } => Results.Ok(result),
				{ IsSuccess: false } => Results.BadRequest(result)
			};
		}
	}
}
