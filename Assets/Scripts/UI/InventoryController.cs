using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [Header("Data")]
    public Sprite item_0;
    public Sprite item_1;
    public Sprite skill_3, skill_4, skill_8;
    public Sprite anchoredSkill_none, anchoredSkill_3, anchoredSkill_4, anchoredSkill_8;
    [Header("Reference")]
    public TextMeshProUGUI txtItemName;
    public TextMeshProUGUI txtItemDescription;
    public Image draggingItem, anchoredSkill;
    public ItemFollowingMouse itemFollowingMouse;

    bool[] slotUsed;
    int[] itemIds;
    List<InventoryData> inventoryData;
    InventoryData selectingItemData;
    int selectingSlot;
    [HideInInspector] public int hoveringSlot;
    [HideInInspector] public bool isHoveringSkillAnchor;
    int skillAnchored;
    public bool canDrag, isDragging;
    Image imgTmp;
    InputActionReader input;


    void Awake()
    {
        input = new InputActionReader();
        LoadInventoryDataFromSO();

        slotUsed = new bool[20];
        slotUsed[0] = true;
        slotUsed[1] = true;


        itemIds = new int[21];
        itemIds[0] = 0;
        itemIds[1] = 1;

        skillAnchored = -1;
    }

    void OnEnable()
    {
        hoveringSlot = -1;
        selectingSlot = -1;
        selectingItemData = inventoryData[0];
        itemFollowingMouse.Hide();
    }

    void OnDisable()
    {
        if (selectingSlot != -1) transform.GetChild(0).GetChild(selectingSlot).GetChild(0).gameObject.SetActive(false);
    }


    void Update()
    {
        if (canDrag && !isDragging && input.IsEligibleToDragItem())
        {
            isDragging = true;

            // if (isHoveringSkillAnchor)
            // {
            //     imgTmp = transform.GetChild(1).GetChild(0).GetComponent<Image>();
            //     imgTmp.sprite = an;

            //     draggingItem.color = new(1, 1, 1, 1);
            //     draggingItem.sprite = selectingItemData.itemSprite;
            //     // reset dragging item shader...
            // }
            // else
            {
                imgTmp = transform.GetChild(0).GetChild(selectingSlot).GetChild(1).GetComponent<Image>();
                imgTmp.color = new (0, 0, 0, 0);

                draggingItem.color = new(1, 1, 1, 1);
                draggingItem.sprite = selectingItemData.itemSprite;
                itemFollowingMouse.Display();
            }
        }
    }


    public void DisplayData(int id)
    {
        selectingItemData = inventoryData[itemIds[id]];
        txtItemName.text = selectingItemData.itemName;
        txtItemDescription.text = selectingItemData.itemDescription;

        if (selectingSlot != -1) transform.GetChild(0).GetChild(selectingSlot).GetChild(0).gameObject.SetActive(false);
        selectingSlot = id;
    }


    public void DisplayAnchoredSkillData()
    {
        if (skillAnchored != -1)
        {
            selectingItemData = inventoryData[itemIds[20]];
            txtItemName.text = selectingItemData.itemName + "\n<size=24><color=#00A99D>-- ACTIVATED --</color></size>";
            txtItemDescription.text = selectingItemData.itemDescription;
            if (selectingSlot != -1) transform.GetChild(0).GetChild(selectingSlot).GetChild(0).gameObject.SetActive(false);
        }
    }

    
    public bool IsSelectingSlotIndex(int id)
    {
        return selectingSlot == id;
    }


    public bool HasSomethingInSlotIndex(int id)
    {
        return slotUsed[id];
    }


    public void SetInventoryState()
    {
        canDrag = true;
        input.draggingStartPos = Mouse.current.position.ReadValue();
    }
    public void ResetInventoryState()
    {
        canDrag = false;
        if (isDragging)
        {
            isDragging = false;
            draggingItem.color = new(0, 0, 0, 0);
            itemFollowingMouse.Hide();
        
            if (hoveringSlot >= 0)
            {
                if (slotUsed[hoveringSlot])
                {
                    imgTmp.color = new(1, 1, 1, 1);
                    imgTmp.sprite = inventoryData[itemIds[hoveringSlot]].itemSprite;
                    (itemIds[hoveringSlot], itemIds[selectingSlot]) = (itemIds[selectingSlot], itemIds[hoveringSlot]);
                    imgTmp.transform.parent.GetChild(0).gameObject.SetActive(false);

                    imgTmp = transform.GetChild(0).GetChild(hoveringSlot).GetChild(1).GetComponent<Image>();
                    imgTmp.sprite = selectingItemData.itemSprite;
                }
                else
                {
                    slotUsed[selectingSlot] = false;
                    imgTmp.transform.parent.GetChild(0).gameObject.SetActive(false);

                    imgTmp = transform.GetChild(0).GetChild(hoveringSlot).GetChild(1).GetComponent<Image>();
                    imgTmp.color = new(1, 1, 1, 1);
                    imgTmp.sprite = selectingItemData.itemSprite;
                    slotUsed[hoveringSlot] = true;
                    itemIds[hoveringSlot] = itemIds[selectingSlot];
                }
                    
                selectingSlot = hoveringSlot;
            }
            else if (isHoveringSkillAnchor && inventoryData[itemIds[selectingSlot]].skillId > -1)
            {
                imgTmp.transform.parent.GetChild(0).gameObject.SetActive(false); // deactive current selector
                UIManager.Instance.ActivateSkill(inventoryData[itemIds[selectingSlot]].skillId);
                txtItemName.text += "\n<size=24><color=#00A99D>-- ACTIVATED --</color></size>";
                if (skillAnchored == -1)
                {
                    anchoredSkill.sprite = selectingItemData.anchoredSkillSprite;
                    slotUsed[selectingSlot] = false;
                    itemIds[20] = itemIds[selectingSlot];
                    skillAnchored = selectingItemData.skillId;
                }
                else
                {
                    imgTmp.color = new(1, 1, 1, 1);
                    imgTmp.sprite = inventoryData[itemIds[20]].itemSprite;
                    anchoredSkill.sprite = inventoryData[itemIds[selectingSlot]].anchoredSkillSprite;
                    (itemIds[20], itemIds[selectingSlot]) = (itemIds[selectingSlot], itemIds[20]);
                    selectingSlot = -1;
                }
            }
            else
            {
                imgTmp.color = new(1, 1, 1, 1);
            }
        }

        
    }


    public void AddItem(int itemId)
    {
        for (int i = 0; i < 20; i++)
        {
            if (!slotUsed[i])
            {
                slotUsed[i] = true;
                itemIds[i] = itemId;

                Image img = transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<Image>();
                img.color = new (1, 1, 1, 1);
                img.sprite = inventoryData[itemId].itemSprite;
                if (inventoryData[itemId].skillId > -1) UnlockSkill(inventoryData[itemId].skillId);
                break;
            }
        }
    }


    public void UnlockSkill(int skillId)
    {
        UIManager.Instance.UpdateSkillTree(skillId);
    }


    void LoadInventoryDataFromSO()
    {
        inventoryData = new List<InventoryData>
        {
            new("Item 1", "A nessessary item !!!", -1, item_0),
            new("Item 2", "A nessessary item !!!", -1, item_1),
            new("[Counter] Guide", "The ability to defend yourself agaisnt dangerous enemies and give them a taste of their own medicine.", 3, skill_3, anchoredSkill_3),
            new("[Stomp] Guide", "Upon reaching a certain height threshold, you can slam back to the ground, dealing massive damage to enemies and immediately get out of danger's way.", 4, skill_4, anchoredSkill_4),
            new("[Dash Charge] Guide", "\"No one shall stop you and your little legs.\"", 8, skill_8, anchoredSkill_8),
        };
    }
}



class InventoryData
{
    public string itemName;
    public string itemDescription;
    public int skillId;
    public Sprite itemSprite;
    public Sprite anchoredSkillSprite;

    public InventoryData(string name, string description, int skill, Sprite item, Sprite anchoredSkill = null)
    {
        itemName = name;
        itemDescription = description;
        skillId = skill;
        itemSprite = item;
        anchoredSkillSprite = anchoredSkill;
    }
}


public class InputActionReader
{
    public Vector2 draggingStartPos;
    public float effectRotation;

    public bool IsEligibleToDragItem()
    {
        return Vector2.Distance(draggingStartPos, Mouse.current.position.ReadValue()) > 5f;
    }
}
