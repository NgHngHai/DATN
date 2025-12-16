using UnityEngine;
using UnityEngine.EventSystems;

public class DatabaseSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public DatabaseController databaseController;
    GameObject slotFrameGo;

    void Awake()
    {
        slotFrameGo = transform.GetChild(0).gameObject;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (databaseController.IsSelectingDataIndex(transform.GetSiblingIndex())) return;
        databaseController.DisplayData(transform.GetSiblingIndex());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        slotFrameGo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (databaseController.IsSelectingDataIndex(transform.GetSiblingIndex())) return;
        slotFrameGo.SetActive(false);
    }
}
