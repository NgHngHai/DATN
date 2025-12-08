using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveables : SaveableObject
{
    // Component references
    private Health playerHealth;
    private PlayerMoneyManager playerMoney;
    private PlayerSkillManager playerSkill;
    private PlayerController playerController;

    // Position params
    private string playerRoomID;
    private string playerLinkDoorID;

    private void Start()
    {
        playerHealth = GetComponent<Health>();
        playerMoney = GetComponent<PlayerMoneyManager>();
        playerSkill = GetComponent<PlayerSkillManager>();
        playerController = GetComponent<PlayerController>();
    }

    public override object CaptureState()
    {
        return new PlayerState
        {
            maxHealth = playerHealth.maxHealth,
            currentHealth = playerHealth.currentHealth,

            maxEnergy = playerSkill.maxEnergy,
            currentEnergy = playerSkill.currentEnergy,

            skills = playerSkill.skills,

            money = playerMoney.CurrentMoney,

            currentRoomID = playerRoomID,
            linkDoorID = playerLinkDoorID,
            position = transform.position
        };
    }

    public override void RestoreState(object state)
    {
        playerHealth.maxHealth = ((PlayerState)state).maxHealth;
        playerHealth.currentHealth = ((PlayerState)state).currentHealth;

        playerSkill.maxEnergy = ((PlayerState)state).maxEnergy;
        playerSkill.currentEnergy = ((PlayerState)state).currentEnergy;

        playerSkill.skills = ((PlayerState)state).skills;

        playerMoney.CurrentMoney = ((PlayerState)state).money;

        playerRoomID = ((PlayerState)state).currentRoomID;
        playerLinkDoorID = ((PlayerState)state).linkDoorID;
        transform.position = ((PlayerState)state).position;
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
        public string linkDoorID;
        public Vector3 position;
    }
}
