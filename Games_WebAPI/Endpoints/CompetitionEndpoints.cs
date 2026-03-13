using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Games_WebAPI.Entities;
using Games_WebAPI.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Games_WebAPI.Endpoints;

public static class CompetitionEndpoints
{
    public static RouteGroupBuilder MapCompetitionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/competitions")
            .WithTags("Competitions");

        group.MapGet("/", (ICompetitionRepository repo) => Results.Ok(repo.GetAll()));

        group.MapGet("/{id:guid}", (Guid id, ICompetitionRepository repo) =>
        {
            var competition = repo.GetById(id);
            return competition is null ? Results.NotFound() : Results.Ok(competition);
        });

        group.MapPost("/", (Competition competition, ICompetitionRepository repo) =>
        {
            repo.Add(competition);
            return Results.Created($"/api/competitions/{competition.Id}", competition);
        });

        group.MapPut("/{id:guid}", (Guid id, Competition competition, ICompetitionRepository repo) =>
        {
            if (competition.Id != id) return Results.BadRequest();
            repo.Update(competition);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", (Guid id, ICompetitionRepository repo) =>
        {
            repo.Delete(id);
            return Results.NoContent();
        });


        return group;
    }
}