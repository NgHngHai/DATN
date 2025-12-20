using UnityEngine;
using UnityEngine.SceneManagement;

public class MapRoomManager : SaveableObject
{
    public static MapRoomManager instance;

    private MapContainerData[] _rooms;

    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _rooms = GetComponentsInChildren<MapContainerData>(true);

        base.Awake();

    }

    public void RevealRoom(string roomID)
    {

        foreach (var room in _rooms)
        {
            if (room.roomID == roomID && !room.isDiscovered)
            {
                room.gameObject.SetActive(true);
                room.isDiscovered = true;

                return;
            }
        }
    }

    public override object CaptureState()
    {
        string discoveredRoomsID = string.Empty;
        foreach (var room in _rooms)
        {
            if (room.isDiscovered)
            {
                discoveredRoomsID += room.roomID + "-";
            }
        }
        return new MapState
        {
            discoveredRoomsID = discoveredRoomsID
        };
    }

    public override void RestoreState(object state)
    {
        var mapState = Utility.ConvertState<MapState>(state);

        string[] discoveredRooms = mapState.discoveredRoomsID.Split('-');
        foreach (var roomID in discoveredRooms)
        {
            foreach (var room in _rooms)
            {
                if (room.roomID == roomID)
                {
                    room.isDiscovered = true;
                    room.gameObject.SetActive(true);
                }
            }
        }
    }

    [System.Serializable]
    public struct MapState
    {
        public string discoveredRoomsID;
    }
}
