using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class UIManager : GenericSingleton<UIManager>
{
    public Image hpBar, tmpHpBar, mpBar, counterBar0, counterBar1, counterBar2;
    public GameObject playerStats, glow, mapCanvas, pauseText, functionText;
    public GameObject pauseMenu, functionMenu, skillMenu, inventory, map, database;
    private InputAction pauseGameAction, openFunctionAction, qBtnAction, eBtnAction;
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
            Time.timeScale = 0;
        }

        if (openFunctionAction.WasPerformedThisFrame())
        {
            if (currentUiId == 0)
            {
                currentUiId = 2;
                playerStats.SetActive(false);
                mapCanvas.SetActive(false);
                functionMenu.SetActive(true);
                pauseText.SetActive(false);
                functionText.SetActive(true);
                qBtnAction.Enable();
                eBtnAction.Enable();
            }
            else if (currentUiId > 0)
            {
                if (currentUiId > 1)
                {
                    if (currentUiId == 2) inventory.SetActive(false);
                    else if (currentUiId == 3) map.SetActive(false);
                    else if (currentUiId == 4) database.SetActive(false);
                    skillMenu.SetActive(true);
                }
                currentUiId = 0;
                playerStats.SetActive(true);
                mapCanvas.SetActive(true);
                functionMenu.SetActive(false);
                pauseText.SetActive(true);
                functionText.SetActive(false);
                qBtnAction.Disable();
                eBtnAction.Disable();
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
        playerStats.SetActive(true);
        mapCanvas.SetActive(true);
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
        HideFunctionMenuSpecifically();
        currentUiId += 1;
        if (currentUiId > 5) currentUiId = 2;
        DisplayFunctionMenuSpecifically();
    }

    public void DisplayPreviousTab()
    {
        HideFunctionMenuSpecifically();
        currentUiId -= 1;
        if (currentUiId < 2) currentUiId = 5;
        DisplayFunctionMenuSpecifically();
    }

    public void DisplayFunctionMenuSpecifically()
    {
        if (currentUiId == 2)
        {
            skillMenu.SetActive(true);
        }
        else if (currentUiId == 3)
        {
            inventory.SetActive(true);
        }
        else if (currentUiId == 4)
        {
            map.SetActive(true);
        }
        else if (currentUiId == 5)
        {
            database.SetActive(true);
        }
    }

    public void HideFunctionMenuSpecifically()
    {
        if (currentUiId == 2)
        {
            skillMenu.SetActive(false);
        }
        else if (currentUiId == 3)
        {
            inventory.SetActive(false);
        }
        else if (currentUiId == 4)
        {
            map.SetActive(false);
        }
        else if (currentUiId == 5)
        {
            database.SetActive(false);
        }
    }
}
