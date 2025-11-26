using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public string currentRoom;
    [SerializeField] private GameObject[] _adjacentRooms;
    [SerializeField] private GameObject[] roomObjectList; 
    private Dictionary<string, GameObject> _roomList = new();

    // References
    public PlayerController playerController;
    [SerializeField] private CinemachineConfiner2D _cameraConfiner;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {

        foreach (var room in roomObjectList)
        {
            var roomData = room.GetComponent<RoomData>();
            if (roomData != null)
            {
                _roomList[roomData.roomID] = room;
            }
        }
    }

    public void SwitchCurrentRoom(string roomID)
    {
        // Unload adjacent rooms
        // Switch current room
        currentRoom = roomID;
        SetCameraBoundary();
        // Preload adjacent rooms
    }

    public void LoadAdjacentRooms()
    {
        // Instantiate room prefabs adjacent to current room
    }

    public void UnloadRooms(string roomID)
    {
        // Save rooms state
        // Destroy room prefabs adjacent to current room
    }

    public void SetCameraBoundary()
    {
        GameObject currentRoomObject = _roomList[currentRoom];
        _cameraConfiner.BoundingShape2D = currentRoomObject.GetComponent<RoomData>().roomCameraBoundary;
    }
}
