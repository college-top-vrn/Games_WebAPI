using System;

namespace Games_WebAPI.Entities;

public record Competition
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateTime Date { get; init; }
    public required string Location { get; init; }
    public required string SportType { get; init; }
    
}