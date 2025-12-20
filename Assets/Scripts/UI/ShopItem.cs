using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ShopController controller;
    GameObject goSelector;

    void Awake()
    {
        goSelector = transform.GetChild(0).gameObject;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        controller.DisplayItemData(transform.GetSiblingIndex());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        goSelector.SetActive(true);
        controller.HoverItem(transform.GetSiblingIndex());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        goSelector.SetActive(false);
    }
}
