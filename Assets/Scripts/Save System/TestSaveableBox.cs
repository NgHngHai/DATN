using UnityEngine;
public class TestSaveableBox : SaveableObject
{
    public override object CaptureState()
    {
        return new TransformData
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z
        };
    }

    public override void RestoreState(object state)
    {
        TransformData data = (TransformData)state;
        transform.position = new Vector3(data.x, data.y, data.z);
    }
}


[System.Serializable]
public class TransformData
{
    public float x;
    public float y;
    public float z;
}
