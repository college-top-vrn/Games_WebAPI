namespace GamesWebAPI;

public class Result: IEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required Guid CompetitionId { get; set; }
    public required string ParticipantName { get; set; }
    public required int Place
    {
        get;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(Score),
                    "Place cannot be zero or negative.");

            field = value;
        }
    }
    public required decimal? Score { get; set; }
    public bool IsDeleted { get; set; }
}