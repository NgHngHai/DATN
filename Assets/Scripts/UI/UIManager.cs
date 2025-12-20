using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class UIManager : GenericSingleton<UIManager>
{
    [Header("Player UI")]
    public Image hpBar;
    public Image tmpHpBar, mpBar, counterBar0, counterBar1, counterBar2;
    public GameObject playerStats, glow, mapCanvas;
    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject pauseText;
    [Header("Function Menu")]
    public GameObject functionMenu;
    public GameObject functionText, skillMenu, inventory, map, database;
    public Image skillText, inventoryText, mapText, databaseText;
    public FunctionButton skillButton, inventoryButton, mapButton, databaseButton;
    public ArrowButton prevButton, nextButton;

    InputAction pauseGameAction, openFunctionAction, qBtnAction, eBtnAction;
    int currentUiId;
    int totalCounter;


    protected override void Awake()
    {
        base.Awake();
        currentUiId = 0;
        pauseGameAction = new(null, type: InputActionType.Button, binding: "<Keyboard>/escape");
        openFunctionAction = new(null, type: InputActionType.Button, binding: "<Keyboard>/tab");
        qBtnAction = new(null, InputActionType.Button, "<Keyboard>/q");
        eBtnAction = new(null, InputActionType.Button, "<Keyboard>/e");
        pauseGameAction.Enable();
        openFunctionAction.Enable();
        totalCounter = 0;
    }

    void Update()
    {
        if (pauseGameAction.WasPerformedThisFrame() && currentUiId == 0)
        {
            currentUiId = 1;
            playerStats.SetActive(false);
            mapCanvas.SetActive(false);
            pauseMenu.SetActive(true);
            pauseText.SetActive(true);
            Time.timeScale = 0;
            openFunctionAction.Disable();
        }

        if (openFunctionAction.WasPerformedThisFrame())
        {
            if (currentUiId == 0)
            {
                currentUiId = 2;
                playerStats.SetActive(false);
                mapCanvas.SetActive(false);
                functionMenu.SetActive(true);
                functionText.SetActive(true);
                qBtnAction.Enable();
                eBtnAction.Enable();
                pauseGameAction.Disable();
            }
            else if (currentUiId > 0)
            {
                if (currentUiId > 2)
                {
                    if (currentUiId == 3) 
                    {
                        inventoryText.color = new(0, 0, 0, 0);
                        inventory.SetActive(false);
                    }
                    else if (currentUiId == 4)
                    {
                        mapText.color = new(0, 0, 0, 0);
                        map.SetActive(false);
                    }
                    else if (currentUiId == 5)
                    {
                        databaseText.color = new(0, 0, 0, 0);
                        database.SetActive(false);
                    }
                    skillText.color = new(1, 1, 1, 1);
                    skillMenu.SetActive(true);
                }
                currentUiId = 0;
                functionMenu.SetActive(false);
                functionText.SetActive(false);
                qBtnAction.Disable();
                eBtnAction.Disable();
                playerStats.SetActive(true);
                mapCanvas.SetActive(true);
                pauseGameAction.Enable();
            }
        }

        if(qBtnAction.WasPressedThisFrame())
        {
            DisplayPreviousTab();
        }
        if (eBtnAction.WasPressedThisFrame())
        {
            DisplayNextTab();
        }
    }

    public void ResumePlaying()
    {
        currentUiId = 0;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        pauseText.SetActive(false);
        playerStats.SetActive(true);
        mapCanvas.SetActive(true);
        openFunctionAction.Enable();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        pauseGameAction.Dispose();
        openFunctionAction.Dispose();
        qBtnAction.Dispose();
        eBtnAction.Dispose();
    }


    public void UpdateHP(int hp, int maxHP)
    {
        float f = (float)hp / maxHP;
        if (hpBar.fillAmount > f) StartCoroutine(LossHp(f));
        else StartCoroutine(HealHp(f));
    }
    IEnumerator LossHp(float f)
    {
        tmpHpBar.color = new Color32(255, 234, 96, 255);
        hpBar.fillAmount = f;
        float tmp = tmpHpBar.fillAmount - f;
        while (tmp > 0)
        {
            float factor = Mathf.Min(f * Time.deltaTime, tmp);
            tmpHpBar.fillAmount -= factor;
            tmp -= factor;
            yield return null;
        }
    }

    IEnumerator HealHp(float f)
    {
        tmpHpBar.color = new Color32(96, 255, 96, 255);
        tmpHpBar.fillAmount = f;
        float tmp = tmpHpBar.fillAmount - hpBar.fillAmount;
        while (tmp > 0)
        {
            float factor = Mathf.Min(0.25f * f * Time.deltaTime, tmp);
            hpBar.fillAmount += factor;
            tmp -= factor;
            yield return null;
        }
    }


    public void UpdateMp(int mp, int maxMp)
    {
        if (mp == maxMp)
        {
            glow.SetActive(true);
        }
        else
        {
            glow.SetActive(false);
        }
        mpBar.fillAmount = (float)mp / maxMp;
    }


    public void UpdateCounter(int c, int max)
    {
        if (c > totalCounter)
        {
            totalCounter = c;
            for (int i = 0; i < c; i++)
            {
                if (i == 0) counterBar0.color = new Color32(115, 246, 241, 255);
                else if (i == 1) counterBar1.color = new Color32(115, 246, 241, 255);
                else counterBar2.color = new Color32(115, 246, 241, 255);
            }
        }
        else if (c < totalCounter)
        {
            totalCounter = c;
            for (int i = 2; i >= c; i--)
            {
                if (i == 0) counterBar0.color = new Color32(255, 255, 255, 255);
                else if (i == 1) counterBar1.color = new Color32(255, 255, 255, 255);
                else counterBar2.color = new Color32(255, 255, 255, 255);
            }
        }
    }


    public void DisplayNextTab()
    {
        nextButton.SetEffectFactor(0, Time.time);
        HideFunctionMenuSpecifically();
        currentUiId += 1;
        if (currentUiId > 5) currentUiId = 2;
        DisplayFunctionMenuSpecifically(0);
    }

    public void DisplayPreviousTab()
    {
        prevButton.SetEffectFactor(0, Time.time);
        HideFunctionMenuSpecifically();
        currentUiId -= 1;
        if (currentUiId < 2) currentUiId = 5;
        DisplayFunctionMenuSpecifically(Mathf.PI);
    }

    public void DisplayFunctionMenuSpecifically(float rad)
    {
        if (currentUiId == 2)
        {
            skillText.color = new(1, 1, 1, 1);
            skillButton.SetEffectFactor(rad, Time.time);
            skillMenu.SetActive(true);
        }
        else if (currentUiId == 3)
        {
            inventoryText.color = new(1, 1, 1, 1);
            inventoryButton.SetEffectFactor(rad, Time.time);
            inventory.SetActive(true);
        }
        else if (currentUiId == 4)
        {
            mapText.color = new(1, 1, 1, 1);
            mapButton.SetEffectFactor(rad, Time.time);
            map.SetActive(true);
        }
        else if (currentUiId == 5)
        {
            databaseText.color = new(1, 1, 1, 1);
            databaseButton.SetEffectFactor(rad, Time.time);
            database.SetActive(true);
        }
    }

    public void HideFunctionMenuSpecifically()
    {
        if (currentUiId == 2)
        {
            skillText.color = new(0, 0, 0, 0);
            skillMenu.SetActive(false);
        }
        else if (currentUiId == 3)
        {
            inventoryText.color = new(0, 0, 0, 0);
            inventory.SetActive(false);
        }
        else if (currentUiId == 4)
        {
            mapText.color = new(0, 0, 0, 0);
            map.SetActive(false);
        }
        else if (currentUiId == 5)
        {
            databaseText.color = new(0, 0, 0, 0);
            database.SetActive(false);
        }
    }
}
