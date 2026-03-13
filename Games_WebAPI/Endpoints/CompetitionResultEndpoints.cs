using System;
using Games_WebAPI.Entities;
using Games_WebAPI.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Games_WebAPI.Endpoints;

public static class CompetitionResultEndpoints
{
    public static RouteGroupBuilder MapResultEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Results");

        group.MapGet("/results", (ICompetitionResultRepository repo) => Results.Ok(repo.GetAll()));

        group.MapGet("/results/{id:guid}", (Guid id, ICompetitionResultRepository repo) =>
        {
            var result = repo.GetResultById(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapGet("/competitions/{competitionId}/results", (Guid competitionId, ICompetitionResultRepository repo) =>
        {
            Results.Ok(repo.GetAllResultsConcreteCompetition(competitionId)); 
        });

        group.MapPost("/results", (CompetitionResult result, ICompetitionResultRepository repo) =>
        {
            repo.Add(result);
            return Results.Created($"/api/results/{result.Id}", result);
        });

        group.MapPut("/result/{id:guid}", (Guid id, CompetitionResult result, ICompetitionResultRepository repo) =>
        {
            if (result.Id != id) return Results.BadRequest(); 
            repo.Update(result);
            return Results.NoContent();
        });


        group.MapDelete("/results/{id:guid}", (Guid id, ICompetitionResultRepository repo) =>
        {
            repo.Delete(id);
            return Results.NoContent(); 
        });


        return group;
    }
}