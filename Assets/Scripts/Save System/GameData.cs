using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    // In the future there will be more types of data to store.
    public Dictionary<string, object> savedObjects = new();
}
