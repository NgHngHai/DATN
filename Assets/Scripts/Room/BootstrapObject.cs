using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootstrapObject : MonoBehaviour
{
    [SerializeField] private string persistentSceneName = "Persistent Scene";
    [SerializeField] private string firstRoomName = "Room 1";

    private void Start()
    {
        StartCoroutine(LoadPersistentAndStartGame());
    }

    private IEnumerator LoadPersistentAndStartGame()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(persistentSceneName, LoadSceneMode.Additive);
        yield return op;

        Scene persistent = SceneManager.GetSceneByName(persistentSceneName);

        RoomManager roomManager = FindFirstObjectByType<RoomManager>();
        roomManager.LoadRoomWithNoTransition(firstRoomName);

        SceneManager.SetActiveScene(persistent);
        Scene bootScene = gameObject.scene;
        SceneManager.UnloadSceneAsync(bootScene);
    }
}

