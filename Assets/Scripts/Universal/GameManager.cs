using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingleton<GameManager>
{
    private Animator firstTimeTransitionAnimator;
    private string bootstrapSceneName = "Bootstrap Scene";
    private string mainMenuSceneName = "MainMenu";
    private float playSessionTime;
    private bool isInGame;

    protected override void Awake()
    {
        base.Awake();

        firstTimeTransitionAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInGame)
            playSessionTime += Time.deltaTime;
    }

    public void StartGame(int saveSlotIndex)
    {
        firstTimeTransitionAnimator.SetTrigger("loadInGame");
        SaveSystem.Instance.LoadGame(saveSlotIndex);
        playSessionTime = SaveSystem.Instance.GetGameData().playTimeSession;
    }

    public void ToMainMenu()
    {
        isInGame = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void FinishLoadInGame()
    {
        isInGame = true;
        firstTimeTransitionAnimator.speed = 1;
    }

    private void LoadBootstrapScene()
    {
        firstTimeTransitionAnimator.speed = 0;
        SceneManager.LoadScene(bootstrapSceneName);
    }

    public float GetPlaySessionTime() => playSessionTime;
}

