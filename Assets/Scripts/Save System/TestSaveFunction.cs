using UnityEngine;

public class TestSaveFunction : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(SaveGame), 6);
    }

    void SaveGame() => SaveSystem.Instance.SaveGame();
}
