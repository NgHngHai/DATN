using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTreeController : MonoBehaviour
{
    public TextMeshProUGUI txtSkillName, txtSkillDescription;
    int selectingSkill;
    List<SkillData> skillData;

    void Awake()
    {
        LoadSkillDataFromSO();

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


    public bool IsSelectingSkill(int id)
    {
        return selectingSkill == id;
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
