using UnityEngine;
using System;

[DisallowMultipleComponent]
public class UniqueID : MonoBehaviour
{
    [SerializeField, HideInInspector] private string uniqueID;

    public string ID => uniqueID;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Generate ID only when in Editor (to not change ID each time run)
        if (string.IsNullOrEmpty(uniqueID) || !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            uniqueID = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif

    private void Awake()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = Guid.NewGuid().ToString();
        }
    }
}
