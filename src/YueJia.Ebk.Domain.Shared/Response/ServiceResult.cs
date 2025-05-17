namespace YueJia.Ebk.Domain.Shared.Response;

public record ServiceResult
{
	/// <summary>
	/// 是否成功
	/// </summary>
	public bool IsSuccess { get; set; }
	/// <summary>
	/// 信息
	/// </summary>
	public string? Message { get; set; } = default!;

	public static ServiceResult Success()
	{
		return new ServiceResult
		{
			IsSuccess = true,
		};
	}
	public static ServiceResult Failure(string message)
	{
		return new ServiceResult
		{
			IsSuccess = false,
			Message = message
		};
	}

}

// 自定义结果类
public record ServiceResult<T> : ServiceResult
{

	/// <summary>
	/// 数据
	/// </summary>
	public T? Data { get; set; }

	public static ServiceResult<T> Success(T data)
	{
		return new ServiceResult<T>
		{
			IsSuccess = true,
			Data = data
		};
	}

	public new static ServiceResult<T> Failure(string message)
	{
		return new ServiceResult<T>
		{
			IsSuccess = false,
			Message = message
		};
	}
}
