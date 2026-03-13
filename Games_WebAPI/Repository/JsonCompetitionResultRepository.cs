using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Games_WebAPI.Entities;

namespace Games_WebAPI.Repository;

public class JsonCompetitionResultRepository : ICompetitionResultRepository
{
    private readonly string _filePath;

    private List<CompetitionResult> _results = [];

    public JsonCompetitionResultRepository(string filePath)
    {
        _filePath = filePath;
        _results = LoadFromJson();
    }


    private List<CompetitionResult> LoadFromJson()
    {
        if (!File.Exists(_filePath)) return [];

        var json = File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<List<CompetitionResult>>(json) ?? [];
    }


    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };


    private void SaveToJson()
    {
        var json = JsonSerializer.Serialize(_results, Options);

        File.WriteAllText(_filePath, json);
    }

    public IEnumerable<CompetitionResult> GetAll() => _results;

    public CompetitionResult GetResultById(Guid id)
    {
        return _results.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<CompetitionResult> GetAllResultsConcreteCompetition(Guid id)
    {
        return _results.Where(r => r.CompetitionId == id);
    }

    public void Add(CompetitionResult result)
    {
        if (_results.Any(r => r.Id == result.Id))
            throw new ArgumentException("Такой результат уже существует");

        _results.Add(result);
        SaveToJson();
    }

    public void Update(CompetitionResult result)
    {
        var index = _results.FindIndex(r => r.Id == result.Id);
        if (index < 0) throw new ArgumentException("Результаты не найдены");

        _results[index] = result;
        SaveToJson();
    }

    public void Delete(Guid id)
    {
        var removed = _results.RemoveAll(r => r.Id == id);
        if (removed > 0)
            SaveToJson();
    }
}