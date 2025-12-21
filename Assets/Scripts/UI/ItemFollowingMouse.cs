using UnityEngine;
using UnityEngine.InputSystem;

public class ItemFollowingMouse : MonoBehaviour
{
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.anchoredPosition = new(Mouse.current.position.ReadValue().x / 1920 * Screen.currentResolution.width, Mouse.current.position.ReadValue().y / 1080 * Screen.currentResolution.height);
    }
}
