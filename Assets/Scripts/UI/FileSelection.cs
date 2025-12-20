using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FileSelection : MonoBehaviour
{
    [SerializeField] private FileButton[] fileButtons;
    int selectingFileIndex;

    void Awake()
    {
        selectingFileIndex = -1;
        InitializeFileButtons();
    }

    private void InitializeFileButtons()
    {
        FileDataHandler dataHandler = new FileDataHandler();
        List<GameData> fileDatas = dataHandler.LoadAllSaves();

        for (int i = 0; i < fileButtons.Length; i++)
        {
            if (i >= fileDatas.Count || fileDatas[i] == null)
            {
                fileButtons[i].Display(null);
            }
            else
            {
                fileButtons[i].Display(new FileButtonDisplayData
                {
                    metaData = "Saved file",
                    progress = fileDatas[i].savedAtTicks.ToString("HH:mm:ss dd/MM/yyyy")
                });
            }
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


    public bool IsSelectingFile(int i)
    {
        return i == selectingFileIndex;
    }
}
