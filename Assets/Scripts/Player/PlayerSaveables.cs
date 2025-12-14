using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveables : SaveableObject
{
    // Component references
    private Health playerHealth;
    private PlayerMoneyManager playerMoney;
    private PlayerSkillManager playerSkill;
    private PlayerController playerController;

    // Skills
    private string _unlockedSkillsID;

    // Position params
    public string playerRoomID;
    public string playerLinkDoorID;

    // Checkpoint
    [Header("Checkpoint Save")]
    [Tooltip("Room name containing the last checkpoint used.")]
    public string lastCheckpointRoomName;

    protected override void Awake()
    {
        base.Awake();

        playerHealth = GetComponent<Health>();  
        playerMoney = GetComponent<PlayerMoneyManager>();
        playerSkill = GetComponent<PlayerSkillManager>();
        playerController = GetComponent<PlayerController>();
    }

    public override object CaptureState()
    {
        // Reset unlocked skills string
        _unlockedSkillsID = string.Empty;

        foreach (var skill in playerSkill.skills)
        {
            _unlockedSkillsID += skill.skillId + "-";
        }
        return new PlayerState
        {
            maxHealth = playerHealth.maxHealth,
            currentHealth = playerHealth.currentHealth,

            maxEnergy = playerSkill.maxEnergy,
            currentEnergy = playerSkill.currentEnergy,

            skills = _unlockedSkillsID,

            money = playerMoney.CurrentMoney,

            currentRoomID = playerRoomID,
            lastCheckpointRoomName = lastCheckpointRoomName,
            linkDoorID = playerLinkDoorID,
            position = transform.position
        };
    }

    public override void RestoreState(object state)
    {
        var playerState = (PlayerState)state;

        playerHealth.maxHealth = playerState.maxHealth;
        playerHealth.currentHealth = playerState.currentHealth;

        playerSkill.maxEnergy = playerState.maxEnergy;
        playerSkill.currentEnergy = playerState.currentEnergy;

        playerMoney.CurrentMoney = playerState.money;

        playerRoomID = playerState.currentRoomID;
        lastCheckpointRoomName = playerState.lastCheckpointRoomName;
        playerLinkDoorID = playerState.linkDoorID;
        transform.position = playerState.position;

        // Restore skills
        var savedSkills = playerState.skills ?? string.Empty;
        if (playerSkill.skills != null)
        {
            // First, lock all skills; then unlock those present in the saved list.
            for (int i = 0; i < playerSkill.skills.Count; i++)
            {
                playerSkill.skills[i].isUnlocked = false;
            }

            if (!string.IsNullOrEmpty(savedSkills))
            {
                var parts = savedSkills.Split(new[] { '-' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    if (int.TryParse(part, out int skillId))
                    {
                        // Find matching skill by id and unlock it.
                        for (int i = 0; i < playerSkill.skills.Count; i++)
                        {
                            // Assumes SkillDefinition has an int skillId property.
                            if (playerSkill.skills[i].skillId == skillId)
                            {
                                playerSkill.skills[i].isUnlocked = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    [System.Serializable]
    public struct PlayerState
    {
        // Health
        public int maxHealth;
        public int currentHealth;

        // Energy
        public int maxEnergy;
        public int currentEnergy;

        // Skills
        public string skills;

        // Money
        public int money;

        // Current room, position, checkpoint
        public string currentRoomID;
        public string lastCheckpointRoomName;
        public string linkDoorID;
        public Vector3 position;
    }
}
