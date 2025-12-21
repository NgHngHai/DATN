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
    public string playerLinkDoorID;

    // Checkpoint
    [Header("Checkpoint Save")]
    [Tooltip("Room name containing the last checkpoint used.")]
    public string lastCheckpointRoomName = null;

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
            if (skill.isUnlocked && skill != null)
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

            lastCheckpointRoomName = lastCheckpointRoomName,
        };
    }

    public override void RestoreState(object state)
    {
        var playerState = Utility.ConvertState<PlayerState>(state);

        playerHealth.maxHealth = playerState.maxHealth;
        playerHealth.currentHealth = playerState.currentHealth;

        playerHealth.OnHealthChanged?.Invoke(playerState.currentHealth, playerState.maxHealth);

        playerSkill.maxEnergy = playerState.maxEnergy;
        playerSkill.currentEnergy = playerState.currentEnergy;

        playerSkill.OnEnergyChanged?.Invoke(playerState.currentEnergy, playerState.maxEnergy);

        playerMoney.CurrentMoney = playerState.money;
        playerMoney.OnMoneyChanged?.Invoke(playerState.money, 0);

        lastCheckpointRoomName = playerState.lastCheckpointRoomName;

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

    public string GetDisplayContentForFileData()
    {
        return $"Health: {playerHealth.currentHealth} / {playerHealth.maxHealth} - Money: {playerMoney.CurrentMoney}";
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
        public string lastCheckpointRoomName;
    }
}
