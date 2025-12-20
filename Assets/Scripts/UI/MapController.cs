using UnityEngine;
using UnityEngine.InputSystem;

public class MapController : MonoBehaviour
{
    public Camera mapCamera;
    public float scrollSensitivity = 1;
    public float mouseValueScalingFactor = 1000;
    public float maxCameraSize = 60;
    public float minCameraSize = 10;

    float size;
    bool isDragging;
    Vector2 startDraggingPosition, currentCameraPosition;


    void Awake()
    {
        size = 30;
        isDragging = false;
        currentCameraPosition = new (0, 0);
    }

    void Update()
    {
        size -= Mouse.current.scroll.y.ReadValue();
        size = Mathf.Clamp(size, minCameraSize, maxCameraSize);
        mapCamera.orthographicSize = size;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            startDraggingPosition = Mouse.current.position.ReadValue();
        }
        if (isDragging)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
                currentCameraPosition = mapCamera.transform.localPosition;
                return;
            }
            mapCamera.transform.localPosition = currentCameraPosition - (Mouse.current.position.ReadValue() - startDraggingPosition) / mouseValueScalingFactor;
        }
    }


    void OnDisable()
    {
        currentCameraPosition = new(0, 0);
        mapCamera.transform.localPosition = currentCameraPosition;

        isDragging = false;
    }
}
