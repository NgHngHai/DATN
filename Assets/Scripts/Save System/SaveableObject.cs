using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public abstract class SaveableObject : MonoBehaviour, ISaveable
{
    private UniqueID uniqueID;

    protected virtual void Awake()
    {
        uniqueID = GetComponent<UniqueID>();
        SaveSystem.Instance.Register(this);   
    }

    protected virtual void OnDestroy()
    {
        SaveSystem.Instance.Unregister(this);
    }

    public string GetUniqueID() => uniqueID.ID;

    public abstract object CaptureState();
    public abstract void RestoreState(object state);
}
