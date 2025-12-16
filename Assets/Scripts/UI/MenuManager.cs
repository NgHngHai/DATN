using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject fileSelectionUI;

    public void OpenFileSelectionUI()
    {
        fileSelectionUI.SetActive(true);
    }
}
