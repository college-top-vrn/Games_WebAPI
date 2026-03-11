using System;
using System.Collections.Generic;
using System.Linq;
using CompetitionWebAPI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

const string version = "v1";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"/openapi/{version}.json", version);
    });
}

var competitions = Competition.ReadFromFile("competitions.json");
var results = Result.ReadFromFile("results.json");

app.UseHttpsRedirection();

// Сompetition
const string urlСompetition = $"/api/competitions";

var apiCompetition = app.MapGroup(urlСompetition)
    .WithTags("Competitive games");

apiCompetition.MapGet("/", () => 
        competitions.Where(c => !c.ItDeleted).Any()
            ? Results.Ok(competitions.Where(c => !c.ItDeleted).ToList())
            : Results.NoContent())
    .Produces<List<Competition>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent);

apiCompetition.MapGet("/{id:int}", (int id) =>
    {
        var comp = competitions.FirstOrDefault(c => c.Id == id && !c.ItDeleted);
        return comp is null ? Results.NotFound() : Results.Ok(comp);
    })
    .Produces<Competition>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("Get Competition by id");

apiCompetition.MapPost("/", (Competition newCompetition) =>
{
    if (competitions.Any(c => c.Id == newCompetition.Id))
    {
        return Results.BadRequest("Соревнование с таким ID уже существует.");
    }
    
    competitions.Add(newCompetition);
    Competition.SaveToFile(competitions, "competitions.json");
    
    return Results.Created($"/api/competitions/{newCompetition.Id}", newCompetition);
}).Produces<Competition>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("Create Competition");

apiCompetition.MapPut("/{id:int}", (int id, Competition updatedCompetition) =>
    {
        var index = competitions.FindIndex(c => c.Id == id);
        if (index == -1) 
            return Results.NotFound();

        if (string.IsNullOrWhiteSpace(updatedCompetition.Name) ||
            string.IsNullOrWhiteSpace(updatedCompetition.Location) ||
            string.IsNullOrWhiteSpace(updatedCompetition.SportType) ||
            updatedCompetition.Date == DateTime.MinValue)
            return Results.BadRequest("Name, Location, SportType и Date обязательны.");
        
        competitions[index] = new Competition
        {
            Id = id,                                   
            Name = updatedCompetition.Name,
            Date = updatedCompetition.Date,
            Location = updatedCompetition.Location,
            SportType = updatedCompetition.SportType
        };
    
        Competition.SaveToFile(competitions, "competitions.json");
        return Results.Ok(competitions[index]);
    })
    .Produces<Competition>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("UpdateCompetition");

apiCompetition.MapDelete("/{id:int}", (int id) =>
    {
        var existingComp = competitions.FirstOrDefault(c => c.Id == id);
        if (existingComp == null) return Results.NotFound();
        if (existingComp.ItDeleted) return Results.StatusCode(410); 
    
        existingComp.ItDeleted = true;
        Competition.SaveToFile(competitions, "competitions.json");
        return Results.Ok(existingComp);
    })
    .Produces<Competition>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status410Gone)
    .WithName("Soft Delete Competition");

// Results
const string urlResults = "/api/results";

var apiResults = app.MapGroup(urlResults)
    .WithTags("Results");

apiResults.MapGet("/", () => 
        results.Where(r => !r.ItDeleted).Any()
            ? Results.Ok(results.Where(r => !r.ItDeleted).ToList())
            : Results.NoContent())
    .Produces<List<Result>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent);

apiResults.MapGet("/{id:int}", (int id) =>
    {
        var result = results.FirstOrDefault(r => r.Id == id && !r.ItDeleted);
        return result is null ? Results.NotFound() : Results.Ok(result);
    })
    .Produces<Result>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("Get Result by id");

apiResults.MapGet("/{competitionId:int}/results", (int competitionId) =>
    {
        var competitionResults = results
            .Where(r => r.CompetitionId == competitionId && !r.ItDeleted)
            .ToList();
        return competitionResults.Any() ? Results.Ok(competitionResults) : Results.NoContent();
    })
    .Produces<List<Result>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent);

apiResults.MapPost("/", (Result newResult) =>
    {
        if (string.IsNullOrWhiteSpace(newResult.ParticipantName))
            return Results.BadRequest("ParticipantName не должен быть пустым.");
    
        if (newResult.Place <= 0)
            return Results.BadRequest("Place должен быть больше 0.");
    
        if (newResult.CompetitionId <= 0)
            return Results.BadRequest("CompetitionId должен быть больше 0.");

        var competitionExists = competitions.Any(c => c.Id == newResult.CompetitionId);
        if (!competitionExists)
            return Results.BadRequest("Соревнование с таким CompetitionId не существует.");

        if (results.Any(r => r.Id == newResult.Id))
            return Results.BadRequest("Результат с таким ID уже существует.");
        
        results.Add(newResult);
        Result.SaveToFile(results, "results.json");
    
        return Results.Created($"/api/results/{newResult.Id}", newResult);
    })
    .Produces<Result>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("Create Result");

apiResults.MapPut("/{id:int}", (int id, Result updatedResult) =>
    {
        var index = results.FindIndex(r => r.Id == id);
        if (index == -1) return Results.NotFound();

        if (string.IsNullOrWhiteSpace(updatedResult.ParticipantName) ||
            updatedResult.Place <= 0 ||
            !competitions.Any(c => c.Id == updatedResult.CompetitionId))
            return Results.BadRequest();
        
        results[index] = new Result
        {
            Id = id,
            CompetitionId = updatedResult.CompetitionId,
            ParticipantName = updatedResult.ParticipantName,
            Place = updatedResult.Place,
            Score = updatedResult.Score
        };
    
        Result.SaveToFile(results, "results.json");
        return Results.Ok(results[index]);
    })
    .Produces<Result>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound) 
    .WithName("Update Result");

apiResults.MapDelete("/{id:int}", (int id) =>
    {
        var existingResult = results.FirstOrDefault(r => r.Id == id);
        if (existingResult == null) return Results.NotFound();
        if (existingResult.ItDeleted) return Results.StatusCode(410); 
    
        existingResult.ItDeleted = true;
        Result.SaveToFile(results, "results.json");
        return Results.Ok(existingResult);
    })
    .Produces<Result>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status410Gone)
    .WithName("SoftDelete Result");

app.Run();