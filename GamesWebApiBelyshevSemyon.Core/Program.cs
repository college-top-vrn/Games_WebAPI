using WebAPI;
// ReSharper disable ConvertClosureToMethodGroup

// ReSharper disable UseSymbolAlias

// ReSharper disable RedundantArgumentDefaultValue

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

#region Методы соревнований

app.MapGet("/api/competitions", Competitions.GetCompetition)
    .Produces(StatusCodes.Status200OK);

app.MapGet("/api/competitions/{id:guid}",
        (Guid id) => Competitions.GetById(id)
        )
    .Produces<Competition>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/competitions", 
        (Competition competition) => Competitions.Add(competition)
        )
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status201Created);

app.MapPut("/api/competitions/{id:guid}",
        (Guid id, Competition competition) => Competitions.UpdateWithNew(id, competition)
        )
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status201Created);

app.MapDelete("/api/competitions/{id:guid}",
        (Guid id) => Competitions.DeleteById(id)
        )
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status200OK);

#endregion

#region Результаты

app.MapGet("/api/results", WebAPI.Results.GetResults)
    .Produces(StatusCodes.Status200OK);

app.MapGet("/api/results/{id:guid}",
        (Guid id) => WebAPI.Results.GetById(id)).Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/competitions/{competitionId:guid}/results",
        (Guid competitionId) => WebAPI.Results.GetByCompetitionId(competitionId)
        )
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/results", (Result result) => WebAPI.Results.Add(result))
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status201Created);

app.MapPut("/api/results/{id:guid}",
        (Guid id, Result result) => WebAPI.Results.UpdateWithNew(id, result)
        )
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status200OK);

app.MapDelete("/api/results/{id:guid}",
        (Guid id) => WebAPI.Results.DeleteById(id)
        )
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status200OK);

#endregion

app.Run();