using UnityEngine;
using UnityEngine.EventSystems;

public class AnchoredSkill : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryController inventoryController;


    public void OnPointerDown(PointerEventData eventData)
    {
        inventoryController.DisplayAnchoredSkillData();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.isHoveringSkillAnchor = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.isHoveringSkillAnchor = false;
    }
}
