using UnityEngine;
using UnityEngine.UI;

public class ArrowButton : BaseMeshEffect
{
    public float glitchDirection;
    public float startDraggingTime;


    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        var verts = new System.Collections.Generic.List<UIVertex>();
        vh.GetUIVertexStream(verts);

        for (int i = 0; i < verts.Count; i++)
        {
            UIVertex v = verts[i];
            v.uv1 = new Vector2(glitchDirection, startDraggingTime);
            verts[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }
    

    public void SetEffectFactor(float direction, float startTime)
    {
        glitchDirection = direction;
        startDraggingTime = startTime;
        if (graphic != null) graphic.SetVerticesDirty();
    }
}
