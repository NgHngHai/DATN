using UnityEngine;

public class Skill_DJump : MonoBehaviour, ISkillPassive
{
    public int extraJumpCount = 1;

    public void SetPassiveActive(bool active, GameObject player)
    {
        var playerController = player.GetComponent<PlayerController>();

        if (active)
            playerController.maxExtraJumpCount = extraJumpCount;
    }    
}
