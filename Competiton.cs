using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;


namespace DefaultNamespace;

public class Competition
{
    [property: Required]

    public int Id { get; set; } 
    
    [property: Required]

    public string Name { get; set; }
    
    [property: Required]


    public DateTime Date { get; set; } 
    
    [property: Required]


    public string Location { get; set; } 
    
    [property: Required]


    public string SportType { get; set; } 
    [property: Required]
    
    public bool ItDeleted { get; set; } 

    public static List<Competition>? ReadFromFile(string path)
    {
        if (!File.Exists(path)) return null;
        
        var json = File.ReadAllText(path);

        var result = JsonSerializer.Deserialize<List<Competition>>(json);
        
        return result;
    }

    public static void SaveToFile(List<Competition> competitions, string path)
    {

        var json = JsonSerializer.Serialize(competitions);
        File.WriteAllText(path, json);
    }
}