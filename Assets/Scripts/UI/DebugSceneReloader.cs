using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugSceneReloader : MonoBehaviour
{
    public int sceneIndex = 0;
    InputAction reloadAction;

    void Start()
    {
        reloadAction = InputSystem.actions.FindAction("[DEBUG]ReloadScene");
    }

    void Update()
    {
        if (reloadAction.WasPressedThisFrame())
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}