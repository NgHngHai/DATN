using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private readonly string directoryPath;

    public FileDataHandler()
    {
        directoryPath = Application.persistentDataPath;
    }

    public void Save(GameData data)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(GetFullPath(data.saveSlotIndex), json);
    }

    public GameData Load(int slotIndex)
    {
        string fullPath = GetFullPath(slotIndex);

        if (!File.Exists(fullPath))
            return null;

        string json = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<GameData>(json);
    }

    public void Delete(int slotIndex)
    {
        string path = GetFullPath(slotIndex);
        if (File.Exists(path))
            File.Delete(path);
    }

    public List<GameData> LoadAllSaves()
    {
        List<GameData> allSaves = new();

        if (!Directory.Exists(directoryPath))
            return allSaves;

        string[] files = Directory.GetFiles(directoryPath, "save_file_*.json");

        foreach (string filePath in files)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                GameData data = JsonConvert.DeserializeObject<GameData>(json);
                if (data != null)
                    allSaves.Add(data);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Load lỗi {filePath}: {e.Message}");
            }
        }

        return allSaves;
    }

    public int GetSaveFileCount()
    {
        if (!Directory.Exists(directoryPath))
            return 0;

        return Directory.GetFiles(directoryPath, "save_file_*.json").Length;
    }


    private string GetFullPath(int slotIndex)
    {
        return Path.Combine(directoryPath, $"save_file_{slotIndex}.json");
    }
}
