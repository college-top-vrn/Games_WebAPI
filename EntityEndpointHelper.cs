using Microsoft.AspNetCore.Http;

namespace GamesWebAPI;

public static class EntityEndpointHelper
{
    public static SearchResult<T> FindEntity<T>(
        this IEnumerable<T> items, 
        Guid id) where T : IEntity
    {
        string entityName = typeof(T).Name;
        var item = items.SingleOrDefault(x => x.Id == id);
        
        if (item == null)
            return new SearchResult<T>(Results.NotFound($"{entityName} not found"), default);
            
        if (item.IsDeleted)
        {
            var goneResult = Results.StatusCode(410);
            
            return new SearchResult<T>(goneResult, default);
        }
        
        return new SearchResult<T>(null, item);
    }
}