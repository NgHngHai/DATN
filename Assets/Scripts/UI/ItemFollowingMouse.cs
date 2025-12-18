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
        rect.anchoredPosition = Mouse.current.position.ReadValue();
    }
}
