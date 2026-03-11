using System.Text.Json;

namespace GamesWebApiBelyshevSemyon.Core.Classes;

public static class CompetitionsUtility
{
    private const string JsonCompetitionsPath = "Files\\competitions.json";
    private static readonly string JsonCompetitions = File.ReadAllText(JsonCompetitionsPath);
    private static List<Competition>? _competitions = JsonSerializer.Deserialize<List<Competition>>(JsonCompetitions);

    public static Competition GetById(Guid id)
    {
         var foundCompetition = _competitions!.FirstOrDefault(competition => competition.Id == id);

        if (Utility.IsNull(foundCompetition)) Results.NotFound("Не найдено соревнование");

        return foundCompetition!;
    }

    public static IResult Add(Competition competition)
    {
        if (Utility.IsEmpty(competition.Name)) return Results.BadRequest("Отсутствует имя");
        if (competition.Date == DateTime.MinValue) return Results.BadRequest("Отсутствует дата");
        if (Utility.IsEmpty(competition.Location)) return Results.BadRequest("Отсутствует локация");
        if (Utility.IsEmpty(competition.SportType)) return Results.BadRequest("Отсутствует вид спорта");

        var newCompetition = competition with { Id = Guid.NewGuid(), IsDeleted = false };

        _competitions!.Add(newCompetition);

        Utility.UpdateDataFile(JsonCompetitionsPath, _competitions);

        return Results.Created();
    }

    public static IResult UpdateWithNew(Guid id, Competition competition)
    {
        var foundCompetition = _competitions!.FirstOrDefault(existingCompetition => existingCompetition.Id == id);

        if (Utility.IsNull(foundCompetition)) return Results.NotFound();

        _competitions!.RemoveAll(existingCompetition => existingCompetition.Id == id);

        var newName = Utility.IsEmpty(competition.Name) ? foundCompetition!.Name : competition.Name;
        var newDate = competition.Date == DateTime.MinValue ? foundCompetition!.Date : competition.Date;
        var newLocation = Utility.IsEmpty(competition.Location) ? foundCompetition!.Location : competition.Location;
        var newSportType = Utility.IsEmpty(competition.SportType) ? foundCompetition!.SportType : competition.SportType;

        var updatedCompetition = competition with
        {
            Name = newName,
            Date = newDate,
            Location = newLocation,
            SportType = newSportType
        };

        _competitions.Add(updatedCompetition);

        Utility.UpdateDataFile(JsonCompetitionsPath, _competitions);

        return Results.Created();
    }

    public static IResult DeleteById(Guid id)
    {
        var foundCompetition = _competitions!.FirstOrDefault(competition => competition.Id == id);

        if (Utility.IsNull(foundCompetition)) return Results.NotFound("Не найдено соревнование");

        _competitions!.RemoveAll(existingCompetition => existingCompetition.Id == id);

        _competitions.Add(foundCompetition! with { IsDeleted = true });

        Utility.UpdateDataFile(JsonCompetitionsPath, _competitions);

        return Results.Ok();
    }
}