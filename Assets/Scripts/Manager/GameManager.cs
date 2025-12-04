using System.Collections.Generic;
using UnityEngine;

public class GameManager : GenericSingleton<GameManager>
{
    public void StartNewGame()
    {
        SaveSystem.Instance.CreateNewGameData();
    }
}

