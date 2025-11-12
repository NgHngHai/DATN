using UnityEngine;

public interface ISaveable
{
    string GetUniqueID();  // Generate random ID at somewhere
    object CaptureState();
    void RestoreState(object state);
}
