using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject fileSelectionUI, controlsUI, creditsUI;
    public Transform screenCanvas;

    void Awake()
    {
        
    }

    public void OpenFileSelectionUI()
    {
        fileSelectionUI.SetActive(true);
    }

    public void OpenControlsUI()
    {
        controlsUI.SetActive(true);
    }

    public void OpenCreditsUI()
    {
        creditsUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    
    public void DisplayMenuButtons(bool b)
    {
        for (int i = 0; i < 5; i++) screenCanvas.GetChild(i).gameObject.SetActive(b);
    }
    
}
