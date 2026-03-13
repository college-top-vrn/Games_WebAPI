namespace GamesWebAPI;

public class Competition: IEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public required string Location { get; set; }
    public required string SportType { get; set; }
    public bool IsDeleted { get; set; }
}