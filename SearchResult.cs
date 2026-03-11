namespace GamesWebAPI;

public class SearchResult<T>(IResult? error, T? entity)
    where T : IEntity
{
    public IResult? Error { get; } = error;
    public T? Entity { get; } = entity;

    public IResult Finally(Func<T, IResult> onSuccess)
    {
        if (Error != null) return Error;
        
        return onSuccess(Entity!);
    }
}