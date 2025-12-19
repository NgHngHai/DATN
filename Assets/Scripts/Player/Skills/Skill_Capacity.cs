using UnityEngine;

public class Skill_Capacity : MonoBehaviour, ISkillPassive
{
    public int newMaxEnergy = 12;

    public void SetPassiveActive(GameObject player)
    {
        var playerSkillManager = player.GetComponent<PlayerSkillManager>();

        //if (active)
        //{
        //}
            playerSkillManager.maxEnergy = newMaxEnergy;
    }
}
