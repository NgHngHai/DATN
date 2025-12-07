using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(WipeButtonController))]


public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GlitchController glitchController;
    WipeButtonController wipeButtonController;

    public void Start()
    {
        wipeButtonController = GetComponent<WipeButtonController>();
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            glitchController.StartSceneTransition();
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        wipeButtonController.StartWipeEffect();
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        wipeButtonController.StopWipeEffect();
    }
}