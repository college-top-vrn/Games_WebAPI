using System.Text.Json;

namespace GamesWebAPI;

public static class StorageManager
{
    public static List<T> ReadFromFile<T>(string path) where T: IEntity
    {
        try
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "[]");
                return new List<T>();
            }

            var fileText = File.ReadAllText(path);
            var objects = JsonSerializer.Deserialize<List<T>>(fileText)

            if (objects == null)
            {
                objects = new List<T>();
                File.WriteAllText(path, JsonSerializer.Serialize(objects));
            }
            
            return objects;
        }
        catch (JsonException)
        {
            File.WriteAllText(path, "[]");
            return new List<T>();
        }
    }

    public static void WriteToFile<T>(List<T> objects, string path) where T: IEntity
    {
        var json = JsonSerializer.Serialize(objects);
        File.WriteAllText(path, json);
    }
}