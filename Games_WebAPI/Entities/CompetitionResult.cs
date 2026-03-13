using System;

namespace Games_WebAPI.Entities;

public record CompetitionResult
{
    public required Guid Id { get; init; }
    public required Guid CompetitionId { get; init; }
    public required string ParticipantName { get; init; }
    public required uint Place { get; init; }
    public required decimal? Score { get; init; }
    
}