using UnityEngine;

public class Skill_Haste : MonoBehaviour, ISkillPassive
{
    public int newMoveSpeed = 9;

    public void SetPassiveActive(GameObject player)
    {
        var playerController = player.GetComponent<PlayerController>();

        //if (active)
        //{
        playerController.moveSpeed = newMoveSpeed;
        //}
    }
}
