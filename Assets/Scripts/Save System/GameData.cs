using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public int saveSlotIndex;
    public float playTimeSession;
    public string saveRoomId = "Room1";
    public string spawnDoorId = "";

    public Dictionary<string, object> savedObjects = new();
}
