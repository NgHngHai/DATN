using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public string roomID;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private string adjacentRooms;
    [SerializeField] private List<ISaveable> _saveables = new();

    private Coroutine _scanRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _scanRoutine = StartCoroutine(ScanForSaveables());
    }

    private IEnumerator ScanForSaveables()
    {
        // Delay one frame to ensure all objects are initialized
        yield return null;

        // Finds all active MonoBehaviours in the currently loaded scene(s) and filters those implementing ISaveable.
        _saveables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID)
            .OfType<ISaveable>()
            .ToList();

        Debug.Log($"Collected {_saveables.Count} ISaveable objects.");

        _scanRoutine = null;
    }

    // Load the saved state for all saveable objects in this room
    private void LoadRoomState()
    {
        foreach (var saveable in _saveables)
        {
            // Assuming a SaveSystem class exists to handle loading
            var state = 0; //= SaveSystem.LoadState(saveable.GetUniqueID());
            if (state != null)
            {
                saveable.RestoreState(state);
            }
        }
    }

    // Give each saveable a unique ID based on the room ID and its index in the list
    //private void LabelSaveables()
    //{
    //    foreach (var (saveable, index) in _saveables.Select((value, i) => (value, i)))
    //    {
    //        if (saveable is MonoBehaviour mb)
    //        {
    //            if (mb.TryGetComponent<ISaveable>(out var saveableComponent))
    //            {
    //                var uniqueID = $"{roomID}-{index}";
    //                // Assuming ISaveable has a method to set unique ID
    //                // saveableComponent.SetUniqueID(uniqueID);
    //                Debug.Log($"Assigned Unique ID: {uniqueID} to {mb.name}");
    //            }
    //        }
    //    }
    //}    
}
