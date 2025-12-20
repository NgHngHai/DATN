using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : GenericSingleton<SaveSystem>
{
    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> saveables = new();

    private int activeSlotIndex;

    protected override void Awake()
    {
        base.Awake();
        dataHandler = new FileDataHandler();
    }

    #region Saveable Registration
    public void Register(ISaveable saveable)
    {
        if (!saveables.Contains(saveable))
            saveables.Add(saveable);
    }

    public void Unregister(ISaveable saveable)
    {
        saveables.Remove(saveable);
    }
    #endregion

    #region Game Data
    public void CreateNewGameData()
    {
        gameData = new GameData
        {
            saveSlotIndex = activeSlotIndex
        };
    }

    public void SaveGame()
    {
        if (gameData == null)
            CreateNewGameData();

        CaptureRegisteredStates();
        dataHandler.Save(gameData);

        Debug.Log($"Saved slot {gameData.saveSlotIndex}");
    }

    public void LoadGame(int activeSlotIndex)
    {
        this.activeSlotIndex = activeSlotIndex;
        gameData = dataHandler.Load(activeSlotIndex);

        if (gameData == null)
        {
            Debug.Log("No save -> New game");
            CreateNewGameData();
            return;
        }
    }

    public void DeleteSave()
    {
        dataHandler.Delete(gameData.saveSlotIndex);
        gameData = null;
    }
    #endregion

    #region Capture / Restore
    public void CaptureRegisteredStates()
    {
        if (gameData == null)
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

