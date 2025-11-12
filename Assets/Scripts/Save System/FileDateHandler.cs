using System.IO;
using UnityEngine;
using Newtonsoft.Json; 

public class FileDataHandler
{
    private readonly string directoryPath;
    private readonly string fileName;

    public FileDataHandler(string directoryPath, string fileName)
    {
        this.directoryPath = directoryPath;
        this.fileName = fileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(directoryPath, fileName);
        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"No save file found at {fullPath}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(fullPath);

            return JsonConvert.DeserializeObject<GameData>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load file: {ex}");
            return null;
        }
    }

    public void Save(GameData data)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string fullPath = Path.Combine(directoryPath, fileName);

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        File.WriteAllText(fullPath, json);
    }

    public void Delete()
    {
        string fullPath = Path.Combine(directoryPath, fileName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    public bool FileExists()
    {
        string fullPath = Path.Combine(directoryPath, fileName);
        return File.Exists(fullPath);
    }
}
