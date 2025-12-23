using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class ControlsController : MonoBehaviour
{
    [Header("Reference")]
    public ControlsButton imgMapping0;
    public ControlsButton imgMapping1, imgMapping2, imgMapping3, imgMapping4, imgMapping5, imgMapping6;
    public ControlsButton imgMapping7, imgMapping8, imgMapping9, imgMapping10, imgMapping11, imgMapping12;
    [Header("Data")]
    public Sprite shortBtnUnselectedSprite;
    public Sprite shortBtnSelectedSprite, longBtnUnselectedSprite, longBtnSelectedSprite;
    [Header("input Action")]
    public InputActionReference moveActionRef;
    public InputActionReference jumpActionRef, attackActionRef, parryActionRef, dashActionRef, skillActionRef, healActionRef, interactActionRef;

    int selectingControlId;
    List<ControlsButton> imgMappings = new();
    List<GameObject> goRebindingBoxes = new();
    List<TextMeshProUGUI> txtRebindingKeys = new();

    System.IDisposable _buttonListener;
    int step = 0;
    string keyname;
    bool isHoveringMappingKeyButton;
    List<string> usedKeys = new();


    void Awake()
    {
        LoadKeyUsedFromSaveFile();

        imgMappings.Add(imgMapping0);
        imgMappings.Add(imgMapping1);
        imgMappings.Add(imgMapping2);
        imgMappings.Add(imgMapping3);
        imgMappings.Add(imgMapping4);
        imgMappings.Add(imgMapping5);
        imgMappings.Add(imgMapping6);
        imgMappings.Add(imgMapping7);
        imgMappings.Add(imgMapping8);
        imgMappings.Add(imgMapping9);
        imgMappings.Add(imgMapping10);
        imgMappings.Add(imgMapping11);
        imgMappings.Add(imgMapping12);
        for (int i = 0; i < 13; i++)
        {
            goRebindingBoxes.Add(imgMappings[i].transform.GetChild(2).gameObject);
            txtRebindingKeys.Add(goRebindingBoxes[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>());
        }

        selectingControlId = -1;
        step = 0;
        keyname = "";
        isHoveringMappingKeyButton = false;
    }


    public void FocusButton(int id)
    {
        _buttonListener = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
        
        imgMappings[id].ChangeSprite(shortBtnSelectedSprite, longBtnSelectedSprite);
        goRebindingBoxes[id].SetActive(true);
        txtRebindingKeys[id].text = "";

        if (selectingControlId != -1)
        {
            imgMappings[selectingControlId].ChangeSprite(shortBtnUnselectedSprite, longBtnUnselectedSprite);
            goRebindingBoxes[selectingControlId].SetActive(false);
            txtRebindingKeys[selectingControlId].text = "";
        }
        selectingControlId = id;
    }


    public bool IsSelectingControlId(int id) => selectingControlId == id;

    public void SetHoverState(bool state) => isHoveringMappingKeyButton = state;


    public void OnAnyButtonPress(InputControl ic)
    {
        if (ic.name == "leftButton")
        {
            if (!isHoveringMappingKeyButton)
            {
                _buttonListener?.Dispose();
                _buttonListener = null;

                step = 0;
                keyname = "";

                if (selectingControlId != -1)
                {
                    imgMappings[selectingControlId].ChangeSprite(shortBtnUnselectedSprite, longBtnUnselectedSprite);
                    goRebindingBoxes[selectingControlId].SetActive(false);
                    txtRebindingKeys[selectingControlId].text = "";
                }
                selectingControlId = -1;
            }
            return;
        }
        if (ic.name == "middleButton" || ic.name == "rightButton") return;
        
        if (step == 0)
        {
            if (ic.name == usedKeys[selectingControlId] || !usedKeys.Contains(ic.name))
            {
                step = 1;
                txtRebindingKeys[selectingControlId].color = new(1, 1, 1, 1);
            }
            else
            {
                step = 0;
                txtRebindingKeys[selectingControlId].color = new Color32(255, 96, 96, 255);
            }
            keyname = ic.name;
            txtRebindingKeys[selectingControlId].text = keyname;
        }
        else if (step == 1)
        {
            if (ic.name == "enter")
            {
                step = 0;
                ModifyControlButtonUI();
                RebindingAction();
                _buttonListener?.Dispose();
                _buttonListener = null;
                selectingControlId = -1;
                return;
            }
            else if (ic.name == "backspace")
            {
                step = 0;
                txtRebindingKeys[selectingControlId].text = "";
                return;
            }
        }
    }


    public void RebindingAction()
    {
        switch (selectingControlId)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                moveActionRef.action.ApplyBindingOverride(2, "<Keyboard>/" + keyname);
                break;
            case 3:
                moveActionRef.action.ApplyBindingOverride(3, "<Keyboard>/" + keyname);
                break;
            case 4:
                jumpActionRef.action.ApplyBindingOverride("<Keyboard>/" + keyname);
                break;
            case 5:
                UIManager.Instance.ChangeOpenFunctionActionBinding("<Keyboard>/" + keyname);
                break;
            case 6:
                UIManager.Instance.ChangePauseGameActionBinding("<Keyboard>/" + keyname);
                break;
            case 7:
                attackActionRef.action.ApplyBindingOverride("<Keyboard>/" + keyname);
                break;
            case 8:
                // parryActionRef.action.ApplyBindingOverride("<Keyboard>/" + keyname);
                break;
            case 9:
                dashActionRef.action.ApplyBindingOverride("<Keyboard>/" + keyname);
                break;
            case 10:
                skillActionRef.action.ApplyBindingOverride("<Keyboard>/" + keyname);
                break;
            case 11:
                healActionRef.action.ApplyBindingOverride("<Keyboard>/" + keyname);
                break;
            case 12:
                interactActionRef.action.ApplyBindingOverride("<Keyboard>/" + keyname);
                break;
        }
    }


    public void ModifyControlButtonUI()
    {
        usedKeys[selectingControlId] = keyname;

        txtRebindingKeys[selectingControlId].text = "";
        goRebindingBoxes[selectingControlId].SetActive(false);
        
        imgMappings[selectingControlId].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = keyname;

        if (keyname.Length > 1)
        {
            imgMappings[selectingControlId].type = 1;
        }
        else
        {
            imgMappings[selectingControlId].type = 0;
        }
        imgMappings[selectingControlId].ChangeSprite(shortBtnUnselectedSprite, longBtnUnselectedSprite);
    }
    

    void OnDisable()
    {
        _buttonListener?.Dispose();
        _buttonListener = null;

        step = 0;
        keyname = "";

        if (selectingControlId != -1)
        {
            imgMappings[selectingControlId].ChangeSprite(shortBtnUnselectedSprite, longBtnUnselectedSprite);
            goRebindingBoxes[selectingControlId].SetActive(false);
            txtRebindingKeys[selectingControlId].text = "";
        }
        selectingControlId = -1;
        isHoveringMappingKeyButton = false;
    }


    public void LoadKeyUsedFromSaveFile()
    {
        usedKeys.Add("w");
        usedKeys.Add("s");
        usedKeys.Add("a");
        usedKeys.Add("d");
        usedKeys.Add("space");
        usedKeys.Add("tab");
        usedKeys.Add("esc");
        usedKeys.Add("j");
        usedKeys.Add("k");
        usedKeys.Add("l");
        usedKeys.Add("f");
        usedKeys.Add("r");
        usedKeys.Add("e");
    }
}