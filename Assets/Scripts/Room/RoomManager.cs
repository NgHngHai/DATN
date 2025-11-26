using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    public string currentRoom;
    [SerializeField] private GameObject[] _adjacentRooms; // (optional, not used by code below)
    [SerializeField] private GameObject[] _roomObjectList; // Prefabs (assets)
    [SerializeField] private List<GameObject> _loadedRoomList = new(); // Instantiated room instances
    private readonly Dictionary<string, GameObject> _roomList = new(); // roomID -> prefab
    private string _previousRoom;

    // References
    public PlayerController playerController;
    [SerializeField] private CinemachineConfiner2D _cameraConfiner;
    private GameObject currentRoomObject;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // Fallback to player's starting room if not set in inspector
        if (string.IsNullOrEmpty(currentRoom) && playerController != null)
            currentRoom = playerController.currentRoomID;
    }

    private void Start()
    {
        // Build prefab lookup by roomID
        foreach (var room in _roomObjectList)
        {
            if (room == null) continue;
            var roomData = room.GetComponent<RoomData>();
            if (roomData != null && !string.IsNullOrEmpty(roomData.roomID))
            {
                _roomList[roomData.roomID] = room;
            }
        }

        // Initial load and camera setup
        LoadAdjacentRooms();
        SetCameraBoundary();
    }

    public void SwitchCurrentRoom(string roomID)
    {
        // Switch current room
        _previousRoom = currentRoom;
        currentRoom = roomID;

        // Unload rooms not needed anymore, load needed ones, then update camera boundary
        UnloadRooms();
        LoadAdjacentRooms();
        SetCameraBoundary();
    }

    public void LoadAdjacentRooms()
    {
        if (!_roomList.ContainsKey(currentRoom))
        {
            Debug.LogError($"RoomManager: Unknown roomID '{currentRoom}'.");
            return;
        }

        // Ensure current room is loaded
        EnsureRoomLoaded(currentRoom);

        // Ensure all adjacent rooms are loaded
        var currentRoomData = _roomList[currentRoom].GetComponent<RoomData>();
        if (currentRoomData != null && currentRoomData.adjacentRooms != null)
        {
            foreach (string adjacentRoomID in currentRoomData.adjacentRooms)
            {
                EnsureRoomLoaded(adjacentRoomID);
            }
        }

        // Update cached instance reference for current room
        currentRoomObject = FindLoadedRoomInstance(currentRoom);
    }

    public void UnloadRooms()
    {
        if (!_roomList.ContainsKey(currentRoom))
            return;

        // Desired set = current room + its adjacents
        var desired = new HashSet<string> { currentRoom };
        var currentRoomData = _roomList[currentRoom].GetComponent<RoomData>();
        if (currentRoomData != null && currentRoomData.adjacentRooms != null)
        {
            foreach (var id in currentRoomData.adjacentRooms)
                desired.Add(id);
        }

        // Destroy instances not in desired set
        for (int i = _loadedRoomList.Count - 1; i >= 0; i--)
        {
            var instance = _loadedRoomList[i];
            if (instance == null)
            {
                _loadedRoomList.RemoveAt(i);
                continue;
            }

            var data = instance.GetComponent<RoomData>();
            var id = data != null ? data.roomID : null;

            if (string.IsNullOrEmpty(id) || !desired.Contains(id))
            {
                Destroy(instance); // Destroy scene instance, not prefab asset
                _loadedRoomList.RemoveAt(i);
            }
        }
    }

    public void SetCameraBoundary()
    {
        // Use the current room INSTANCE collider for confiner, not the prefab
        var instance = FindLoadedRoomInstance(currentRoom);
        if (instance == null)
        {
            Debug.LogWarning("RoomManager: Current room instance not found when setting camera boundary. Loading it now.");
            instance = EnsureRoomLoaded(currentRoom);
        }

        var data = instance != null ? instance.GetComponent<RoomData>() : null;
        if (data != null && _cameraConfiner != null)
        {
            _cameraConfiner.BoundingShape2D = data.roomCameraBoundary;
        }

        currentRoomObject = instance;
    }

    // Helpers

    private GameObject EnsureRoomLoaded(string roomID)
    {
        var loaded = FindLoadedRoomInstance(roomID);
        if (loaded != null)
            return loaded;

        if (!_roomList.TryGetValue(roomID, out var prefab) || prefab == null)
        {
            Debug.LogError($"RoomManager: Prefab for roomID '{roomID}' not found.");
            return null;
        }

        var prefabData = prefab.GetComponent<RoomData>();
        var spawnPos = prefabData != null ? prefabData.roomPos : Vector2.zero;

        var instance = Instantiate(prefab, spawnPos, Quaternion.identity);
        _loadedRoomList.Add(instance);
        return instance;
    }

    private GameObject FindLoadedRoomInstance(string roomID)
    {
        for (int i = 0; i < _loadedRoomList.Count; i++)
        {
            var go = _loadedRoomList[i];
            if (go == null) continue;
            var data = go.GetComponent<RoomData>();
            if (data != null && data.roomID == roomID)
                return go;
        }
        return null;
    }
}
