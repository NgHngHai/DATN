using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static SaveSystem _instance;
    public static SaveSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SaveSystem>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SaveSystem");
                    _instance = go.AddComponent<SaveSystem>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> saveables = new List<ISaveable>();

    [Header("File Settings")]
    [SerializeField] private string fileName = "save.json";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        string path = Application.persistentDataPath;
        dataHandler = new FileDataHandler(path, fileName);
    }

    public void Register(ISaveable saveable)
    {
        if (!saveables.Contains(saveable))
            saveables.Add(saveable);
    }

    public void Unregister(ISaveable saveable)
    {
        if (saveables.Contains(saveable))
            saveables.Remove(saveable);
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void SaveGame()
    {
        if (gameData == null)
            gameData = new GameData();

        foreach (var saveable in saveables)
        {
            string id = saveable.GetUniqueID();
            object state = saveable.CaptureState();
            gameData.savedObjects[id] = state;
        }

        dataHandler.Save(gameData);
        Debug.Log("✅ Game saved!");
    }

    // Problem
    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("⚠️ No save found, starting new game.");
            NewGame();
            return;
        }

        foreach (var saveable in saveables)
        {
            string id = saveable.GetUniqueID();
            if (gameData.savedObjects.TryGetValue(id, out object state))
                saveable.RestoreState(state);
        }

        Debug.Log("✅ Game loaded!");
    }

    public void DeleteSave()
    {
        dataHandler.Delete();
        Debug.Log("🗑 Save deleted!");
    }

    public bool SaveExists() => dataHandler.FileExists();
}
