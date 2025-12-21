using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FileSelection : MonoBehaviour
{
    [SerializeField] private FileButton[] fileButtons;
    int selectingFileIndex;
    FileDataHandler dataHandler;

    void Awake()
    {
        selectingFileIndex = -1;
        dataHandler = new FileDataHandler();
        InitializeFileButtons();
    }


    private void InitializeFileButtons()
    {
        List<GameData> fileDatas = dataHandler.LoadAllSaves();

        for (int i = 0; i < fileButtons.Length; i++)
        {
            FileDisplayData displayData = null;

            GameData fileData = null;
            foreach(GameData data in fileDatas)
            {
                if(data.saveSlotIndex == i)
                    fileData = data;
            }

            if (fileData == null)
            {
                fileButtons[i].Display(null);
                continue;
            }

            if (fileData.savedObjects.TryGetValue("fileDisplayData", out object raw))
            {
                if (raw is JObject jObj)
                    displayData = jObj.ToObject<FileDisplayData>();
                else
                    displayData = raw as FileDisplayData;
            }
            fileButtons[i].Display(displayData);

        }
    }


    public void StartPlayingGame()
    {
        if (selectingFileIndex != -1)
            GameManager.Instance.StartGame(selectingFileIndex);
    }


    public void ClickFileButton(int fileIndex)
    {
        if (selectingFileIndex != -1)
        {
            fileButtons[selectingFileIndex].StopFocusing();
        }
        selectingFileIndex = fileIndex;
        fileButtons[selectingFileIndex].StartFocusing();
    }

    public void DeleteCurrentSelectSaveSlot()
    {
        if(selectingFileIndex != -1)
        {
            dataHandler.Delete(selectingFileIndex);
            fileButtons[selectingFileIndex].Display(null);
        }
    }

    public bool IsSelectingFile(int i)
    {
        return i == selectingFileIndex;
    }
}
