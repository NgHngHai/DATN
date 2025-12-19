using UnityEngine;

public class Skill_DJump : MonoBehaviour, ISkillPassive
{
    public int extraJumpCount = 1;

    public void SetPassiveActive(GameObject player)
    {
        var playerController = player.GetComponent<PlayerController>();

        //if (active)
            playerController.maxExtraJumpCount = extraJumpCount;
    }    
}
