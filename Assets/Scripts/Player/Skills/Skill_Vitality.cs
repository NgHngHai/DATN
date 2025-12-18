using UnityEngine;

public class Skill_Vitality : MonoBehaviour, ISkillPassive
{
    public int newMaxHealth = 70;

    public void SetPassiveActive(bool active, GameObject player)
    {
        var playerHealth = player.GetComponent<Health>();

        if (active)
        {
            var diff = newMaxHealth - playerHealth.maxHealth;
            playerHealth.maxHealth = newMaxHealth;
            playerHealth.currentHealth += diff;
        }
    }
}
