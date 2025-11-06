using UnityEngine;

public class Skill_DJump : MonoBehaviour, ISkill
{
    // References
    public GameObject player;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
    }

    public void Activate()
    {
        Debug.Log("Double jump activated.");
    }
}
