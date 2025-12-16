using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FileSelection : MonoBehaviour
{
    int selectingFileIndex;

    void Awake()
    {
        selectingFileIndex = -1;
    }


    public void StartPlayingGame()
    {
        SceneManager.LoadScene(2);
    }


    public void ClickFileButton(int fileIndex)
    {
        if (selectingFileIndex != -1)
        {
            transform.GetChild(2).GetChild(selectingFileIndex).GetComponent<FileButton>().StopFocusing();
        }
        selectingFileIndex = fileIndex;
        transform.GetChild(2).GetChild(selectingFileIndex).GetComponent<FileButton>().StartFocusing();
    }


    public bool IsSelectingFile(int i)
    {
        return i == selectingFileIndex;
    }
}
