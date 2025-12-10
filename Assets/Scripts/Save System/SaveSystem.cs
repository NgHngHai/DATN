using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : GenericSingleton<SaveSystem>
{
    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> saveables = new List<ISaveable>();

    [Header("File Settings")]
    [SerializeField] private string fileName = "save.json";

    protected override void Awake()
    {
        base.Awake();

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

    #region Game Data Functions
    public void CreateNewGameData()
    {
        gameData = new GameData();
    }

    public void SaveGame()
    {
        if (gameData == null)
            CreateNewGameData();

        CaptureRegisteredStates();
        dataHandler.Save(gameData);

        Debug.Log("Game saved!");
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (gameData == null)
        {
            Debug.Log("No save found -> creating new");
            CreateNewGameData();
            return;
        }

        RestoreRegisteredStates();
        Debug.Log("Game loaded!");
    }

    public void DeleteSave()
    {
        dataHandler.Delete();
    }
    #endregion

    #region Capture & Restore States
    public void CaptureRegisteredStates()
    {
        if(gameData == null)
            CreateNewGameData();

        foreach (var saveable in saveables)
        {
            string id = saveable.GetUniqueID();
            object state = saveable.CaptureState();
            gameData.savedObjects[id] = state;
        }
    }

    public void RestoreRegisteredStates()
    {
        if (gameData == null)
            CreateNewGameData();

        foreach (var saveable in saveables)
        {
            string id = saveable.GetUniqueID();
            if (gameData.savedObjects.TryGetValue(id, out object state))
                saveable.RestoreState(state);
        }
    }

    #endregion
}

