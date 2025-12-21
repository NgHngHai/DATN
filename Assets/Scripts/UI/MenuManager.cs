using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject fileSelectionUI;
    public Transform screenCanvas;

    void Awake()
    {
        
    }

    public void OpenFileSelectionUI()
    {
        fileSelectionUI.SetActive(true);
    }

    
    public void DisplayMenuButtons(bool b)
    {
        for (int i = 0; i < 6; i++) screenCanvas.GetChild(i).gameObject.SetActive(b);
    }
    
}
