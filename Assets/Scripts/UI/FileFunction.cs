using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FileFunction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int type = 0;
    public Sprite buttonSprite, hoveredButtonSprite;
    RectTransform rect;
    Image img;
    TextMeshProUGUI tmp;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (type == 0)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 80);
        img.sprite = hoveredButtonSprite;
        tmp.color = Color.black;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 100);
        img.sprite = buttonSprite;
        tmp.color = Color.white;
    }
}
