using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Video;
using Unity.Jobs;
using JetBrains.Annotations;

public class ShopController : MonoBehaviour
{
    [Header("Object Reference")]
    public RectTransform scroller;
    public ScrollRect scrollRect;
    public Transform itemList;
    public GameObject itemDetailsFrame, itemName;
    public TextMeshProUGUI txtItemName, txtItemDescription1, txtItemDescription2;
    public TypewriterEffect npcDialog;
    public Image imgPurchaseBtn, imgExitBtn;
    public GameObject confirmDialog;
    public Image imgConfirmPurchaseBtn, imgExitConfirmDialogBtn;
    public TextMeshProUGUI txtMoney;
    [Header("Player Data")]
    public PlayerMoneyManager moneyManager;
    [Header("Other Data")]
    public Sprite unselectedItemSprite;
    public Sprite selectedItemSprite, unselectedButtonSprite, selectedButtonSprite;

    InputAction purchaseAction, exitAction;
    bool[] itemsSoldOut;
    ShopItemData[] shopItemList;
    int selectingItemId, hoveringItemId;
    int step;
    int currentMoney;

    void Awake()
    {
        LoadShopItemDataFromSO();
        purchaseAction = new("", InputActionType.Button, "<Keyboard>/f");
        exitAction = new("", InputActionType.Button, "<Keyboard>/a");

        itemsSoldOut = new bool[9];
        for (int i = 0; i < 9; i++)
        {
            if (itemsSoldOut[i]) itemList.GetChild(i).gameObject.SetActive(false);
        }

        selectingItemId = -1;
        hoveringItemId = 0;
        step = 0;
    }


    public void OnEnable()
    {
        purchaseAction.Enable();
        exitAction.Enable();
        
        currentMoney = moneyManager.GetCurrentMoney();
        txtMoney.text = currentMoney.ToString();
    }


    void Update()
    {
        scroller.anchoredPosition = Vector2.Lerp(new(0, -553), new(0, 2), Mathf.Clamp01(scrollRect.verticalNormalizedPosition));
        if (purchaseAction.WasPressedThisFrame())
        {
            HoldPurchaseButton();
        }
        else if (purchaseAction.WasReleasedThisFrame())
        {
            ReleasePurchaseButton();
        }
        else if (exitAction.WasPressedThisFrame())
        {
            HoldExitButton();
        }
        else if (exitAction.WasReleasedThisFrame())
        {
            ReleaseExitButton();
        }
    }


    public void HoverItem(int id)
    {
        hoveringItemId = id;
    }


    public void DisplayItemData(int id)
    {
        if (id == selectingItemId) return;
        itemDetailsFrame.SetActive(true);
        UnselectItem(selectingItemId);
        selectingItemId = id;
        SelectItemHovered();
    }


    public void UnselectItem(int id)
    {
        if (id == -1) return;
        StartCoroutine(UnselectItemRoutine());
    }
    IEnumerator UnselectItemRoutine()
    {
        RectTransform rect = itemList.GetChild(selectingItemId).GetComponent<RectTransform>();
        rect.GetComponent<Image>().sprite = unselectedItemSprite;
        rect.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(248, 237, 209, 255);
        rect.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(248, 237, 209, 255);
        while (rect.sizeDelta.x > 320)
        {
            rect.sizeDelta -= new Vector2(360 * Time.unscaledDeltaTime, 0);
            if (rect.sizeDelta.x < 320) rect.sizeDelta = new(320, 130);
            yield return null;
        }
    }


    public void SelectItemHovered()
    {
        StartCoroutine(SelectItemRoutine());
    }

