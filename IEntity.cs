namespace GamesWebAPI;

public interface IEntity
{
    Guid Id { get; }
    bool IsDeleted { get; set; }
}