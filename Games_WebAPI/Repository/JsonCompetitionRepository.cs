using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Games_WebAPI.Entities;

namespace Games_WebAPI.Repository;

public class JsonCompetitionRepository : ICompetitionRepository
{
    private readonly string _filePath;

    private List<Competition> _competitions = [];


    public JsonCompetitionRepository(string filePath)
    {
        _filePath = filePath;
        _competitions = LoadFromJson();
    }

    public IEnumerable<Competition> GetAll() => _competitions;


    public Competition GetById(Guid id)
    {
        return _competitions.FirstOrDefault(c => c.Id == id);
    }

    public void Add(Competition competition)
    {
        if (_competitions.Any(c => c.Id == competition.Id))
            throw new ArgumentException("Такое соревнование уже существует");

        _competitions.Add(competition);
        SaveToJson();
    }

    public void Update(Competition competition)
    {
        var index = _competitions.FindIndex(c => c.Id == competition.Id);
        if (index < 0) throw new ArgumentException("Соревнование не найдено");

        _competitions[index] = competition;
        SaveToJson();
    }

    public void Delete(Guid id)
    {
        var removed = _competitions.RemoveAll(c => c.Id == id);
        if (removed > 0) 
            SaveToJson();
    }


    private List<Competition> LoadFromJson()
    {
        if (!File.Exists(_filePath)) return [];

        var json = File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<List<Competition>>(json) ?? [];
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };


    private void SaveToJson()
    {
        var json = JsonSerializer.Serialize(_competitions, Options);

        File.WriteAllText(_filePath, json);
    }
}