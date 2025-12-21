using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlsButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int id = 0;
    public int type = 0;
    public ControlsController controller;
    Image image;
    TextMeshProUGUI txtControlName, txtMappingKey;

    void Awake()
    {
        image = GetComponent<Image>();
        txtControlName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        txtMappingKey = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller.IsSelectingControlId(id)) return;
        txtMappingKey.color = new (0, 0, 0, 1);
        controller.FocusButton(id);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (controller.IsSelectingControlId(id)) return;
        txtControlName.color = new Color32(221, 255, 255, 255);
        txtMappingKey.color = new Color32(221, 255, 255, 255);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (controller.IsSelectingControlId(id)) return;
        txtControlName.color = new Color32(144, 169, 177, 255);
        txtMappingKey.color = new Color32(144, 169, 177, 255);
    }


    public void ChangeSprite(Sprite shortBtnSprite, Sprite longBtnSprite)
    {
        image.sprite = (type == 0) ? shortBtnSprite : longBtnSprite;
        if (controller.IsSelectingControlId(id))
        {
            txtControlName.color = new Color32(144, 169, 177, 255);
            txtMappingKey.color = new Color32(144, 169, 177, 255);
        }
    }
}
