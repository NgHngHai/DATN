using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    public Image hpBar;
    public Image tmpHpBar, mpBar, counterBar0, counterBar1, counterBar2;
    public GameObject playerStats, glow, mapCanvas;
    public GameObject moneyContainer;
    public TextMeshProUGUI txtMoney;
    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject pauseText;
    public GameObject controlsPopup;
    [Header("Function Menu")]
    public GameObject functionMenu;
    public GameObject functionText, skillMenu, draggingItem, inventory, map, database;
    public Image skillText, inventoryText, mapText, databaseText;
    public FunctionButton skillButton, inventoryButton, mapButton, databaseButton;
    public ScaleBouncedObject prevButton, nextButton;
    public SkillTreeController skillTreeController;
    public InventoryController inventoryController;
    [Header("Shop")]
    public GameObject shop;
    [Header("Player")]
    public PlayerSkillManager skillManager;

    InputAction pauseGameAction, openFunctionAction, qBtnAction, eBtnAction;
    int currentUiId;
    int totalCounter;


    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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
            moneyContainer.SetActive(false);
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
                moneyContainer.SetActive(false);
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


    public void OpenControlsPopup()
    {
        controlsPopup.SetActive(true);
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

    private void OnDestroy()
    {
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
                if (i == 0)
                {
                    counterBar0.color = new Color32(115, 246, 241, 255);
                    // counterBar0.GetComponent<ScaleBouncedObject>().SetEffectFactor(0, Time.time);
                }
                else if (i == 1)
                {
                    counterBar1.color = new Color32(115, 246, 241, 255);
                    // counterBar1.GetComponent<ScaleBouncedObject>().SetEffectFactor(0, Time.time);
                }
                else
                {
                    counterBar2.color = new Color32(115, 246, 241, 255);
                    // counterBar2.GetComponent<ScaleBouncedObject>().SetEffectFactor(0, Time.time);
                }
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


    public void UpdateMoney(int currentMoney, int changedMoney)
    {
        if (currentUiId == 6) return;
        StartCoroutine(DoUpdateMoney(currentMoney, changedMoney));
    }

    IEnumerator DoUpdateMoney(int currentMoney, int changedMoney)
    {
        float duration = 0.5f;
        float delay = 0.05f;
        WaitForSeconds wfs = new(delay);

        int changeTimes = Mathf.CeilToInt(duration / delay);
        txtMoney.text = (currentMoney - changedMoney).ToString();
        moneyContainer.SetActive(true);
        yield return wfs;
        
        for (int i = changeTimes - 1; i >= 0; i--)
        {
            yield return wfs;
            txtMoney.text = Mathf.CeilToInt(currentMoney - changedMoney * (float)i / changeTimes).ToString();
        }
        txtMoney.text = currentMoney.ToString();

        yield return new WaitForSeconds(2);
        moneyContainer.SetActive(false);
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
            draggingItem.SetActive(true);
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
            draggingItem.SetActive(false);
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


    public void OpenShop()
    {
        currentUiId = 6;

        pauseGameAction.Disable();
        openFunctionAction.Disable();
        Time.timeScale = 0;

        playerStats.SetActive(false);
        moneyContainer.SetActive(false);
        shop.SetActive(true);
    }

    public void CloseShop()
    {
        currentUiId = 0;

        pauseGameAction.Enable();
        openFunctionAction.Enable();
        Time.timeScale = 1;

        shop.SetActive(false);
        playerStats.SetActive(true);
    }


    public void UpdateInventory(int itemId, int skillId)
    {
        inventoryController.gameObject.SetActive(true);
        functionMenu.SetActive(true);

        if (skillId > -1) skillManager.UnlockSkill(skillId);
        if (itemId > 0)
        {
            inventoryController.AddItem(itemId);
        }

        inventoryController.gameObject.SetActive(false);
        functionMenu.SetActive(false);
    }

    public void UpdateSkillTree(int skillId)
    {
        
    }


    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
        Destroy(gameObject);
    }


    public void ChangePauseGameActionBinding(string binding)
    {
        pauseGameAction.ApplyBindingOverride(binding);
    }

    public void ChangeOpenFunctionActionBinding(string binding)
    {
        openFunctionAction.ApplyBindingOverride(binding);
    }


    public void ActivateSkill(int skillId)
    {
        skillManager.ChangeActiveSkill(skillId);
    }
}
