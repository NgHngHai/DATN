using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveables : MonoBehaviour, ISaveable
{
    [SerializeField] private string uniqueID;

    // Component references
    private Health playerHealth;
    private PlayerMoneyManager playerMoney;
    private PlayerSkillManager playerSkill;
    private PlayerController playerController;

    private void Start()
    {
        playerHealth = GetComponent<Health>();
        playerMoney = GetComponent<PlayerMoneyManager>();
        playerSkill = GetComponent<PlayerSkillManager>();
        playerController = GetComponent<PlayerController>();
    }

    public string GetUniqueID() => uniqueID;
    public object CaptureState()
    {
        return new PlayerState
        {
            maxHealth = playerHealth.maxHealth,
            currentHealth = playerHealth.currentHealth,

            maxEnergy = playerSkill.maxEnergy,
            currentEnergy = playerSkill.currentEnergy,

            skills = playerSkill.skills,

            money = playerMoney.CurrentMoney,

            currentRoomID = playerController.currentRoomID,
            position = transform.position
        };
    }

    public void RestoreState(object state)
    {
            
    }

    [System.Serializable]
    private struct PlayerState
    {
        // Health
        public int maxHealth;
        public int currentHealth;

        // Energy
        public int maxEnergy;
        public int currentEnergy;

        // Skills
        public List<SkillDefinition> skills;

        // Money
        public int money;

        // Current room & position
        public string currentRoomID;
        public Vector3 position;
    }
}
