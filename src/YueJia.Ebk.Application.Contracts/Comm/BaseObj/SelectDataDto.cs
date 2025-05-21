namespace YueJia.Ebk.Application.Contracts.Comm.BaseObj;

public record SelectDataDto<T>
{
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public T Value { get; set; } = default!;

    public string Label { get; set; } = default!;
}



public record TreeSelectDataDto<T> : SelectDataDto<T>
{


    public List<SelectDataDto<T>> Children { get; set; } = new();
}