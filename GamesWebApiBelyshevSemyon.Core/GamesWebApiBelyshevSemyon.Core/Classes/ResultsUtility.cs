using System.Text.Json;

namespace GamesWebApiBelyshevSemyon.Core.Classes;

public class ResultsUtility
{
    private const string JsonResultsPath = "Files\\results.json";
    private static readonly string JsonResults = File.ReadAllText(JsonResultsPath);
    private static List<Result>? _results = JsonSerializer.Deserialize<List<Result>>(JsonResults);

    public static Result GetById(Guid id)
    {
        var foundResult = _results!.FirstOrDefault(result => result.Id == id);

        if (Utility.IsNull(foundResult)) Results.NotFound("Не найден результат");
        
        return foundResult!;
    }

    public static List<Result> GetByCompetitionId(Guid id)
    {
        var foundResults = _results!.Where(result => result.CompetitionId == id).ToList();

        if (foundResults.Count == 0) Results.NotFound("Не найдены результаты соревнования");
        
        return foundResults;
    }
}