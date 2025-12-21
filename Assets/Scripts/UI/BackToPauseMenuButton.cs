using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackToPauseMenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    TextMeshProUGUI txtBack;

    void Awake()
    {
        txtBack = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        txtBack.color = new Color32(144, 169, 177, 255);
        transform.parent.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        txtBack.color = new Color32(221, 255, 255, 255);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        txtBack.color = new Color32(144, 169, 177, 255);
    }
}
