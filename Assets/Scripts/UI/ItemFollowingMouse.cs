using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemFollowingMouse : BaseMeshEffect
{
    public float mouseDeltaThreshold;

    RectTransform rect;
    bool isDisplayed;
    float rotation, startTime;


    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        var verts = new System.Collections.Generic.List<UIVertex>();
        vh.GetUIVertexStream(verts);

        for (int i = 0; i < verts.Count; i++)
        {
            UIVertex v = verts[i];
            v.uv1 = new Vector2(rotation, startTime);
            verts[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }


    protected override void Awake()
    {
        base.Awake();

        rect = GetComponent<RectTransform>();
        isDisplayed = false;
    }


    void Update()
    {
        if (isDisplayed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            if (delta.sqrMagnitude > mouseDeltaThreshold)
            {
                rotation = Mathf.Atan2(delta.y, delta.x);
                startTime = Time.time;
                if (graphic != null) graphic.SetVerticesDirty();
            }
            rect.anchoredPosition = new(Mouse.current.position.ReadValue().x / 1920 * Screen.currentResolution.width, Mouse.current.position.ReadValue().y / 1080 * Screen.currentResolution.height);
        }
    }


    public void Display()
    {
        isDisplayed = true;
        rotation = 0;
        startTime = 0;
        if (graphic != null) graphic.SetVerticesDirty();
    }

    public void Hide()
    {
        isDisplayed = false;
    }
}
