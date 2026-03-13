using Games_WebAPI.Endpoints;
using Games_WebAPI.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

builder.Services.AddSingleton<ICompetitionRepository>(sp => 
    new JsonCompetitionRepository("competitions.json"));

builder.Services.AddSingleton<ICompetitionResultRepository>(sp => 
    new JsonCompetitionResultRepository("results.json"));

var app = builder.Build();
const string version = "v1";

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => { options.SwaggerEndpoint($"/openapi/{version}.json", version); });
}

app.UseHttpsRedirection();

app.MapCompetitionEndpoints();
app.MapResultEndpoints();

app.Run();

