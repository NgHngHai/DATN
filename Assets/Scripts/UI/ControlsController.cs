using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsController : MonoBehaviour
{
    [Header("Reference")]
    public Image imgMapping0;
    public Image imgMapping1, imgMapping2, imgMapping3, imgMapping4, imgMapping5, imgMapping6;
    public Image imgMapping7, imgMapping8, imgMapping9, imgMapping10, imgMapping11, imgMapping12;
    [Header("Data")]
    public Sprite shortBtnUnselectedSprite;
    public Sprite shortBtnSelectedSprite, longBtnUnselectedSprite, longBtnSelectedSprite;
    int selectingControlId;
    List<Image> imgMappings = new();


    void Awake()
    {
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

        selectingControlId = -1;
    }


    public void FocusButton(int id, int type)
    {
        if (type == 0)
        {
            imgMappings[id].sprite = shortBtnSelectedSprite;
        }
        else
        {
            imgMappings[id].sprite = longBtnSelectedSprite;
        }

        if (selectingControlId != -1)
        {
            if (type == 0)
            {
                imgMappings[selectingControlId].sprite = shortBtnUnselectedSprite;
            }
            else
            {
                imgMappings[selectingControlId].sprite = longBtnUnselectedSprite;
            }
            imgMappings[selectingControlId].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color32(144, 169, 177, 255);
        }
        selectingControlId = id;
    }


    public bool IsSelectingControlId(int id) => selectingControlId == id;
}
