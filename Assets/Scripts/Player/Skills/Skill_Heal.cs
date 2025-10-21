using UnityEngine;

public class Skill_Heal : MonoBehaviour, ISkill
{
    public void Activate()
    {
        Debug.Log("Heal activated.");
    }
}
