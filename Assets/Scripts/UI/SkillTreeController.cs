using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTreeController : MonoBehaviour
{
    public Sprite skillActivatedSprite, skillDeactivatedSprite;
    public GameObject skill_1, skill_2, skill_3, skill_4, skill_5, skill_6, skill_7, skill_8, skill_9;
    public Image imgSkill_3, imgSkill_4, imgSkill_8;
    public TextMeshProUGUI txtSkillName, txtSkillDescription;

    int selectingSkill;
    List<SkillData> skillData;
    List<GameObject> skills;
    bool[] lockStates;


    void Awake()
    {
        LoadSkillDataFromSO();
        LoadLockStateFromSaveFile();

        skills = new()
        {
            skill_1, skill_2, skill_3, skill_4, skill_5, skill_6, skill_7, skill_8, skill_9
        };

        selectingSkill = 0;
        DisplayData(0);
    }

    public void DisplayData(int id)
    {
        txtSkillName.text = skillData[id].name;
        txtSkillDescription.text = skillData[id].description;

        transform.GetChild(0).GetChild(selectingSkill).GetChild(0).gameObject.SetActive(false);
        selectingSkill = id;
    }


    void LoadSkillDataFromSO ()
    {
        skillData = new List<SkillData>
        {
            new("Stay Alive !", "The most fundamental skill to mastering a newer and shinier skill.\nYou only got one life (or maybe not?) so try to stay alive until you gain yourself a skill point.\nHapply exploring, don't die!"),
            new("Double Jump", "Defy physics and you shall reach higher places."),
            new("Health I", "Increase your health by 25."),
            new("Counter", "The ability to defend yourself agaisnt dangerous enemies and give them a taste of their own medicine."),
            new("Stomp", "Upon reaching a certain height threshold, you can slam back to the ground, dealing massive damage to enemies and immediately get out of danger's way."),
            new("LOCKED", ""),
            new("Speed I", "Increase your model's flexibility and speed by 15%."),
            new("LOCKED", ""),
            new("Dash Charge", "You can now dash, but also dealing damage to anyone dare to stop you on your path."),
            new("Energy I", "Increase your energy pool by 3."),
            new("LOCKED", ""),
            new("LOCKED", ""),
            new("LOCKED", ""),
            new("LOCKED", ""),
            new("LOCKED", ""),
            new("LOCKED", "")
        };
    }

    void LoadLockStateFromSaveFile()
    {
        lockStates = new bool[15];
        lockStates[0] = true;

    }

    
    public bool IsSelectingSkill(int id)
    {
        return selectingSkill == id;
    }


    public void UnlockSkill(int id)
    {
        skills[id].SetActive(true);
    }


    public void ChangeActiveSkill(int id)
    {
        if (id == 3)
        {
            imgSkill_3.sprite = skillActivatedSprite;
            imgSkill_4.sprite = skillDeactivatedSprite;
            imgSkill_8.sprite = skillDeactivatedSprite;
        }
        else if (id == 4)
        {
            imgSkill_3.sprite = skillDeactivatedSprite;
            imgSkill_4.sprite = skillActivatedSprite;
            imgSkill_8.sprite = skillDeactivatedSprite;
        }
        else
        {
            imgSkill_3.sprite = skillDeactivatedSprite;
            imgSkill_4.sprite = skillDeactivatedSprite;
            imgSkill_8.sprite = skillActivatedSprite;
        }
    }
}


public class SkillData
{
    public string name;
    public string description;

    public SkillData(string skillName, string skillDescription)
    {
        name = skillName;
        description = skillDescription;
    }
}