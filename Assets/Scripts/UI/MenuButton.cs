using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MenuButtonController))]
public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    MenuButtonController menuButtonController;
    public MenuManager menuManager;
    int btnId;

    public void Start()
    {
        menuButtonController = GetComponent<MenuButtonController>();
        btnId = transform.GetSiblingIndex();
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (btnId == 0)
        {
            menuManager.OpenFileSelectionUI();
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        menuButtonController.StartWipeEffect();
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        menuButtonController.StopWipeEffect();
    }
}