using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public int saveSlotIndex;
    public long savedAtTicks;

    public Dictionary<string, object> savedObjects = new();
}
