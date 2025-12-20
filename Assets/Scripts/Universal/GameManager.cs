using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingleton<GameManager>
{
    private Animator firstTimeTransitionAnimator;
    private string bootstrapSceneName = "Bootstrap Scene";
    private string mainMenuSceneName = "MainMenu";

    protected override void Awake()
    {
        base.Awake();

        firstTimeTransitionAnimator = GetComponent<Animator>();
    }

    public void StartGame(int saveSlotIndex)
    {
        firstTimeTransitionAnimator.SetTrigger("loadInGame");
        SaveSystem.Instance.LoadGame(saveSlotIndex);
    }

    private void LoadBootstrapScene()
    {
        firstTimeTransitionAnimator.speed = 0;
        SceneManager.LoadScene(bootstrapSceneName);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void FinishFirstTimeLoadInGameTransition()
    {
        firstTimeTransitionAnimator.speed = 1;
    }

}

