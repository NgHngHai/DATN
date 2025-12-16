using UnityEngine;
using UnityEngine.UI;

public class PauseButtonAnimating : BaseMeshEffect
{
    [Range(0, 1)]
    public float effectFactor = 0f;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        var verts = new System.Collections.Generic.List<UIVertex>();
        vh.GetUIVertexStream(verts);

        for (int i = 0; i < verts.Count; i++)
        {
            UIVertex v = verts[i];
            v.uv1 = new Vector2(effectFactor, 0);
            verts[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }

    public void SetEffectFactor(float val)
    {
        effectFactor = val;
        if (graphic != null) graphic.SetVerticesDirty();
    }
    

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (graphic != null) graphic.SetVerticesDirty();
    }
#endif
}
