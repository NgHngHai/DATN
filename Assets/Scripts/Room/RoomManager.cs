using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public string currentRoom;
    [SerializeField] private GameObject[] _adjacentRooms;
    [SerializeField] private GameObject[] roomObjectList; 
    private Dictionary<string, GameObject> _roomList = new();
    private string _previousRoom;

    // References
    public PlayerController playerController;
    [SerializeField] private CinemachineConfiner2D _cameraConfiner;
    private GameObject currentRoomObject;

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

        LoadAdjacentRooms();
    }

    public void SwitchCurrentRoom(string roomID)
    {
        // Unload adjacent rooms
        // Switch current room
        _previousRoom = currentRoom;
        currentRoom = roomID;
        SetCameraBoundary();
        LoadAdjacentRooms();
    }

    public void LoadAdjacentRooms()
    {
        currentRoomObject = _roomList[currentRoom];
        RoomData currentRoomData = currentRoomObject.GetComponent<RoomData>();

        foreach (string adjacentRoomID in currentRoomData.adjacentRooms)
        {
            if (adjacentRoomID == _previousRoom)
                continue; // Skip loading the previous room to avoid duplicates
            RoomData adjacentRoomData = _roomList[adjacentRoomID].GetComponent<RoomData>();
            Instantiate(_roomList[adjacentRoomID], adjacentRoomData.roomPos, Quaternion.identity);
        }
    }

    public void UnloadRooms(string roomID)
    {
        // Save rooms state
        // Destroy room prefabs adjacent to current room
    }

    public void SetCameraBoundary()
    {
        currentRoomObject = _roomList[currentRoom];
        _cameraConfiner.BoundingShape2D = currentRoomObject.GetComponent<RoomData>().roomCameraBoundary;
    }
}
