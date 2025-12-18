using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryController inventoryController;
    GameObject slotFrameGo;

    void Awake()
    {
        slotFrameGo = transform.GetChild(0).gameObject;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        int id = transform.GetSiblingIndex();
        if (inventoryController.HasSomethingInSlotIndex(id)) 
        {
            if (!inventoryController.IsSelectingSlotIndex(id))
                inventoryController.DisplayData(id);
            inventoryController.SetInventoryState();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inventoryController.ResetInventoryState();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        int id = transform.GetSiblingIndex();
        inventoryController.hoveringSlot = id;
        if (inventoryController.isDragging || inventoryController.HasSomethingInSlotIndex(id))
        {
            slotFrameGo.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.hoveringSlot = -1;
        int id = transform.GetSiblingIndex();
        if (inventoryController.IsSelectingSlotIndex(id)) return;
        slotFrameGo.SetActive(false);
    }
}
