namespace YueJia.Ebk.Application.Contracts.Comm.BaseObj;

public record SelectDataDto<T>
{
    public T Text { get; set; } = default!;

    public string Label { get; set; } = default!;
}
