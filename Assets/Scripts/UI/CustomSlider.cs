using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CustomSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int leftLimit = 0;
    public int rightLimit = 100;
    public int maxValue = 20;
    public TextMeshProUGUI txtVolume;

    public bool canDrag = false;
    float originMousePosX, offset, f;
    Image imgVolumeBar;
    RectTransform rect;


    void Awake()
    {
        rect = GetComponent<RectTransform>();
        imgVolumeBar = transform.parent.GetChild(1).GetComponent<Image>();
        f = 1;
    }


    void Update()
    {
        if (canDrag)
        {
            float mousePosX = Mouse.current.position.ReadValue().x;
            f = Mathf.InverseLerp(originMousePosX - 200, originMousePosX, mousePosX - offset);
            txtVolume.text = Mathf.RoundToInt(maxValue * f).ToString();
            imgVolumeBar.fillAmount = f;
            
            rect.anchoredPosition = new (Mathf.Lerp(leftLimit, rightLimit, f), rect.anchoredPosition.y);
            // AudioManager.Instance.SetMasterVolume(f);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        imgVolumeBar.color = new Color32(115, 228, 198, 255);
        originMousePosX = Mouse.current.position.ReadValue().x;
        canDrag = true;
        offset = (1 - f) * 200;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        imgVolumeBar.color = new Color32(1, 152, 143, 255);
        canDrag = false;
    }
}
