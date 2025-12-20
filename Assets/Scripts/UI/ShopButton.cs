using UnityEngine;
using UnityEngine.EventSystems;

public class ShopButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int type = 0;
    public ShopController controller;
    GameObject goSelector;

    void Awake()
    {
        goSelector = transform.GetChild(0).gameObject;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (type == 0) controller.HoldPurchaseButton();
        else controller.HoldExitButton();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (type == 0) controller.ReleasePurchaseButton();
        else controller.ReleaseExitButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        goSelector.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        goSelector.SetActive(false);
    }
}
