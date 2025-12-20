using System.Collections.Generic;
using UnityEngine;

public enum GameArea
{
    WastelandProcessor,
    EuphoniousMelodia,
    NoData
}

public class RoomData : MonoBehaviour
{
    [SerializeField] private GameArea roomInArea;
    public Collider2D CameraBoundary;
    [SerializeField] private List<Door> doorList = new List<Door>();
    public Vector2 FirstSpawnPosition { get; set; }
    private Dictionary<string, Door> doorDict;

    [SerializeField] private Checkpoint roomCheckpoint;

    private void Awake()
    {
        doorDict = new Dictionary<string, Door>();

        foreach (Door door in doorList)
        {
            if (!doorDict.ContainsKey(door.LinkID))
                doorDict.Add(door.LinkID, door);
        }

        roomCheckpoint = FindFirstObjectByType<Checkpoint>();

    }

    public Vector2 GetDoorLinkPosition(string linkID)
    {
        if (doorDict.TryGetValue(linkID, out Door door))
        {
            FirstSpawnPosition = door.transform.position;
            return door.transform.position;
        }

        FirstSpawnPosition = Vector2.zero;
        return Vector2.zero;
    }

    public Vector2 GetRoomCheckpointPosition()
    {
        if (roomCheckpoint != null)
            return (Vector2)roomCheckpoint.transform.position;

        return FirstSpawnPosition;
    }
}
