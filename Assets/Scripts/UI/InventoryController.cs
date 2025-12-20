using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [Header("Data")]
    public Sprite item0;
    public Sprite item1, item2, item3, skill0, skill1, skill2;
    [Header("Reference")]
    public TextMeshProUGUI txtItemName;
    public TextMeshProUGUI txtItemDescription;
    public Image draggingItem, anchoredSkill;
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
        slotUsed[2] = true;
        slotUsed[3] = true;

        itemIds = new int[21];
        itemIds[0] = 0;
        itemIds[1] = 1;
        itemIds[2] = 2;
        itemIds[3] = 3;

        selectingSlot = 0;
        hoveringSlot = -1;
        skillAnchored = -1;
        selectingSlot = -1;
        selectingItemData = inventoryData[0];
    }


    void Update()
    {
        if (canDrag && !isDragging && input.IsEligibleToDragItem())
        {
            isDragging = true;

            if (isHoveringSkillAnchor)
            {
                imgTmp = transform.GetChild(1).GetChild(0).GetComponent<Image>();
                imgTmp.sprite = skill0;

                draggingItem.color = new(1, 1, 1, 1);
                draggingItem.sprite = selectingItemData.itemSprite;
                // reset dragging item shader
            }
            else
            {
                imgTmp = transform.GetChild(0).GetChild(selectingSlot).GetChild(1).GetComponent<Image>();
                imgTmp.color = new (0, 0, 0, 0);

                draggingItem.color = new(1, 1, 1, 1);
                draggingItem.sprite = selectingItemData.itemSprite;
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
            txtItemName.text = selectingItemData.itemName;
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
                imgTmp.transform.parent.GetChild(0).gameObject.SetActive(false);
                if (skillAnchored == -1)
                {
                    anchoredSkill.sprite = selectingItemData.skillId == 0 ? skill1 : skill2;
                    slotUsed[selectingSlot] = false;
                    itemIds[20] = itemIds[selectingSlot];
                    skillAnchored = selectingItemData.skillId;
                }
                else
                {
                    imgTmp.color = new(1, 1, 1, 1);
                    imgTmp.sprite = inventoryData[itemIds[20]].itemSprite;
                    anchoredSkill.sprite = inventoryData[itemIds[selectingSlot]].skillId == 0 ? skill1 : skill2;
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


    void LoadInventoryDataFromSO()
    {
        inventoryData = new List<InventoryData>
        {
            new("Item 1", "A nessessary item !!!", -1, item0),
            new("Item 2", "A nessessary item !!!", -1, item1),
            new("Skill 1", "A nessessary skill !!!", 0, item2),
            new("Skill 2", "A nessessary skill !!!", 1, item3),
        };
    }
}



class InventoryData
{
    public string itemName;
    public string itemDescription;
    public int skillId;
    public Sprite itemSprite;

    public InventoryData(string name, string description, int skill, Sprite sprite)
    {
        itemName = name;
        itemDescription = description;
        skillId = skill;
        itemSprite = sprite;
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
