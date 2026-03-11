using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;

namespace DefaultNamespace;

public class Result
{
    [property: Required]
    
    public int Id { get; set; } 
    
    [property: Required]
    
    public int CompetitionId { get; set; } 
    
    [property: Required]
    
    public string ParticipantName { get; set; } 
    
    [property: Required]
    
    public int Place { get; set; } 
    
    public decimal? Score { get; set; }

    [property: Required] public bool ItDeleted { get; set; } 

    public static List<Result>? ReadFromFile(string path)
    {
        if (!File.Exists(path)) return null;
        
        var json = File.ReadAllText(path);

        var result = JsonSerializer.Deserialize<List<Result>>(json);
        
        return result;
    }
    public static void SaveToFile(List<Result> results, string path)
    {
        var json = JsonSerializer.Serialize(results);
        File.WriteAllText(path, json);
    }
}