    IEnumerator SelectItemRoutine()
    {
        ShopItemData data = shopItemList[selectingItemId];
        txtItemName.text = data.name;
        txtItemDescription1.text = data.description1;
        txtItemDescription2.text = data.description2;
        npcDialog.ShowText(shopItemList[selectingItemId].npcDialog);
        RectTransform rect = itemList.GetChild(selectingItemId).GetComponent<RectTransform>();
        rect.GetComponent<Image>().sprite = selectedItemSprite;
        rect.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 10, 31, 255);
        rect.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(0, 10, 31, 255);
        while (rect.sizeDelta.x < 350)
        {
            rect.sizeDelta += new Vector2(360 * Time.unscaledDeltaTime, 0);
            if (rect.sizeDelta.x > 350) rect.sizeDelta = new(350, 130);
            yield return null;
        }
    }


    public void BuyItem()
    {
        UnselectItem(selectingItemId);
        if (moneyManager.TrySpend(shopItemList[selectingItemId].price))
        {
            itemList.GetChild(selectingItemId).gameObject.SetActive(false);
            itemsSoldOut[selectingItemId] = true;
            UIManager.Instance.UpdateInventory(shopItemList[selectingItemId].inventoryId, shopItemList[selectingItemId].skillId);
            StartCoroutine(SpendMoney());
        }
        else
        {
            selectingItemId = -1;
        }
    }

    IEnumerator SpendMoney()
    {
        float duration = 0.5f;
        float delay = 0.05f;
        WaitForSeconds wfs = new(delay);
        int spendMoney = Mathf.Min(currentMoney, shopItemList[selectingItemId].price);
        moneyManager.ForceSpend(spendMoney);
        int changeTimes = Mathf.CeilToInt(duration / delay);
        for (int i = 1; i <= changeTimes; i++)
        {
            yield return wfs;
            txtMoney.text = Mathf.CeilToInt(currentMoney - spendMoney * (float)i / changeTimes).ToString();
        }
        currentMoney = moneyManager.GetCurrentMoney();
        txtMoney.text = currentMoney.ToString();
        selectingItemId = -1;
    }


    public void HoldPurchaseButton()
    {
        if (step == 0)
        {
            if (selectingItemId == -1) return;
            imgPurchaseBtn.sprite = selectedButtonSprite;
        }
        else
        {
            imgConfirmPurchaseBtn.sprite = selectedButtonSprite;
        }
    }


    public void ReleasePurchaseButton()
    {
        if (step == 0)
        {
            if (selectingItemId == -1) return;
            step = 1;
            imgPurchaseBtn.transform.GetChild(0).gameObject.SetActive(false);
            imgExitBtn.transform.GetChild(0).gameObject.SetActive(false);
            confirmDialog.SetActive(true);
            HideHoveringItemSelector();
            imgPurchaseBtn.sprite = unselectedButtonSprite;
        }
        else
        {
            step = 0;
            imgConfirmPurchaseBtn.transform.GetChild(0).gameObject.SetActive(false);
            imgExitConfirmDialogBtn.transform.GetChild(0).gameObject.SetActive(false);
            confirmDialog.SetActive(false);
            imgConfirmPurchaseBtn.sprite = unselectedButtonSprite;
            BuyItem();
        }
    }


    public void HoldExitButton()
    {
        if (step == 0)
        {
            imgExitBtn.sprite = selectedButtonSprite;
        }
        else
        {
            imgExitConfirmDialogBtn.sprite = selectedButtonSprite;
        }
    }


    public void ReleaseExitButton()
    {
        if (step == 0)
        {
            imgPurchaseBtn.transform.GetChild(0).gameObject.SetActive(false);
            imgExitBtn.transform.GetChild(0).gameObject.SetActive(false);
            imgExitBtn.sprite = unselectedButtonSprite;
            UIManager.Instance.CloseShop();
        }
        else
        {
            step = 0;
            imgConfirmPurchaseBtn.transform.GetChild(0).gameObject.SetActive(false);
            imgExitConfirmDialogBtn.transform.GetChild(0).gameObject.SetActive(false);
            confirmDialog.SetActive(false);
            imgExitConfirmDialogBtn.sprite = unselectedButtonSprite;
        }
    }


    public void HideHoveringItemSelector()
    {
        itemList.GetChild(hoveringItemId).GetChild(0).gameObject.SetActive(false);
    }


    void OnDisable()
    {
        if (selectingItemId != -1)
        {
            Transform t = itemList.GetChild(selectingItemId);
            t.GetComponent<RectTransform>().sizeDelta = new (320, 130);
            t.GetComponent<Image>().sprite = unselectedItemSprite;
        }
        HideHoveringItemSelector();

        imgConfirmPurchaseBtn.transform.GetChild(0).gameObject.SetActive(false);
        imgExitConfirmDialogBtn.transform.GetChild(0).gameObject.SetActive(false);

        purchaseAction.Disable();
        exitAction.Disable();
    }


    void OnDestroy()
    {
        purchaseAction.Dispose();
        exitAction.Dispose();
    }


    void LoadShopItemDataFromSO()
    {
        shopItemList = new ShopItemData[9] {
            new ("Counter", 2, 3, 50, "The ability to defend yourself agaisnt dangerous enemies and give them a taste of their own medicine.", "Was violent behavior necessary?\nNot always, but instead of being helpless agaisnt hostile beings, you can now choose to be violent too."),
            new ("Speed I", -1, 6, 50, "Increase your model's flexibility and speed by 15%.", "Your legs may be short, but it shall not affect your ability to be speedy."),
            new ("Double jump", -1, 1 , 50, "Defy physics and you shall reach higher places.", "Like the top shelf of the storage."),
            new ("Stomp", 3, 4, 50, "Upon reaching a certain height threshold, you can slam back to the ground, dealing massive damage to enemies and immediately get out of danger's way.", ""),
            new ("Dash Charge", 4, 8, 50, "You can now dash, but also dealing damage to anyone dare to stop you on your path.", "No one shall stop you and your little legs."),
            new ("Health I", -1, 2, 50, "Increase your health by 25.", "Allow you to withstand more damage."),
            new ("Energy I", -1, 9, 50, "Increase your energy pool by 3.", "The more, the merrier. Happy spamming skill!"),
            new ("Dash charge chip", 0, -1, 50, "I found it by the waste processor. It's not really useful for me, but who knows? Maybe you can use it.", "Install the chip in Inventory to learn the ability \"Dash charge\" - Dash forward to deal massive damage to any enemies within your path.", "Oooh~ Execellent choice, little cat. You got an eye for artifacts."),
            new ("Stomp chip", 0, -1, 50, "Yet another out dated chip. Do you ever wonder who made these chips in the first place?", "Install the chip in Inventory to learn the ability \"Stomp\" - Smash downward to destroy any enemies that stand in your path.", "You gotta be the only one that can use this rusty thing right now. For that, i'll give you a specical discount~"),
        };
    }
}


public class ShopItemData
{
    public string name;
    public int inventoryId;
    public int skillId;
    public int price;
    public string description1, description2;
    public string npcDialog;

    public ShopItemData (string itemName, int inventory, int skill, int itemPrice, string description_1, string description_2, string dialog = "Do you even have enough money for that? No offense, buddy, but business is business.\nExcept you're willing to do a favour for me...?")
    {
        name = itemName;
        inventoryId = inventory;
        skillId = skill;
        price = itemPrice;
        description1 = description_1;
        description2 = description_2;
        npcDialog = dialog;
    }
}
