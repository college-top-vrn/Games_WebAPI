using GamesWebAPI;

var competitions = StorageManager.ReadFromFile<Competition>("competitions.json").ToList();
var results = StorageManager.ReadFromFile<Result>("results.json").ToList();

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

app.UseHttpsRedirection();

app.Lifetime.ApplicationStopping.Register(() =>
{
    StorageManager.WriteToFile(competitions, "competitions.json");
    StorageManager.WriteToFile(results, "results.json");
});


// Competitions endpoints

const string competitionsUrl = $"/api/{version}/competitions";

var apiCompetitions = app.MapGroup(competitionsUrl)
    .WithTags("Competitions");

apiCompetitions.MapGet("/", () =>
{
    var notDeleted = competitions.Where(s => !s.IsDeleted).ToList();
    return notDeleted.Count == 0
        ? Results.NoContent()
        : Results.Ok(notDeleted);
})
    .WithName("GetCompetitions")
    .Produces(StatusCodes.Status204NoContent)
    .Produces<List<Competition>>();

apiCompetitions.MapGet("/{id:guid}", (Guid id) =>
    {
        return competitions.FindEntity(id).Finally(foundCompetition => { return Results.Ok(foundCompetition); });
    })
    .WithName("Get a single competition by id")
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status410Gone)
    .Produces<Competition>();

apiCompetitions.MapGet("/{id:guid}/results", (Guid id) =>
    {
        var competitionSearch = competitions.FindEntity(id);
        if (competitionSearch.Error != null)
            return competitionSearch.Error;
        
        var resultSearch = results.SingleOrDefault(s => s.CompetitionId == competitionSearch.Entity!.Id);
        if (resultSearch == null)
            return Results.NotFound("Result not found");
        if (resultSearch.IsDeleted)
        {
            return Results.StatusCode(410);
        }

        return Results.Ok(resultSearch);
    })
    .WithDescription("Get a single result by competition id")
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status410Gone)
    .Produces<Competition[]>();

apiCompetitions.MapPost("/", (Competition competition) =>
    {
        if (competitions.SingleOrDefault(s => s.Id == competition.Id) != null)
        {
            return Results.BadRequest("Competition with this id already exists");
        }

        var newCompetition = new Competition()
        {
            Name = competition.Name,
            Date = competition.Date,
            Location = competition.Location,
            SportType = competition.SportType
        };
        competitions.Add(newCompetition);
        return Results.Created($"{competitionsUrl}/{newCompetition.Id}", newCompetition);
    })
    .WithName("CreateANewCompetition")
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status201Created);

apiCompetitions.MapDelete("/{id:guid}", (Guid id) =>
    {
        return competitions.FindEntity(id).Finally(foundCompetition =>
        {
            foundCompetition.IsDeleted = true;
            
            return Results.Ok(foundCompetition);
        });
    })
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status410Gone)
    .Produces(StatusCodes.Status200OK)
    .WithName("Delete a competition");

apiCompetitions.MapPut("/{id:guid}", (Guid id, Competition competition) =>
    {
        return competitions.FindEntity(id).Finally(foundCompetition =>
        {
            foundCompetition.Name = competition.Name;
            foundCompetition.Date = competition.Date;
            foundCompetition.Location = competition.Location;
            foundCompetition.SportType = competition.SportType;
            
            return Results.Ok(foundCompetition);
        });
    })
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status410Gone)
    .Produces(StatusCodes.Status200OK)
    .WithName("Edit a competition.");

// Result endpoints

const string resultsUrl = $"/api/{version}/results";
var apiResults = app.MapGroup(resultsUrl)
    .WithTags("Results");

apiResults.MapGet("/", () =>
{
    var notDeleted = results.Where(s => !s.IsDeleted).ToList();
    return notDeleted.Count == 0
        ? Results.NoContent()
        : Results.Ok(notDeleted);
})
    .WithName("Get all results.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces<List<Result>>();

apiResults.MapGet("/{id:guid}", (Guid id) =>
    {
        return results.FindEntity(id).Finally(foundResult => { return Results.Ok(foundResult);});
    })
    .WithName("Get a single result by id.")
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status410Gone)
    .Produces<Result>();

apiResults.MapPost("/", (Result result) =>
    {
        if (results.SingleOrDefault(s => s.Id == result.Id) != null)
        {
            return Results.BadRequest("Result with this id already exists.");
        }
        
        if (competitions.SingleOrDefault(s => s.Id == result.CompetitionId) == null)
        {
            return Results.BadRequest("Cannot create a new result. No competition with such id found.");
        }

        var newResult = new Result()
        {
            CompetitionId = result.CompetitionId,
            ParticipantName = result.ParticipantName,
            Place = result.Place,
            Score = result.Score
        };
        results.Add(newResult);
        return Results.Created($"{resultsUrl}/{newResult.Id}", newResult);
    })
    .WithName("Create a new result.")
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError)
    .Produces(StatusCodes.Status201Created);

apiResults.MapDelete("/{id:guid}", (Guid id) =>
    {
        return results.FindEntity(id).Finally(foundResult =>
        {
            foundResult.IsDeleted = true;
            
            return Results.Ok();
        });
    })
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status410Gone)
    .Produces(StatusCodes.Status200OK)
    .WithName("Delete a result.");

apiResults.MapPut("/{id:guid}", (Guid id, Result result) =>
    {
        return results.FindEntity(id).Finally(foundResult =>
        {
            foundResult.CompetitionId = result.CompetitionId;
            foundResult.ParticipantName = result.ParticipantName;
            foundResult.Place = result.Place;
            foundResult.Score = result.Score;
            
            return Results.Ok(foundResult);
        });
    })
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status410Gone)
    .Produces(StatusCodes.Status200OK)
    .WithName("EditAResult");


app.Run();
