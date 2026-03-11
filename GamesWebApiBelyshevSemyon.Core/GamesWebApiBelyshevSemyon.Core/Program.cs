using System.Text.Json;
using GamesWebApiBelyshevSemyon.Core;
using GamesWebApiBelyshevSemyon.Core.Classes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

const string version = "v1";

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint($"/openapi/{version}.json", version); });
}

app.UseHttpsRedirection();

#region Общие сущности

const string jsonCompetitionsPath = "Files\\competitions.json";
const string jsonResultsPath = "Files\\results.json";

var jsonCompetitions = File.ReadAllText(jsonCompetitionsPath);
var jsonResults = File.ReadAllText(jsonResultsPath);

var competitions = JsonSerializer.Deserialize<List<Competition>>(jsonCompetitions);
var results = JsonSerializer.Deserialize<List<Result>>(jsonResults);

#endregion

#region Методы соревнований

app.MapGet("/api/competitions", () => competitions);

app.MapGet("/api/competitions/{id:guid}", CompetitionsUtility.GetById).Produces<Competition>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/competitions", CompetitionsUtility.Add).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status201Created);

app.MapPut("/api/competitions/{id:guid}", CompetitionsUtility.UpdateWithNew).Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status201Created);

app.MapDelete("/api/competitions/{id:guid}", CompetitionsUtility.DeleteById).Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status200OK);

#endregion

#region Результаты

app.MapGet("/api/results", () => results);

app.MapGet("/api/results/{id:guid}", (Guid id) =>
{
    var foundResult = results!.FirstOrDefault(result => result.Id == id);

    if (Utility.IsNull(foundResult)) return foundResult;

    Results.NotFound("Не найден результат");
    return null;
}).Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/competitions/{competitionId:guid}/results", (Guid competitionId) =>
{
    var foundResults = results!.Where(result => result.CompetitionId == competitionId).ToList();

    if (foundResults.Count == 0) return foundResults;

    Results.NotFound("Не найдены результаты соревнования");
    return null;
}).Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/results", (Result result) =>
{
    if (competitions!.Any(competition => competition.Id != result.CompetitionId))
        return Results.BadRequest("Несуществующий идентификатор соревнования");
    if (Utility.IsEmpty(result.ParticipantName)) return Results.BadRequest("Отсутствует имя");
    if (result.Place <= 0) return Results.BadRequest("Место не больше нуля");
    if (result.Score <= 0) return Results.BadRequest("Очки не больше нуля");

    var newResult = new Result
    {
        Id = Guid.NewGuid(),
        CompetitionId = result.CompetitionId,
        ParticipantName = result.ParticipantName,
        Place = result.Place,
        Score = result.Score
    };

    results!.Add(newResult);

    Utility.UpdateDataFile(jsonResultsPath, results);

    return Results.Created();
}).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status201Created);

app.MapPut("/api/results/{id}", (Guid id, Result result) =>
{
    var foundResult = results!.FirstOrDefault(existingResult => existingResult.Id == id);

    if (Utility.IsNull(foundResult)) return Results.NotFound("Не найден результат");

    results!.RemoveAll(existingResult => existingResult.Id == id);

    var newCompetitionId =
        Utility.IsEmpty(result.CompetitionId.ToString()) ? foundResult!.CompetitionId : result.CompetitionId;
    var newParticipantName = Utility.IsEmpty(result.ParticipantName) ? foundResult!.ParticipantName : result.ParticipantName;
    var newPlace = result.Place > 0 ? result.Place : foundResult!.Place;
    var newScore = result.Score > 0 ? result.Score : foundResult!.Score;

    var updatedResult = result with
    {
        CompetitionId = newCompetitionId,
        ParticipantName = newParticipantName,
        Place = newPlace,
        Score = newScore
    };

    results.Add(updatedResult);

    Utility.UpdateDataFile(jsonResultsPath, results);

    return Results.Ok();
}).Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status200OK);

app.MapDelete("/api/results/{id:guid}", (Guid id) =>
{
    var foundResult = results!.FirstOrDefault(result => result.Id == id);

    if (Utility.IsNull(foundResult)) return Results.NotFound("Не найден результат");

    results!.RemoveAll(existingResults => existingResults.Id == id);

    results.Add(foundResult! with { IsDeleted = true });

    Utility.UpdateDataFile(jsonResultsPath, results);

    return Results.Ok();
}).Produces(StatusCodes.Status404NotFound).Produces(StatusCodes.Status200OK);

#endregion

app.Run();