using UnityEngine;

public class Skill_Heal : MonoBehaviour, ISkill
{
    [SerializeField] private int healAmount = 20;

    // References
    [SerializeField] private GameObject player;
    private Health healthComponent;

    private void Awake()
    {
        healthComponent = player.GetComponent<Health>();
    }

    // Heals the Health.cs component attached to gameObject
    public void Activate()
    {
        healthComponent.Heal(healAmount);
        Debug.Log("Heal activated.");
    }
}
