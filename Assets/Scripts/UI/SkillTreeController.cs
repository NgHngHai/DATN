using TMPro;
using UnityEngine;

public class SkillTreeController : MonoBehaviour
{
    public TextMeshProUGUI txtSkillName, txtSkillDescription;
    int selectingSkill;

    void Awake()
    {
        selectingSkill = 0;
        DisplayData(0);
    }

    public void DisplayData(int id)
    {
        if (id == 0)
        {
            txtSkillName.text = "stay alive!";
            txtSkillDescription.text = "The most fundamental skill to mastering a newer and shinier skill.\nYou only got one life (or maybe not?) so try to stay alive until you gain yourself a skill point.\nHapply exploring, don't die!";
        }
        else
        {
            txtSkillName.text = "skill " + id;
            txtSkillDescription.text = "An important skill !!!";
        }

        transform.GetChild(0).GetChild(selectingSkill).GetChild(0).gameObject.SetActive(false);
        selectingSkill = id;
    }


    public bool IsSelectingSkill(int id)
    {
        return selectingSkill == id;
    }
}
