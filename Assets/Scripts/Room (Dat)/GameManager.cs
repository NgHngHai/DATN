using System.Collections.Generic;
using UnityEngine;

namespace TestPersistentScene
{
    public class GameManager : GenericSingleton<GameManager>
    {
        public void StartNewGame()
        {
            SaveSystem.Instance.CreateNewGameData();
        }
    }

}