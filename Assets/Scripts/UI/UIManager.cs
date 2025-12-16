using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : GenericSingleton<UIManager>
{
    public GameObject pauseMenu;
    private InputAction pauseGameAction;
    int currentUiId;

    protected override void Awake()
    {
        base.Awake();
        currentUiId = 0;
        pauseGameAction = new(null, type: InputActionType.Button, binding: "<Keyboard>/escape");
        pauseGameAction.Enable();
    }

    void Update()
    {
        if (pauseGameAction.WasPerformedThisFrame() && currentUiId != 1)
        {
            pauseMenu.SetActive(true);
        } 
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        pauseGameAction.Dispose();
    }
}
